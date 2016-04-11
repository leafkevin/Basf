using System;

namespace Basf.Domain.Command
{
    public abstract class Command : ICommand
    {
        public string CommandType { get; private set; }
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string RoutingKey { get; set; }
        public Command()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
        public Command(string commandType)
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
            this.CommandType = commandType;
        }
    }
}
