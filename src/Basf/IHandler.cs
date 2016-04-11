namespace Basf
{
    public interface IHandler<T> where T : IMessage
    {
        void Handle(T message);
    }
    public interface IHandler : IHandler<IMessage>
    {
    }
}
