using Basf.Domain.Event;

namespace DomainTest.Domain.Event
{
    public class BankAccountAccepted : DomainEvent<int>
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public BankAccountAccepted(string commandId, int transactionId, int accountId, decimal amount)
            : base(commandId)
        {
            this.TransactionId = transactionId;
            this.AccountId = accountId;
            this.Amount = amount;
        }
    }
}
