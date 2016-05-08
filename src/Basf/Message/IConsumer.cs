using System;

namespace Basf.Message
{
    public interface IConsumer
    {
        int TotalCount { get; }
        void Subscribe(string routingKey, int nTotalCount = 1);
        void Start(Action<IMessage, object> executeHander);
        void Ack(object ackKey);
    }
}
