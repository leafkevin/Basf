using System.Threading.Tasks;

namespace Basf.Domain.Event
{
    public interface IEventHandler<TEvent> where TEvent : class, IDomainEvent
    {
        void Handle(TEvent domainEvent);
        Task HandleAsync(TEvent domainEvent);
    }
}
