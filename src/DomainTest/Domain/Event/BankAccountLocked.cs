using Basf.Domain.Event;

namespace DomainTest.Domain.Event
{
    public class BankAccountLocked : DomainEvent<int>
    {
        public int AccountId { get; set; }
        public bool IsLocked { get; private set; }
        public BankAccountLocked(string commandId, int accountId)
            : base(commandId)
        {
            this.AccountId = accountId;
            this.IsLocked = true;
        }
    }
}
