namespace Basf.Domain
{
    public interface IAggRootEventHandler<TKey>
    {
        void HandleEvent<TDomainEvent>(IAggRootState<TKey> state, TDomainEvent domainEvent) where TDomainEvent : IDomainEvent<TKey>;
    }
}
