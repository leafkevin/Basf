using System;

namespace Basf.Domain
{
    public struct AggRootKey : IComparable, IComparable<AggRootKey>, IEquatable<AggRootKey>
    {
        public string AggRootType { get; private set; }
        public string AggRootId { get; private set; }
        public AggRootKey(string aggRootType, string aggRootId)
        {
            this.AggRootType = aggRootType;
            this.AggRootId = aggRootId;
        }
        public override string ToString()
        {
            return String.Format("{{AggRootType:{0},AggRootId:{1}}}", this.AggRootType, this.AggRootId);
        }
        public override bool Equals(object obj)
        {
            return (obj is AggRootKey) && this.Equals((AggRootKey)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = hashCode * 23 + this.AggRootType.GetHashCode();
                hashCode = hashCode * 23 + this.AggRootId.GetHashCode();
                return hashCode;
            }
        }
        public int CompareTo(AggRootKey other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        public int CompareTo(object other)
        {
            return other != null ? this.ToString().CompareTo(other.ToString()) : 1;
        }
        public bool Equals(AggRootKey other)
        {
            return this.AggRootType == other.AggRootType && this.AggRootId == other.AggRootId;
        }
        public static bool operator ==(AggRootKey a, AggRootKey b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(AggRootKey a, AggRootKey b)
        {
            return !a.Equals(b);
        }
    }
}
