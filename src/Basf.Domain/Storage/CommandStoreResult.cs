using Basf.Domain.Command;
using System;

namespace Basf.Domain.Storage
{
    public class CommandStoreResult
    {
        public string CommandType { get; set; }
        public string CommandId { get; set; }
        public CommandResult Result { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
        public string Detail { get; set; }
        public CommandStoreResult(ICommand command, CommandResult result = CommandResult.Stored, string detail = null)
        {
            this.CommandType = command.GetType().FullName;
            this.CommandId = command.UniqueId;
            this.Result = result;
            this.Detail = detail;
            this.CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.UpdateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
