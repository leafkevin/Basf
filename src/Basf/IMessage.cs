using System;

namespace Basf
{
    public interface IMessage : IEntity<string>
    {
        DateTime Timestamp { get; }
        string RoutingKey { get; set; }
    }
}
