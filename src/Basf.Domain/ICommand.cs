namespace Basf.Domain
{
    public interface ICommand : IMessage
    {
        string AggRootId { get; }
    }
}
