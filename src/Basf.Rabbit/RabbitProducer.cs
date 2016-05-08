using System;
using System.Threading.Tasks;
using Basf.Message;

namespace Basf.Rabbit
{
    public class RabbitProducer : IProducer
    {
        public void Initialize(int poolSize)
        {
            throw new NotImplementedException();
        }
        public void Publish(IMessage message)
        {
            throw new NotImplementedException();
        }
        public Task PublishAsync(IMessage message)
        {
            throw new NotImplementedException();
        }
        public string RoutingStrategy(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
