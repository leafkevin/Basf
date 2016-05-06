using Basf.Domain.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface IEventStore
    {
        EventResult Add(params IDomainEvent[] domainEvents);
        Task<EventResult> AddAsync(params IDomainEvent[] domainEvents);
        void UpdateResult(IDomainEvent domainEvent, EventResult result);
        Task UpdateResultAsync(IDomainEvent domainEvent, EventResult result);
        List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>;
        Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>;
    }
}
