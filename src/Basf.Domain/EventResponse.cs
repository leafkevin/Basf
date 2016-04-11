using Basf.Data;
using Basf.Domain.Event;
using System;

namespace Basf.Domain
{
    public class EventResponse<TAggRootId>
    {
        public string UniqueId { get; set; }
        public string EventType { get; set; }
        public TAggRootId AggRootId { get; set; }
        public string CommandId { get; set; }
        public string RoutingKey { get; set; }
        public DateTime Timestamp { get; set; }
        public int Version { get; set; }
        public ActionResponse ActionResult { get; set; }
        private EventResponse(IDomainEvent<TAggRootId> domainEvent, ActionResponse actionResult)
        {
            this.UniqueId = domainEvent.UniqueId;
            this.EventType = domainEvent.EventType;
            this.AggRootId = domainEvent.AggRootId;
            this.CommandId = domainEvent.CommandId;
            //此处的路由应该是commandId
            this.RoutingKey = domainEvent.CommandId;
            this.Timestamp = domainEvent.Timestamp;
            this.Version = domainEvent.Version;
            this.ActionResult = actionResult;
        }
        public static EventResponse<TAggRootId> Resolve(IDomainEvent<TAggRootId> domainEvent)
        {
            return new EventResponse<TAggRootId>(domainEvent, ActionResponse.Success(PromiseResult.Resolved));
        }
        public static EventResponse<TAggRootId> Resolve<T>(IDomainEvent<TAggRootId> domainEvent, T result = default(T))
        {
            return new EventResponse<TAggRootId>(domainEvent, ActionResponse.Success<T>(result));
        }
        public static EventResponse<TAggRootId> Reject(IDomainEvent<TAggRootId> domainEvent, string message, string detail = null)
        {
            return new EventResponse<TAggRootId>(domainEvent, ActionResponse.Fail(message, detail));
        }
        public static EventResponse<TAggRootId> Reject<T>(IDomainEvent<TAggRootId> domainEvent, string message, string detail = null)
        {
            return new EventResponse<TAggRootId>(domainEvent, ActionResponse.Fail<T>(message, detail));
        }
    }
}
