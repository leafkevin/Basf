using Basf.Domain.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface IEventSourcing
    {
        Task CreateSnapshot();
        Task ReplayFrom(IEnumerable<IDomainEvent> domainEvents);
    }
}
