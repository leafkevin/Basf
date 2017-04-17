using System;
using System.Threading.Tasks;

namespace Basf.Mq
{
    public interface IMessageHandler
    {
        string RoutingKey { get; }
        Task Execute(Message msg);
        string AddProcesser(string msgType, Func<object, Task> msgProcesser);
    }
    public interface IMessageHandler<TMsgBody>
    {
        string RoutingKey { get; }
        Task Execute(Message msg);
        string AddProcesser(string msgType, Func<TMsgBody, Task> msgProcesser);
    }
}
