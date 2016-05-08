using Basf.Domain.Event;
using DomainTest.Domain.ValueObject;
using System;

namespace DomainTest.Domain.Event
{
    public class TransactionStarted : DomainEvent<int>
    {
        public int TransactionId { get; set; }
        public int TransferAccountId { get; set; }
        public int AcceptAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public string CreateAt { get; private set; }
        public string UpdateAt { get; private set; }
        public TransactionStarted(string commandId, int transactionId, int transferAccountId, int acceptAccountId, decimal amount)
            : base(commandId)
        {
            this.TransactionId = transactionId;
            this.TransferAccountId = transferAccountId;
            this.AcceptAccountId = acceptAccountId;
            this.Amount = amount;
            this.Status = TransactionStatus.Created;
            this.CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.UpdateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        }
    }
}
