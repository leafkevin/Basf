using System;

namespace Basf
{
    public interface IMessage : IEntity<string>
    {
        DateTime Timestamp { get; }
        string RoutingKey { get; set; }
    }
    public interface IMessage<T> : IMessage
    {
        T Body { get; set; }
    }
}
