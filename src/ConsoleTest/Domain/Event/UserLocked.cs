using Basf.Domain.Event;

namespace ConsoleTest.Domain.Event
{
    public class UserLocked : DomainEvent<int>
    {
        public int UserId { get; private set; }
        public UserLocked(string commandId, int userId)
            : base(commandId)
        {
            this.UserId = userId;
        }
    }
}
