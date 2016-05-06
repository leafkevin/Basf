using System;

namespace Basf.Message
{
    public interface IConsumer
    {
        int ConsumerCount { get; }
        void Subscribe(string routingKey, int nCount = 1);
        void Start(Action<IMessage> executeHander);
    }
}
