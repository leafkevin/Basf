using Basf.Messages;
using System;

namespace Basf.Rabbitmq
{
    public class RabbitConsumer //: IConsumer
    {
        public int TotalCount
        {
            get { throw new NotImplementedException(); }
        }
        public void Ack(object ackKey)
        {
            throw new NotImplementedException();
        }
        public void Start(Action<IMessage, object> executeHander)
        {
            throw new NotImplementedException();
        }
        public void Subscribe(string routingKey)
        {
            throw new NotImplementedException();
        }
    }
}
