using Basf.Domain.Event;

namespace DomainTest.Domain.Event
{
    public class BankAccountUnlocked : DomainEvent<int>
    {
        public int AccountId { get; set; }
        public bool IsLocked { get; private set; }
        public BankAccountUnlocked(string commandId, int accountId)
            : base(commandId)
        {
            this.AccountId = accountId;
            this.IsLocked = false;
        }
    }
}
