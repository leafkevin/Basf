using System;
using System.Threading.Tasks;

namespace Basf.Mq
{
    public interface IConsumer
    {
        void Subscribe(string routingKey);
        Task Execute(Message msg);
        void AddHandler(string msgType, Func<object, Task> handler);
    }
}
