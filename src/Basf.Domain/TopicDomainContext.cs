using Basf.Data;
using Basf.Domain.Event;
using Basf.Domain.Storage;
using Basf.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class TopicDomainContext //: IDomainContext
    {
        private ConcurrentDictionary<string, IAggRoot> aggRoots = new ConcurrentDictionary<string, IAggRoot>();
        private ConcurrentDictionary<Type, Func<IAggRoot, IDomainEvent, ActionResponse>> eventHandlers = new ConcurrentDictionary<Type, Func<IAggRoot, IDomainEvent, ActionResponse>>();
        private ConcurrentDictionary<string, BlockingCollection<Message<IDomainEvent>>> aggRootChanges = new ConcurrentDictionary<string, BlockingCollection<Message<IDomainEvent>>>();
        private IProducer producer = null;
        private IEventStore eventStore = null;
        private Func<IAggRoot, IDomainEvent, Task<ActionResponse>> acceptChange = null;
        public string Topic { get; private set; }
        public int MailBoxSize { get; private set; }
        public int MailBoxPartition { get; private set; }
        public string LocalRootPath { get; set; }
        public TopicDomainContext(IProducer producer, IEventStore eventStore)
        {
            this.producer = producer;
            this.eventStore = eventStore;
        }
        /// <summary>
        /// 初始化当前领域模型消费的Topic，领域事件队列的消费者个数，每个分区的消息最大消息个数
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="localRootPath">本地事件存储基路径</param>
        /// <param name="producerInitializer"></param>
        /// <param name="nConsumerCount">消费者个数，也是消费者队列的分区数</param>
        /// <param name="nMailBoxSize">每个队列分区的最大消息个数，建议有一个固定的大小，防止内存碎片，默认是50000</param>
        public void Initialize(string topic, string localRootPath, Action<IProducer> producerInitializer, int nConsumerCount = 15, int nMailBoxSize = 50000)
        {
            this.Topic = topic;
            this.LocalRootPath = localRootPath;
            this.acceptChange = HandlerFactory.CreateFuncHandler<IAggRoot, IDomainEvent, Task<ActionResponse>>("AcceptChange",
                BindingFlags.Instance | BindingFlags.NonPublic, typeof(AggRoot), typeof(IDomainEvent));
            producerInitializer?.Invoke(this.producer);
            this.MailBoxPartition = nConsumerCount;
            this.MailBoxSize = nMailBoxSize;
            for (int i = 0; i < this.MailBoxPartition; i++)
            {
                if (nMailBoxSize == -1)
                {
                    this.aggRootChanges.TryAdd(i.ToString(), new BlockingCollection<Message<IDomainEvent>>());
                }
                else
                {
                    this.aggRootChanges.TryAdd(i.ToString(), new BlockingCollection<Message<IDomainEvent>>(nMailBoxSize));
                }
            }
        }
        public void Start()
        {
            for (int i = 0; i < this.MailBoxPartition; i++)
            {
                Task.Run(async () =>
                {
                    Message<IDomainEvent> eventMessage = this.aggRootChanges[i.ToString()].Take();
                    IDomainEvent domainEvent = eventMessage.Body;
                    IAggRoot aggRoot = await this.GetAsync(domainEvent.AggRootId);
                    //先将事件的版本+1
                    domainEvent.Version = aggRoot.Version + 1;
                    ActionResponse<EventResult> result = await this.eventStore.AddAsync(domainEvent);
                    //if (result.Success == ActionResult.Failed)
                    //{
                    //    //TODO 先记录日志，再抛出异常
                    //    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
                    //    AppRuntime.ErrorFormat("事件:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                    //    return;
                    //}
                    //事件已经执行过，则返回
                    if (result.Data >= EventResult.Executed)
                    {
                        return;
                    }
                    //先验证事件版本的顺序，通过后，在事件的处理方法中，将聚合根的版本更新为事件的版本(+1后的版本)
                    var tResult = await this.acceptChange(aggRoot, domainEvent);
                    if (tResult.Success == ActionResult.Success)
                    {
                        await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Executed);
                    }
                    else
                    {
                        await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
                        AppRuntime.ErrorFormat("事件:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                    }
                });
            }
        }
        public int RoutingStrategy(IDomainEvent domainEvent)
        {
            string topic = domainEvent.AggRootType;
            string routingKey = domainEvent.AggRootId;
            return routingKey.GetHashCode() % this.MailBoxPartition;
        }
        public IAggRoot Get(string aggRootId)
        {
            return this.aggRoots[aggRootId];
        }
        public Task<IAggRoot> GetAsync(string aggRootId)
        {
            return Task.FromResult<IAggRoot>(this.aggRoots[aggRootId]);
        }
        public void Add(IAggRoot aggRoot)
        {
            this.aggRoots.TryAdd(aggRoot.UniqueId, aggRoot);
        }
        public Task AddAsync(IAggRoot aggRoot)
        {
            return Task.Run(() =>
            {
                this.aggRoots.TryAdd(aggRoot.UniqueId, aggRoot);
            });
        }
        public async Task<ActionResponse> ApplyChange(Message<IDomainEvent> eventMessage)
        {
            this.aggRootChanges[eventMessage.RoutingKey].Add(eventMessage);
            if (!this.aggRoots.ContainsKey(eventMessage.RoutingKey))
            {
                await this.AddAggRoot(eventMessage);
            }
            return ActionResponse.Success;
        }
        public ActionResponse Publish(IDomainEvent domainEvent)
        {
            try
            {
                var msg = new Message<IDomainEvent>(domainEvent);
                msg.RoutingKey = domainEvent.ToAggRootKey().ToString();
                this.producer.Publish(new Message<IDomainEvent>(domainEvent));
            }
            catch (Exception ex)
            {
                return ActionResponse.Fail((int)ActionResultCode.UnknownException, ex.Message, ex.ToString());
            }
            return ActionResponse.Success;
        }
        public async Task<ActionResponse> PublishAsync(IDomainEvent domainEvent)
        {
            try
            {
                await this.producer.PublishAsync(new Message<IDomainEvent>(domainEvent));
            }
            catch (Exception ex)
            {
                return ActionResponse.Fail((int)ActionResultCode.UnknownException, ex.Message, ex.ToString());
            }
            return ActionResponse.Success;
        }
        public ActionResponse InvokeHandler(IAggRoot aggRoot, IDomainEvent domainEvent)
        {
            try
            {
                Type aggRootType = aggRoot.GetType();
                Type eventType = domainEvent.GetType();
                var handler = this.eventHandlers.GetOrAdd(eventType,
                    HandlerFactory.CreateFuncHandler<IAggRoot, IDomainEvent, ActionResponse>("Handle",
                    BindingFlags.Instance | BindingFlags.Public, aggRootType, eventType));
                return handler.Invoke(aggRoot, domainEvent);
            }
            catch (Exception ex)
            {
                return ActionResponse.Fail((int)ActionResultCode.UnknownException, ex.Message, ex.ToString());
            }
        }
        public void AddHandler(Type aggRootType, Type eventType)
        {
            this.eventHandlers.TryAdd(eventType,
                 HandlerFactory.CreateFuncHandler<IAggRoot, IDomainEvent, ActionResponse>("Handle",
                 BindingFlags.Instance | BindingFlags.Public, aggRootType, eventType));
        }
        private async Task<ActionResponse> AddAggRoot(Message<IDomainEvent> eventMessage)
        {
            if (!this.aggRoots.ContainsKey(eventMessage.RoutingKey))
            {
                //从快照中获取聚合根
                IAggRoot aggRoot = await AppRuntime.Resolve<ISnapshotStore>().GetAsync(eventMessage.Topic, eventMessage.RoutingKey);
                List<IDomainEvent> domainEvents = await AppRuntime.Resolve<IEventStore>().FindAsync(eventMessage.Topic, eventMessage.RoutingKey, aggRoot.Version + 1);
                IEventSourcing eventSourcingAggRoot = aggRoot as IEventSourcing;
                await eventSourcingAggRoot.ReplayFrom(domainEvents);
                this.Add(aggRoot);
            }
            return ActionResponse.Success;
        }
        private async Task<ActionResponse> CreateSnapshotStore(Message<IDomainEvent> eventMessage)
        {
            return null;
        }
        private async Task CreateEventStore(IDomainEvent domainEvent)
        {
            IAggRoot aggRoot = await this.GetAsync(domainEvent.AggRootId);
            //先将事件的版本+1
            domainEvent.Version = aggRoot.Version + 1;

            ActionResponse<EventResult> result = await this.eventStore.AddAsync(domainEvent);
            //if (result.Success == ActionResult.Failed)
            //{
            //    //TODO 先记录日志，再抛出异常
            //    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
            //    AppRuntime.ErrorFormat("事件:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
            //    return;
            //}
            ////事件已经执行过，则返回
            //if (result.Data >= EventResult.Executed)
            //{
            //    return;
            //}
        }
        private async Task<ActionResponse> CreateLocalEventStore(IDomainEvent domainEvent)
        {
            //FileStyleUriParser.
            return null;
        }
    }
}
