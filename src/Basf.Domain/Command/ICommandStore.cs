using System.Threading.Tasks;

namespace Basf.Domain.Command
{
    public interface ICommandStore
    {
        void Configure<TCommand>(string commandType) where TCommand : ICommand;
        //void Add(params ICommand[] commands);
        //Task AddAsync(params ICommand[] commands);
        void Add<TCommand>(params TCommand[] commands) where TCommand : ICommand;
        Task AddAsync<TCommand>(params TCommand[] commands) where TCommand : ICommand;
        //ICommand Get(string commandId);
        //Task<ICommand> GetAsync(string commandId);
        TCommand Get<TCommand>(string commandId) where TCommand : ICommand;
        Task<TCommand> GetAsync<TCommand>(string commandId) where TCommand : ICommand;
    }
}
