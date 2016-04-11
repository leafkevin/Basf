using Basf.Data;
using Basf.Domain.Event;

namespace Basf.Domain
{
    public interface IDomainActor<TAggRootId>
    {
        void Resolve(IDomainEvent<TAggRootId> domainEvent, ActionResponse result);
        void Reject(IDomainEvent<TAggRootId> domainEvent, ActionResponse result);
    }
}
