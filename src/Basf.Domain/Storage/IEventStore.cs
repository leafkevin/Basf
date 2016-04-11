using Basf.Domain.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface IEventStore
    {
        void Add<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>;
        Task AddAsync<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>;
        List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>;
        Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>;
    }
}
