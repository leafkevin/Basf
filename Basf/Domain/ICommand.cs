using System;

namespace Basf.Domain
{
    public interface ICommand
    {
        string UniqueId { get; }
        DateTime Timestamp { get; }
    }
}
