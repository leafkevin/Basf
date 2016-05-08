using Basf.Data;
using Basf.Domain.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface IEventStore
    {
        ActionResponse<EventResult> Add(IDomainEvent domainEvent);
        Task<ActionResponse<EventResult>> AddAsync(IDomainEvent domainEvent);
        IDomainEvent Get(string aggRootTypeName, string aggRootId, int version);
        Task<IDomainEvent> GetAsync(string aggRootTypeName, string aggRootId, int version);
        List<IDomainEvent> Find(string aggRootTypeName, string aggRootId, int startVersion);
        Task<List<IDomainEvent>> FindAsync(string aggRootTypeName, string aggRootId, int startVersion);
        void UpdateResult(IDomainEvent domainEvent, EventResult result);
        Task UpdateResultAsync(IDomainEvent domainEvent, EventResult result);
    }
}
