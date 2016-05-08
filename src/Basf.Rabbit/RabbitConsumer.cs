using Basf.Message;
using System;

namespace Basf.Rabbit
{
    public class RabbitConsumer : IConsumer
    {
        public int TotalCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public void Ack(object ackKey)
        {
            throw new NotImplementedException();
        }
        public void Start(Action<IMessage, object> executeHander)
        {
            throw new NotImplementedException();
        }
        public void Subscribe(string routingKey, int nTotalCount = 1)
        {
            throw new NotImplementedException();
        }
    }
}
