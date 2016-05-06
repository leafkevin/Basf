using Basf.Domain.Command;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ICommandStore
    {
        CommandResult Add(params ICommand[] commands);
        Task<CommandResult> AddAsync(params ICommand[] commands);
        void UpdateResult(ICommand commmand, CommandResult result);
        Task UpdateResultAsync(ICommand commmand, CommandResult result);
        TCommand Get<TCommand>(string commandId) where TCommand : class, ICommand;
        Task<TCommand> GetAsync<TCommand>(string commandId) where TCommand : class, ICommand;
    }
}
