using System;
using System.Collections.Generic;

namespace Basf.Messages
{
    public class Message<T>
    {
        public string CommandId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string MessageType { get; set; }
        public Dictionary<string, object> Context { get; set; }
        public T Body { get; set; }
    }
}
