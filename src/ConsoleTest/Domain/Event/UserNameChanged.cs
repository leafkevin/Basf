using Basf.Domain.Event;

namespace ConsoleTest.Domain.Event
{
    public class UserNameChanged : DomainEvent<int>
    {
        public string UserName { get; set; }
        public UserNameChanged(string commandId, string userName)
            : base(commandId)
        {
            this.UserName = userName;
        }
    }
}
