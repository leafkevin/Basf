using Basf.Domain;
using Basf.Domain.Event;
using ConsoleTest.Domain.Event;
using ConsoleTest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleTest.Domain.Model
{
    public class Order : AggRoot<int>
    {
        //订单信息
        public string OrderNo { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; }
        public string Remark { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        //用户信息
        public Consignee Buyer { get; set; }
        //商品明细
        public List<Goods> Commoditys { get; set; }

        public Order(int orderId) : base(orderId)
        {
        }
        public async Task AddCommodity(string commandId, Goods commodity)
        {
            await this.ApplyChange(new OrderCommodityAdded(commandId, this.UniqueId, commodity));
        }
        public void Handle(OrderCommodityAdded domainEvent)
        {
            this.Commoditys.Add(domainEvent.Commodity);
            this.Amount += domainEvent.Commodity.Price * domainEvent.Commodity.Quantity;
        }
    }
}
