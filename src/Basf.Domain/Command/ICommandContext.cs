using Basf.Domain.Command;
using System;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface ICommandContext
    {
        void Execute(ICommand command);
        Task ExecuteAsync(ICommand command);
        Func<object, ICommand, Task> GetCommandHandler(Type commandType);
        void AddCommandHandler(Type handlerType, Type commandType);
    }
}
