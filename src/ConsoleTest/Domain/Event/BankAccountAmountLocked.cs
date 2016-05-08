using Basf.Domain.Event;

namespace ConsoleTest.Domain.Event
{
    public class BankAccountAmountLocked : DomainEvent<int>
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public BankAccountAmountLocked(string commandId, int accountId, decimal amount)
            : base(commandId)
        {
            this.AccountId = accountId;
            this.Amount = amount;
        }
    }
}
