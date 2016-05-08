using Basf.Domain.Event;
using System;

namespace Basf.Domain.Storage
{
    public class EventStoreResult
    {
        public string AggRootType { get; set; }
        public string AggRootId { get; set; }
        public int Version { get; set; }
        public EventResult Result { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
        public string Detail { get; set; }
        public EventStoreResult(IDomainEvent domainEvent, EventResult result = EventResult.Stored, string detail = null)
        {
            this.AggRootType = domainEvent.AggRootType;
            this.AggRootId = domainEvent.AggRootId;
            this.Version = domainEvent.Version;
            this.Result = result;
            this.Detail = detail;
            this.CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.UpdateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
