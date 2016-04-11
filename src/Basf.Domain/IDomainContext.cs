using Basf.Data;
using Basf.Domain.Command;
using System;

namespace Basf.Domain
{
    public interface IDomainContext
    {
        IDomainContext Add(string actionId, Action action);
        void FlowTo(string actionId, string resolveActionId, string rejectActionId);
    }
}
