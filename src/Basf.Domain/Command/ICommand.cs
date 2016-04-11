namespace Basf.Domain.Command
{
    public interface ICommand : IMessage
    {
        string CommandType { get; }
    }
}
