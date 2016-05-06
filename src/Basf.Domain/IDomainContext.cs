using Basf.Domain.Event;
using Basf.Message;
using System;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface IDomainContext
    {
        void Initialize(Action<IProducer> eventProducerInitializer, Action<IConsumer> eventConsumerInitializer);
        void Start();
        IAggRoot Get(AggRootKey aggRootKey);
        Task<IAggRoot> GetAsync(AggRootKey aggRootKey);
        void Add(IAggRoot aggRoot);
        Task AddAsync(IAggRoot aggRoot);
        void Apply(IDomainEvent domainEvent);
        Task ApplyAsync(IDomainEvent domainEvent);
        void Publish(IDomainEvent domainEvent);
        Task PublishAsync(IDomainEvent domainEvent);
        Action<IAggRoot, IDomainEvent> GetEventHandler(Type eventType);
        void AddEventHandler(Type aggRootType, Type eventType);
    }
}
