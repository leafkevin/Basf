using Basf.Data;
using Basf.Domain.Command;
using Basf.Domain.Storage;
using Basf.Message;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public class CommandContext : ICommandContext
    {
        private ConcurrentDictionary<Type, Func<object, ICommand, Task<ActionResponse>>> commandHandlers = new ConcurrentDictionary<Type, Func<object, ICommand, Task<ActionResponse>>>();
        private IProducer producer = null;
        private IConsumer consumer = null;
        private ICommandStore commandStore = null;
        public CommandContext(IProducer producer, IConsumer consumer, ICommandStore commandStore)
        {
            this.producer = producer;
            this.consumer = consumer;
            this.commandStore = commandStore;
        }
        public void Initialize(Action<IProducer> producerInitializer, Action<IConsumer> consumerInitializer)
        {
            producerInitializer?.Invoke(this.producer);
            consumerInitializer?.Invoke(this.consumer);
        }
        public void Start()
        {
            this.consumer.Start(async (msg, ackKey) =>
            {
                Message<ICommand> commandMsg = msg as Message<ICommand>;
                ICommand command = commandMsg.Body;
                var result = await this.commandStore.AddAsync(command);
                if (result.Result == ActionResult.Failed)
                {
                    //TODO 先记录日志，再抛出异常
                    await this.commandStore.UpdateResultAsync(command, CommandResult.Error);
                    AppRuntime.ErrorFormat("命令:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                    return;
                }
                //命令已经执行过，则返回
                if (result.ReturnData >= CommandResult.Executed)
                {
                    return;
                }
                var tResult = await this.ExecuteCommand(command);
                if (tResult.Result == ActionResult.Success)
                {
                    await this.commandStore.UpdateResultAsync(command, CommandResult.Executed);
                    this.consumer.Ack(ackKey);
                }
                else
                {
                    await this.commandStore.UpdateResultAsync(command, CommandResult.Error);
                    AppRuntime.ErrorFormat("命令:{0}存储异常,异常消息:{1}，异常明细:{2}", result.Message, result.Detail);
                }
            });
        }
        public void Execute(ICommand command)
        {
            this.producer.Publish(new Message<ICommand>(command));
        }
        public async Task ExecuteAsync(ICommand command)
        {
            await this.producer.PublishAsync(new Message<ICommand>(command));
        }
        public void AddHandler(Type handlerType, Type commandType)
        {
            var commandHandler = HandlerFactory.CreateFuncHandler<object, ICommand, Task<ActionResponse>>("Execute",
                 BindingFlags.Instance | BindingFlags.Public, handlerType, commandType);
            this.commandHandlers.TryAdd(commandType, commandHandler);
        }
        private async Task<ActionResponse> ExecuteCommand(ICommand command)
        {
            Type commandType = command.GetType();
            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = AppRuntime.Resolve(handlerType);
            var commandHandler = this.commandHandlers.GetOrAdd(commandType,
                HandlerFactory.CreateFuncHandler<object, ICommand, Task<ActionResponse>>("Execute",
                BindingFlags.Instance | BindingFlags.Public, handlerType, commandType));
            return await commandHandler.Invoke(handler, command);
        }
    }
}
