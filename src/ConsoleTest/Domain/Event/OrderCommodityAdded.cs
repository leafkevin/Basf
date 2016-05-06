using Basf.Domain.Event;
using ConsoleTest.Domain.ValueObject;

namespace ConsoleTest.Domain.Event
{
    public class OrderCommodityAdded : DomainEvent<int>
    {
        public int OrderId { get; private set; }
        public Goods Commodity { get; private set; }

        public OrderCommodityAdded(string commandId, int orderId, Goods commodity)
            : base(commandId)
        {
            this.OrderId = orderId;
            this.Commodity = commodity;
        }
    }
}
