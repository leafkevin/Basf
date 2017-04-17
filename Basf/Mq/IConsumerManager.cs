using System;

namespace Basf.Mq
{
    public interface IConsumerManager
    {
        void Start();
        void RegisterConsumer(string routingKey, Type consumerType);
    }
}
