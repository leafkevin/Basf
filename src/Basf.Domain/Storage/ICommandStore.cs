using Basf.Data;
using Basf.Domain.Command;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ICommandStore
    {
        ActionResponse<CommandResult> Add(ICommand command);
        Task<ActionResponse<CommandResult>> AddAsync(ICommand command);      
        ICommand Get(string commandTypeName, string commandId);
        Task<ICommand> GetAsync(string commandTypeName, string commandId);
        List<ICommand> Find(string commandTypeName, CommandResult result);
        Task<List<ICommand>> FindAsync(string commandTypeName, CommandResult result);
        void UpdateResult(ICommand commmand, CommandResult result);
        Task UpdateResultAsync(ICommand commmand, CommandResult result);
    }
}
