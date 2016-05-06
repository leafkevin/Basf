using System;

namespace Basf.Domain.Event
{
    public abstract class DomainEvent<TAggRootId> : IDomainEvent<TAggRootId>, IComparable, IComparable<DomainEvent<TAggRootId>>, IEquatable<DomainEvent<TAggRootId>>
    {
        public string UniqueId { get; private set; }
        public string AggRootType { get; set; }
        public TAggRootId AggRootId { get; set; }
        public string CommandId { get; set; }
        public string RoutingKey { get; set; }
        public DateTime Timestamp { get; private set; }
        public int Version { get; set; }
        string IDomainEvent.AggRootId
        {
            get
            {
                return this.AggRootId.ToString();
            }
            set
            {
                this.AggRootId = (TAggRootId)Convert.ChangeType(value, typeof(TAggRootId));
            }
        }
        public DomainEvent(string commandId)
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.CommandId = commandId;
            this.Timestamp = DateTime.Now;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = hashCode * 23 + this.AggRootType.GetHashCode();
                hashCode = hashCode * 23 + this.AggRootId.GetHashCode();
                hashCode = hashCode * 23 + this.Version.GetHashCode();
                return hashCode;
            }
        }
        public bool Equals(DomainEvent<TAggRootId> other)
        {
            return this.AggRootType == other.AggRootType && this.AggRootId.Equals(other.AggRootId) && this.Version == other.Version;
        }
        public int CompareTo(DomainEvent<TAggRootId> other)
        {
            return this.ToString().CompareTo(other.ToString());
        }       
        public int CompareTo(IDomainEvent other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        public bool Equals(IDomainEvent other)
        {
            return this.AggRootType == other.AggRootType && this.AggRootId.Equals(other.AggRootId) && this.Version == other.Version;
        }
        public int CompareTo(IDomainEvent<TAggRootId> other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        public bool Equals(IDomainEvent<TAggRootId> other)
        {
            return this.AggRootType == other.AggRootType && this.AggRootId.Equals(other.AggRootId) && this.Version == other.Version;
        }
        public int CompareTo(object other)
        {
            return other != null ? this.ToString().CompareTo(other.ToString()) : 1;
        }
        public override string ToString()
        {
            return String.Format("{{AggRootType:{0},AggRootId:{1},Version:{2}}}", this.AggRootType, this.AggRootId, this.Version);
        }
    }
}
