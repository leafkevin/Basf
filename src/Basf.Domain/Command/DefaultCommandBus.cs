using System;

namespace Basf.Domain.Command
{
    public class DefaultCommandBus : ICommandBus
    {
        public DefaultCommandBus()
        {
        }
        public void Send(ICommand command)
        {
            try
            {
                Type type = typeof(ICommandHandler<>).MakeGenericType(command.GetType().GenericTypeArguments[0]);
                ICommandHandler handler = AppRuntime.Resolve(type) as ICommandHandler;
                handler.Execute(command);
            }
            catch (Exception ex)
            {
                AppRuntime.ErrorFormat("命令{0}执行失败。Exception:{1}", command, ex.ToString());
            }
        }
        public void Send<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            try
            {
                AppRuntime.Resolve<ICommandHandler<TCommand>>().Execute(command);
            }
            catch (Exception ex)
            {
                AppRuntime.ErrorFormat("命令{0}执行失败。Exception:{1}", command, ex.ToString());
            }
        }        
    }
}
