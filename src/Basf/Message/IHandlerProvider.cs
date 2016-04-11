using System;
using System.Collections.Generic;

namespace Basf
{
    public interface IHandlerProvider
    {
        IEnumerable<IHandler> GetHandlers(Type messageType);
    }
}
