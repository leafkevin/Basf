using Basf.Domain.Storage;
using Basf.Message;
using System;
using System.Reflection;

namespace Basf.Domain.Command
{
    public class CommandProcessor : ICommandProcessor
    {
        private ICommandContext commandContext = null;
        private IConsumer commandConsumer = null;
        private ICommandStore commandStore = null;
        public int ConsumerTotal { get; set; } = 15;
        public string RoutingKey { get; set; }
        public CommandProcessor(ICommandContext commandContext, IConsumer commandConsumer, ICommandStore commandStore)
        {
            this.commandContext = commandContext;
            this.commandConsumer = commandConsumer;
            this.commandStore = commandStore;
        }
        public void Initialize(string routingKey, int consumerTotal)
        {
            this.RoutingKey = routingKey;
            this.ConsumerTotal = consumerTotal;
            this.commandConsumer.Subscribe(routingKey, consumerTotal);
        }
        public void Start()
        {
            this.commandConsumer.Start(async msg =>
            {
                ICommand command = msg as ICommand;
                CommandResult result = await this.commandStore.AddAsync(command);
                //判断是否已经持久化，已持久化就跳过
                if (result >= CommandResult.Executed)
                {
                    return;
                }
                if (result == CommandResult.Error)
                {
                    //TODO                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                    await this.commandStore.UpdateResultAsync(command, CommandResult.Error);
                    //先记录日志，再抛出异常，理论上在存储里面我不做任何的try catch
                }
                Type commandType = command.GetType();
                Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
                var handler = AppRuntime.Resolve(handlerType);
                //在处理中，聚合根的版本更新为事件的版本
                this.HandleCommand(handler, command);
                await this.commandStore.UpdateResultAsync(command, CommandResult.Executed);
                //TODO:ACK
            });
        }
        private void HandleCommand(object commandHandler, ICommand command)
        {
            Type commandType = command.GetType();
            Type handlerType = commandHandler.GetType();
            var handler = AppRuntime.Resolve<ICommandContext>().GetCommandHandler(commandType);
            if (handler == null)
            {
                handler = HandlerFactory.CreateFuncHandler<object, ICommand>("ExecuteAsync",
                    BindingFlags.Instance | BindingFlags.Public, commandType, handlerType);
                AppRuntime.Resolve<ICommandContext>().AddCommandHandler(handlerType, commandType);
            }
            handler.Invoke(commandHandler, command);
        }
    }
}
