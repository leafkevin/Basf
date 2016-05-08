using Basf.Domain.Event;

namespace DomainTest.Domain.Event
{
    public class BankAccountAmountFrozen : DomainEvent<int>
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public BankAccountAmountFrozen(string commandId, int accountId, decimal amount)
            : base(commandId)
        {
            this.AccountId = accountId;
            this.Amount = amount;
        }
    }
}
