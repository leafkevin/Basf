namespace Basf.Domain.Command
{
    public interface ICommandProcessor
    {
        void Initialize(string routingKey, int consumerTotal);
        void Start();
    }
}
