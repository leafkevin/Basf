using Basf.Domain.Event;
using DomainTest.Domain.ValueObject;
using System;

namespace DomainTest.Domain.Event
{
    public class TransactionStateUpdated : DomainEvent<int>
    {
        public int TransactionId { get; set; }
        public TransactionStatus Status { get; set; }
        public string UpdateAt { get; private set; }
        public TransactionStateUpdated(string commandId, int transactionId, TransactionStatus status)
            : base(commandId)
        {
            this.TransactionId = transactionId;
            this.Status = status;
            this.UpdateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
