using System;

namespace Basf.Domain.Command
{
    public interface ICommand
    {
        string UniqueId { get; }
        DateTime Timestamp { get; }
    }
}
