using Basf.Domain.Command;
using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ICommandStore
    {
        void Add<TCommand>(params TCommand[] commands) where TCommand : class, ICommand;
        Task AddAsync<TCommand>(params TCommand[] commands) where TCommand : class, ICommand;
        TCommand Get<TCommand>(string commandId) where TCommand : class, ICommand;
        Task<TCommand> GetAsync<TCommand>(string commandId) where TCommand : class, ICommand;
    }
}
