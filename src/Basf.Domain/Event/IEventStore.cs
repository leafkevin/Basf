using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain.Event
{
    public interface IEventStore
    {
        void Configure<TEvent, TAggRootId>(string eventType) where TEvent : IDomainEvent<TAggRootId>;
        void Add<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : IDomainEvent<TAggRootId>;
        Task AddAsync<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : IDomainEvent<TAggRootId>;
        List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : IDomainEvent<TAggRootId>;
        Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : IDomainEvent<TAggRootId>;
    }
}
