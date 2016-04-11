using System.Threading.Tasks;

namespace Basf.Domain.Command
{
    public interface ICommandBus
    {
        void Send(ICommand command);
        void Send<TCommand>(TCommand command) where TCommand : class, ICommand;
    }
}
