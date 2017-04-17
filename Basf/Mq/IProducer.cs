namespace Basf.Mq
{
    public interface IProducer
    {
        void Publish(string exchange, string routingKey, Message msg);
    }
}
