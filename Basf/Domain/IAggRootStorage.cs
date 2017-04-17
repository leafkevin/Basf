using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface IAggRootStorage<TAggRootState, TKey> where TAggRootState : IAggRootState<TKey>
    {
        Task<TEvent> Get<TEvent>(TKey aggRootId, int version) where TEvent : IDomainEvent<TKey>;
        Task<bool> Add<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent<TKey>;
        Task<List<IDomainEvent<TKey>>> Find(TKey aggRootId, int startVersion);
        Task<List<TEvent>> Find<TEvent>(TKey aggRootId, string eventType, int startVersion) where TEvent : IDomainEvent<TKey>;
        Task<List<IDomainEvent<TKey>>> Find(string commandId);
        Task<List<TEvent>> Find<TEvent>(string commandId, string eventType) where TEvent : IDomainEvent<TKey>;
        Task Complete<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent<TKey>;
        Task<TAggRootState> GetSnapshot(TKey aggRootId);
        Task SaveSnapshot(TAggRootState state);
    }
}
