using Basf.Domain.Event;

namespace Basf.Domain
{
    public static class Extensions
    {
        public static AggRootKey ToAggRootKey(this IDomainEvent domainEvent)
        {
            return new AggRootKey(domainEvent.AggRootType, domainEvent.AggRootId);
        }
        public static AggRootKey ToAggRootKey(this IAggRoot aggRoot)
        {
            return new AggRootKey(aggRoot.GetType().FullName, aggRoot.UniqueId.ToString());
        }
    }
}
