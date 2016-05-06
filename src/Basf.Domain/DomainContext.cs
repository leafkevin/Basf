using Basf.Domain.Event;
using Basf.Domain.Storage;
using Basf.Message;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class DomainContext : IDomainContext
    {
        private ConcurrentDictionary<AggRootKey, IAggRoot> aggRoots = new ConcurrentDictionary<AggRootKey, IAggRoot>();
        private ConcurrentDictionary<Type, Action<IAggRoot, IDomainEvent>> eventHandlers = new ConcurrentDictionary<Type, Action<IAggRoot, IDomainEvent>>();
        private ConcurrentDictionary<int, BlockingCollection<IDomainEvent>> aggRootChanges = new ConcurrentDictionary<int, BlockingCollection<IDomainEvent>>();
        private IProducer eventProducer = null;
        private IDomainContext domainContext = null;
        private IConsumer eventConsumer = null;
        private IEventStore eventStore = null;
        private Func<IAggRoot, IDomainEvent, Task> acceptChange = null;
        public int MailBoxPartition { get; private set; }
        public DomainContext(IProducer eventProducer, IConsumer eventConsumer, IEventStore eventStore)
        {
            this.eventProducer = eventProducer;
            this.eventConsumer = eventConsumer;
            this.eventStore = eventStore;
        }
        public void Initialize(Action<IProducer> eventProducerInitializer, Action<IConsumer> eventConsumerInitializer)
        {
            this.acceptChange = HandlerFactory.CreateFuncHandler<IAggRoot, IDomainEvent>("AcceptChange",
                BindingFlags.Instance | BindingFlags.NonPublic, typeof(IDomainEvent), typeof(AggRoot));
            eventProducerInitializer?.Invoke(this.eventProducer);
            eventConsumerInitializer?.Invoke(this.eventConsumer);
            this.MailBoxPartition = this.eventConsumer.ConsumerCount;
            for (int i = 0; i < this.MailBoxPartition; i++)
            {
                this.aggRootChanges.TryAdd(i, new BlockingCollection<IDomainEvent>());
            }
        }
        public void Start()
        {
            this.eventConsumer.Start(async msg =>
            {
                IDomainEvent domainEvent = msg as IDomainEvent;
                IAggRoot aggRoot = await this.domainContext.GetAsync(domainEvent.ToAggRootKey());
                domainEvent.AggRootType = aggRoot.GetType().FullName;
                domainEvent.AggRootId = aggRoot.UniqueId;
                //先将事件的版本+1
                domainEvent.Version = aggRoot.Version + 1;
                EventResult result = await this.eventStore.AddAsync(domainEvent);
                if (result == EventResult.Error)
                {
                    //TODO
                    await this.eventStore.UpdateResultAsync(domainEvent, EventResult.Error);
                    //先记录日志，再抛出异常，理论上在存储里面我不做任何的try catch
                }
                //事件已经执行过，则返回
                else if (result >= EventResult.Executed)
                {
                    return;
                }
                //先验证事件版本的顺序，通过后，在事件的处理方法中，将聚合根的版本更新为事件的版本(+1后的版本)
                await this.acceptChange(aggRoot, domainEvent);
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
            return Task.Run(() => this.aggRoots.TryAdd(aggRoot.ToAggRootKey(), aggRoot));
        }
        public void Apply(IDomainEvent domainEvent)
        {
            AggRootKey changeKey = domainEvent.ToAggRootKey();
            int routingKey = changeKey.GetHashCode() % this.MailBoxPartition;
            this.aggRootChanges[routingKey].Add(domainEvent);
        }
        public Task ApplyAsync(IDomainEvent domainEvent)
        {
            return Task.Run(() =>
            {
                AggRootKey changeKey = domainEvent.ToAggRootKey();
                int routingKey = changeKey.GetHashCode() % this.MailBoxPartition;
                this.aggRootChanges[routingKey].Add(domainEvent);
            });
        }
        public void Publish(IDomainEvent domainEvent)
        {
            this.eventProducer.Publish(domainEvent);
        }
        public async Task PublishAsync(IDomainEvent domainEvent)
        {
            await this.eventProducer.PublishAsync(domainEvent);
        }
        public Action<IAggRoot, IDomainEvent> GetEventHandler(Type eventType)
        {
            return this.eventHandlers[eventType];
        }
        public void AddEventHandler(Type aggRootType, Type eventType)
        {
            var eventHandler = HandlerFactory.CreateActionHandler<IAggRoot, IDomainEvent>("Handle",
                BindingFlags.Instance | BindingFlags.Public, eventType, aggRootType);
            this.eventHandlers.TryAdd(eventType, eventHandler);
        }
    }
}
