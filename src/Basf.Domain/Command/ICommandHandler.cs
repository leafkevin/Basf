using Basf.Data;
using System.Threading.Tasks;

namespace Basf.Domain.Command
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        Task<ActionResponse> Execute(TCommand command);
    }
}
