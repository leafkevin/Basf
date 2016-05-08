using System;

namespace Basf.Domain.Command
{
    public abstract class AbstractCommand : ICommand
    {
        public string UniqueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public AbstractCommand()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
    }
}
