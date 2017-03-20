using Basf.Data;
using System;
using System.Threading.Tasks;

namespace Basf.Messages
{
    public interface IConsumer
    {
        void Subscribe(string routingKey);
        void Start();
        void Ack(object ackKey);
    }
}
