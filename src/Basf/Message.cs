using System;

namespace Basf
{
    public class Message : IMessage
    {
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public virtual string RoutingKey { get; set; }
        public Message()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
    }
}
