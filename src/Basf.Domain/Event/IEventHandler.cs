using Basf.Data;
using System.Threading.Tasks;

namespace Basf.Domain.Event
{
    public interface IEventHandler<TEvent> where TEvent : class, IDomainEvent
    {
        Task<ActionResponse> Handle(TEvent domainEvent);
    }
}
