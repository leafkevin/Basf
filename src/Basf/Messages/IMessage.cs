using System;

namespace Basf.Messages
{
    public interface IMessage
    {
        string CommandId { get; }
        DateTime Timestamp { get; }
        string Topic { get; }
    }
    public interface IMessage<T> : IMessage
    {
        T Body { get; set; }
    }
}
