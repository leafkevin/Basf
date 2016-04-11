using Basf.Data;
using System;

namespace Basf
{
    public interface IPromise
    {
        IPromise Notify(Action<ActionResponse> nextAction);
        IPromise Resolve(Action<ActionResponse> nextAction);
        void Reject(Action<ActionResponse> nextAction);
    }
}
