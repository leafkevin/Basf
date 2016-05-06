using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class HandlerFactory
    {
        public static Action<THandler, TMessage> CreateActionHandler<THandler, TMessage>(string methodName, BindingFlags bindingFlags, Type messageType, Type handlerType)
        {
            MethodInfo methodInfo = handlerType.GetMethod(methodName, bindingFlags, Type.DefaultBinder, new Type[] { typeof(TMessage) }, null);
            var dm = new DynamicMethod(messageType.Name + "Execute", typeof(void), new Type[] { typeof(TMessage), typeof(THandler) }, true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, handlerType);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, messageType);
            il.EmitCall(OpCodes.Call, methodInfo, null);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Action<THandler, TMessage>)) as Action<THandler, TMessage>;
        }
        public static Func<THandler, TMessage, Task> CreateFuncHandler<THandler, TMessage>(string methodName, BindingFlags bindingFlags, Type messageType, Type handlerType)
        {
            MethodInfo methodInfo = handlerType.GetMethod(methodName, bindingFlags, Type.DefaultBinder, new Type[] { typeof(TMessage) }, null);
            var dm = new DynamicMethod(messageType.Name + "Execute", typeof(void), new Type[] { typeof(TMessage), typeof(THandler) }, true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, handlerType);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, messageType);
            il.EmitCall(OpCodes.Call, methodInfo, null);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Func<THandler, TMessage, Task>)) as Func<THandler, TMessage, Task>;
        }
    }
}
