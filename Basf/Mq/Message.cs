using System;
using System.Collections.Generic;

namespace Basf.Mq
{
    public class Message<TBody>
    {
        public string CommandId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string MessageType { get; set; }
        public Dictionary<string, object> Context { get; set; }
        public TBody Body { get; set; }
    }
    public class Message : Message<byte[]>
    {
    }
}
