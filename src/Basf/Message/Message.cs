using System;

namespace Basf
{
    public class Message : IMessage
    {
        public string MessageType { get; private set; }
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public virtual string RoutingKey { get; set; }
        public Message()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
    }
    public class Message<T> : IMessage<T>
    {
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public virtual string RoutingKey { get; set; }
        public T Body { get; set; }
        public Message(T body = default(T)) : base()
        {
            this.Body = body;
        }
    }
}
