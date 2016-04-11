namespace Basf.Domain.Event
{
    public interface IEventBus
    {
        void Send<TAggRootId>(IDomainEvent<TAggRootId> domainEvent);
        void Send<TEvent, TAggRootId>(TEvent domainEvent) where TEvent : class, IDomainEvent<TAggRootId>;
    }
}
