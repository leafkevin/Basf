using Basf.Domain.Event;

namespace DomainTest.Domain.Event
{
    public class BankAccountWithdrawed : DomainEvent<int>
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public BankAccountWithdrawed(string commandId, int accountId, decimal amount)
            : base(commandId)
        {
            this.AccountId = accountId;
            this.Amount = amount;
        }
    }
}
