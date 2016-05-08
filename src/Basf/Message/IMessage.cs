using System;

namespace Basf.Message
{
    public interface IMessage
    {
        string RoutingKey { get; set; }
        string UniqueId { get; }
        DateTime Timestamp { get; }
    }
    public interface IMessage<T> : IMessage
    {
        T Body { get; set; }
    }
}
