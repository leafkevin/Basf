using System;

namespace Basf.Message
{
    public class Message : IMessage
    {
        public string MessageType { get; private set; }
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Message()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
    }
    public class Message<T> : Message, IMessage<T>
    {
        public T Body { get; set; }
        public Message(T body = default(T)) : base()
        {
            this.Body = body;
        }
    }
}
