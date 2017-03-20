using System.Threading.Tasks;

namespace Basf.Messages
{
    public interface IProducer
    {
        void Initialize(int poolSize);
        string RoutingStrategy(IMessage message);
        void Publish(IMessage message);
        Task PublishAsync(IMessage message);
    }
}
