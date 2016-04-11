namespace Basf.Domain.Command
{
    public interface ICommandHandler : ICommand
    {
        void Execute(ICommand command);
    }
    public interface ICommandHandler<TCommand> : ICommandHandler where TCommand : class, ICommand
    {
        void Execute(TCommand command);
    }
}
