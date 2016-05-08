using Basf.Domain.Command;
using System;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface ICommandContext
    {
        void Execute(ICommand command);
        Task ExecuteAsync(ICommand command);
        void AddHandler(Type handlerType, Type commandType);
    }
}
