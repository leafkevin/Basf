using System;

namespace Basf.Utilities
{
    /// <summary>
    /// Represents a 128-bit unsigned integer.
    /// </summary>
    public struct uint128 : IEquatable<uint128>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:uint128"/> struct.
        /// </summary>
        public uint128(ulong low, ulong high)
            : this()
        {
            Low = low;
            High = high;
        }
        /// <summary>
        /// Gets or sets the low-order 64-bits.
        /// </summary>
        /// <value>The low-order 64-bits.</value>
        public ulong Low { get; set; }
        /// <summary>
        /// Gets or sets the high-order 64-bits.
        /// </summary>
        /// <value>The high-order 64-bits.</value>
        public ulong High { get; set; }
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(uint128 other)
        {
            return Low == other.Low && High == other.High;
        }
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is uint128 && Equals((uint128)obj);
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Low.GetHashCode() * 397) ^ High.GetHashCode();
            }
        }
    }
}