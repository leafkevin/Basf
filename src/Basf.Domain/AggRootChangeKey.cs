using System;

namespace Basf.Domain
{
    public struct AggRootChangeKey : IComparable, IComparable<AggRootChangeKey>, IEquatable<AggRootChangeKey>
    {
        public string CommandId { get; private set; }
        public string AggRootType { get; private set; }
        public string AggRootId { get; private set; }
        public RuntimeTypeHandle EventType { get; private set; }
        public AggRootChangeKey(string commandId, string aggRootType, string aggRootId, RuntimeTypeHandle eventType)
        {
            this.CommandId = commandId;
            this.AggRootType = aggRootType;
            this.AggRootId = aggRootId;
            this.EventType = eventType;
        }
        public AggRootChangeKey(string commandId, AggRootKey aggRoot, RuntimeTypeHandle eventType)
        {
            this.CommandId = commandId;
            this.AggRootType = aggRoot.AggRootType;
            this.AggRootId = aggRoot.AggRootId;
            this.EventType = eventType;
        }
        public override string ToString()
        {
            return String.Format("{CommandId:'{0}',AggRootType:'{1}',AggRootId:'{2}',EventType:'{3}'}}",
                this.CommandId, this.AggRootType, this.AggRootId, this.EventType);
        }
        public override bool Equals(object obj)
        {
            return (obj is AggRootChangeKey) && this.Equals((AggRootChangeKey)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = hashCode * 23 + this.CommandId.GetHashCode();
                hashCode = hashCode * 23 + this.AggRootType.GetHashCode();
                hashCode = hashCode * 23 + this.AggRootId.GetHashCode();
                hashCode = hashCode * 23 + this.EventType.GetHashCode();
                return hashCode;
            }
        }
        public int CompareTo(AggRootChangeKey other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        public int CompareTo(object other)
        {
            return other != null ? this.ToString().CompareTo(other.ToString()) : 1;
        }
        public bool Equals(AggRootChangeKey other)
        {
            return this.CompareTo(other) == 0;
        }
        public static bool operator ==(AggRootChangeKey a, AggRootChangeKey b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(AggRootChangeKey a, AggRootChangeKey b)
        {
            return !a.Equals(b);
        }
    }
}
