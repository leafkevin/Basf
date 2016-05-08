using Basf.Data;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class HandlerFactory
    {
        public static Action<TOwner, TArgs> CreateActionHandler<TOwner, TArgs>(string methodName, BindingFlags bindingFlags, Type ownerType, Type argsType)
        {
            MethodInfo methodInfo = ownerType.GetMethod(methodName, bindingFlags, Type.DefaultBinder, new Type[] { typeof(TArgs) }, null);
            var dm = new DynamicMethod(argsType.Name + "Execute", typeof(void), new Type[] { typeof(TOwner), typeof(TArgs) }, true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, ownerType);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, argsType);
            il.EmitCall(OpCodes.Call, methodInfo, null);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Action<TOwner, TArgs>)) as Action<TOwner, TArgs>;
        }
        public static Func<TOwner, TArgs, TResult> CreateFuncHandler<TOwner, TArgs, TResult>(string methodName, BindingFlags bindingFlags, Type ownerType, Type argsType)
        {
            MethodInfo methodInfo = ownerType.GetMethod(methodName, bindingFlags, Type.DefaultBinder, new Type[] { typeof(TArgs) }, null);
            var dm = new DynamicMethod(argsType.Name + "Execute", typeof(TResult), new Type[] { typeof(TOwner), typeof(TArgs) }, true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, ownerType);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, argsType);
            il.EmitCall(OpCodes.Call, methodInfo, null);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Func<TOwner, TArgs, TResult>)) as Func<TOwner, TArgs, TResult>;
        }
        public static Func<object, TResult> CreateFuncHandler<TResult>(string methodName, BindingFlags bindingFlags,Type ownerType)
        {
            MethodInfo methodInfo = ownerType.GetMethod(methodName, bindingFlags, Type.DefaultBinder, new Type[] { typeof(void) }, null);
            var dm = new DynamicMethod(methodName + "Execute", typeof(TResult), new Type[] { typeof(object) }, true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, ownerType);
            il.EmitCall(OpCodes.Call, methodInfo, null);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Func<object, TResult>)) as Func<object, TResult>;
        }
    }
}
