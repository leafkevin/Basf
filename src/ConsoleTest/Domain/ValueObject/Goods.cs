using System;

namespace ConsoleTest.Domain.ValueObject
{
    public class Goods
    {
        public int GoodsId { get; set; }
        public string GoodsNo { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }
        public Goods(int goodsId, string goodsNo, decimal price, int quantity)
        {
            this.GoodsId = goodsId;
            this.GoodsNo = goodsNo;
            this.Price = price;
            this.Quantity = quantity;
            this.CreateAt = DateTime.Now;
        }
    }
}
