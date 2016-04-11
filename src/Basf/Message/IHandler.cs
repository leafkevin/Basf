namespace Basf
{
    public interface IHandler<T> : IHandler where T : IMessage
    {
        void Handle(T message);
    }
    public interface IHandler
    {
        void Handle(IMessage message);
    }
}
