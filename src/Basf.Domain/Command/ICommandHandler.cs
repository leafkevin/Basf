using System.Threading.Tasks;

namespace Basf.Domain.Command
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        void Execute(TCommand command);
        Task ExecuteAsync(TCommand command);
    }
}
