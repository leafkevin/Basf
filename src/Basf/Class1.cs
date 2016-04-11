using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basf
{
    public interface IMessageHandlerProvider
    {
        /// <summary>Get all the handlers for the given message type.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        //IEnumerable<MessageHandlerData<IMessageHandlerProxy1>> GetHandlers(Type messageType);
    }
}
