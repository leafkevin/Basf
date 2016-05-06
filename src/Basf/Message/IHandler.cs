namespace Basf.Message
{
    public interface IHandler
    {
        void Handle(IMessage message);
    }
    public interface IHandler<T> : IHandler where T : IMessage
    {
        void Handle(T message);
    }   
}
