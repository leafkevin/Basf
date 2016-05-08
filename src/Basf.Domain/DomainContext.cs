using Basf.Data;
using Basf.Domain.Event;
using Basf.Domain.Storage;
using Basf.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class DomainContext : IDomainContext
    {
        private ConcurrentDictionary<AggRootKey, IAggRoot> aggRoots = new ConcurrentDictionary<AggRootKey, IAggRoot>();
        private ConcurrentDictionary<Type, Func<IAggRoot, IDomainEvent, ActionResponse>> eventHandlers = new ConcurrentDictionary<Type, Func<IAggRoot, IDomainEvent, ActionResponse>>();
        private ConcurrentDictionary<int, BlockingCollection<IDomainEvent>> aggRootChanges = new ConcurrentDictionary<int, BlockingCollection<IDomainEvent>>();
        private IProducer producer = null;
        private IConsumer consumer = null;
        private IEventStore eventStore = null;
        private Func<IAggRoot, IDomainEvent, Task<ActionResponse>> acceptChange = null;
        public int MailBoxPartition { get; private set; }
        public DomainContext(IProducer producer, IConsumer consumer, IEventStore eventStore)
        {
            this.producer = producer;
            this.consumer = consumer;
            this.eventStore = eventStore;
        }
        public void Initialize(Action<IProducer> producerInitializer, Action<IConsumer> consumerInitializer)
        {
            this.acceptChange = HandlerFactory.CreateFuncHandler<IAggRoot, IDomainEvent, Task<ActionResponse>>("AcceptChange",
                BindingFlags.Instance | BindingFlags.NonPublic, typeof(AggRoot), typeof(IDomainEvent));
            producerInitializer?.Invoke(this.producer);
            consumerInitializer?.Invoke(this.consumer);
            this.MailBoxPartition = this.consumer.TotalCount;
            for (int i = 0; i < this.MailBoxPartition; i++)
            {
                this.aggRootChanges.TryAdd(i, new BlockingCollection<IDomainEvent>());
            }
        }
        public void Start()
        {
            this.consumer.Start(async (msg, ackKey) =>
            {
                Message<IDomainEvent> eventMsg = msg as Message<IDomainEvent>;
                IDomainEvent domainEvent = eventMsg.Body;
                IAggRoot aggRoot = await this.GetAsync(domainEvent.ToAggRootKey());
                domainEvent.AggRootType = aggRoot.GetType().FullName;
                domainEvent.AggRootId = aggRoot.UniqueId;
                //先将事件的版本+1
                domainEvent.Version = aggRoot.Version + 1;
                ActionResponse<EventResult> result = await this.eventStore.AddAsync(domainEvent);
                if (result.Result == ActionResult.Failed)
                {
                    //TODO 先记录日志，再抛出异常
                    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
                    AppRuntime.ErrorFormat("事件:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                    return;
                }
                //事件已经执行过，则返回
                if (result.ReturnData >= EventResult.Executed)
                {
                    return;
                }
                //先验证事件版本的顺序，通过后，在事件的处理方法中，将聚合根的版本更新为事件的版本(+1后的版本)
                var tResult = await this.acceptChange(aggRoot, domainEvent);
                if (tResult.Result == ActionResult.Success)
                {
                    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Executed);
                    this.consumer.Ack(ackKey);
                }
                else
                {
                    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
                    AppRuntime.ErrorFormat("事件:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                }
            });
        }
        public IAggRoot Get(AggRootKey aggRootKey)
        {
            return this.aggRoots[aggRootKey];
        }
        public Task<IAggRoot> GetAsync(AggRootKey aggRootKey)
        {
            return Task.FromResult<IAggRoot>(this.aggRoots[aggRootKey]);
        }
        public void Add(IAggRoot aggRoot)
        {
            this.aggRoots.TryAdd(aggRoot.ToAggRootKey(), aggRoot);
        }
        public Task AddAsync(IAggRoot aggRoot)
        {
            return Task.Run(() =>
            {
                this.aggRoots.TryAdd(aggRoot.ToAggRootKey(), aggRoot);
            });
        }
        public async Task<ActionResponse> ApplyChange(IDomainEvent domainEvent)
        {
            AggRootKey changeKey = domainEvent.ToAggRootKey();
            int routingKey = changeKey.GetHashCode() % this.MailBoxPartition;
            this.aggRootChanges[routingKey].Add(domainEvent);
            if (!this.aggRoots.ContainsKey(domainEvent.ToAggRootKey()))
            {
                await this.AddAggRoot(domainEvent);
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
        private async Task<ActionResponse> AddAggRoot(IDomainEvent domainEvent)
        {
            var aggRootKey = domainEvent.ToAggRootKey();
            if (!this.aggRoots.ContainsKey(aggRootKey))
            {
                Type aggRootType = Type.GetType(domainEvent.AggRootType);
                //从仓储中获取聚合根
                //Type repositoryType = typeof(IRepository<>).MakeGenericType(aggRootType);
                //var repository = AppRuntime.Resolve(repositoryType);
                //IAggRoot aggRoot = await this.aggRootGetHandler.Invoke(repository);
                //从快照中获取聚合根
                IAggRoot aggRoot = await AppRuntime.Resolve<ISnapshotStore>().GetAsync(aggRootType.Name, domainEvent.AggRootId);
                List<IDomainEvent> domainEvents = await AppRuntime.Resolve<IEventStore>().FindAsync(aggRootType.Name, domainEvent.AggRootId, aggRoot.Version + 1);
                IEventSourcing eventSourcingAggRoot = aggRoot as IEventSourcing;
                await eventSourcingAggRoot.ReplayFrom(domainEvents);
                await this.AddAsync(aggRoot);
            }
            return ActionResponse.Success;
        }
    }

}
