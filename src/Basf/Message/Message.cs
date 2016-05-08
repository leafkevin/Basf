using System;

namespace Basf.Message
{
    public class Message<T> : IMessage<T>
    {
        public string RoutingKey { get; set; }
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public T Body { get; set; }
        public Message(T body = default(T))
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
            this.Body = body;
        }
        public override int GetHashCode()
        {
            return this.UniqueId.GetHashCode();
        }
    }
}
