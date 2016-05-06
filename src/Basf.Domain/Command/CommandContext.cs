using Basf.Message;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Basf.Domain.Command
{
    public class CommandContext : ICommandContext
    {
        private ConcurrentDictionary<Type, Func<object, ICommand, Task>> commandHandlers = new ConcurrentDictionary<Type, Func<object, ICommand, Task>>();
        private IProducer producer = null;
        public int ProducerTotal { get; set; } = 15;
        public CommandContext(IProducer producer)
        {
            this.producer = producer;
        }
        public void Initialize(int nProducerTotal)
        {
            this.ProducerTotal = nProducerTotal;
            this.producer.Initialize(nProducerTotal);
        }
        public void Execute(ICommand command)
        {
            this.producer.Publish(command);
        }
        public async Task ExecuteAsync(ICommand command)
        {
            await this.producer.PublishAsync(command);
        }
        public Func<object, ICommand, Task> GetCommandHandler(Type commandType)
        {
            return this.commandHandlers[commandType];
        }
        public void AddCommandHandler(Type handlerType, Type commandType)
        {
            var commandHandler = HandlerFactory.CreateFuncHandler<object, ICommand>("ExecuteAsync",
                 BindingFlags.Instance | BindingFlags.Public, commandType, handlerType);
            this.commandHandlers.TryAdd(commandType, commandHandler);
        }
    }
}
