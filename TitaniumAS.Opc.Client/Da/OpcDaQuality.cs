using System;
using System.Runtime.InteropServices;
using System.Text;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a quality of specific item.
    /// </summary>
    /// <seealso cref="System.IComparable" />
    /// <seealso cref="System.IEquatable{TitaniumAS.Opc.Da.OpcDaQuality}" />
    /// <seealso cref="System.IEquatable{System.Int16}" />
    /// <seealso cref="System.IComparable{TitaniumAS.Opc.Da.OpcDaQuality}" />
    /// <seealso cref="System.IComparable{System.Int16}" />
    [StructLayout(LayoutKind.Sequential)]
    public struct OpcDaQuality :
        IComparable
        , IEquatable<OpcDaQuality>
        , IEquatable<short>
        , IComparable<OpcDaQuality>
        , IComparable<short>

    {
        private readonly short m_value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaQuality"/> struct.
        /// </summary>
        /// <param name="qualityStatus">The quality status.</param>
        public OpcDaQuality(OpcDaQualityStatus qualityStatus)
            : this()
        {
            m_value = (short) qualityStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaQuality"/> struct.
        /// </summary>
        /// <param name="value">The quality status value.</param>
        public OpcDaQuality(short value)
        {
            m_value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="OpcDaQuality"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator short(OpcDaQuality This)
        {
            return This.m_value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int16"/> to <see cref="OpcDaQuality"/>.
        /// </summary>
        /// <param name="This">The quality.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator OpcDaQuality(short This)
        {
            return new OpcDaQuality(This);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="OpcDaQuality"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="This">The quality.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator bool(OpcDaQuality This)
        {
            return This.Master == OpcDaQualityMaster.Good;
        }

        /// <summary>
        /// Implements the operator true.
        /// </summary>
        /// <param name="This">The quality.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator true(OpcDaQuality This)
        {
            return This == true;
        }

        /// <summary>
        /// Implements the operator false.
        /// </summary>
        /// <param name="This">The quality.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator false(OpcDaQuality This)
        {
            return This == false;
        }

        #region IEquatable<> Members

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified quality object.
        /// </summary>
        /// <param name="that">The quality.</param>
        /// <returns><c>true</c> if specified quality object has the same value as this instance; otherwise, false.</returns>
        public bool Equals(OpcDaQuality that)
        {
            return (m_value == that.m_value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified quality status value.
        /// </summary>
        /// <param name="that">The HRESULT.</param>
        /// <returns><c>true</c> if specified value is the same as value of this instance; otherwise, false.</returns>
        public bool Equals(short that)
        {
            return (m_value == that);
        }

        #endregion

        #region System.Object Members

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is OpcDaQuality)
                return Equals((OpcDaQuality) obj);
            if (obj is int)
                return Equals((int) obj);
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_value;
        }

        /// <summary>
        /// Gets the quality master status.
        /// </summary>
        /// <value>
        /// The quality master status.
        /// </value>
        public OpcDaQualityMaster Master
        {
            get { return (OpcDaQualityMaster) (m_value & (short) OPC_QUALITY_MASKS.MASTER_MASK); }
        }

        /// <summary>
        /// Gets the quality status.
        /// </summary>
        /// <value>
        /// The quality status.
        /// </value>
        public OpcDaQualityStatus Status
        {
            get { return (OpcDaQualityStatus) (m_value & (short) OPC_QUALITY_MASKS.STATUS_MASK); }
        }

        /// <summary>
        /// Gets the limit field of quality status. It is valid regardless of the quality and substatus. In some cases such as sensor failure it can provide useful diagnostic information.
        /// </summary>
        /// <value>
        /// The limit field of quality status.
        /// </value>
        public OpcDaQualityLimit Limit
        {
            get { return (OpcDaQualityLimit) (m_value & (short) OPC_QUALITY_MASKS.LIMIT_MASK); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(256);
            stringBuilder.AppendFormat("{0}+{1}+{2}", Master, Status, Limit);
            return stringBuilder.ToString();
        }

        #endregion

        #region IComparable<> Members

        /// <summary>
        /// Compares the current <see cref="OpcDaQuality"/> with another OpcDaQuality and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="that">The quality.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Less than zero - This instance precedes obj in the sort order; Zero - This instance occurs in the same position in the sort order as obj; Greater than zero - This instance follows obj in the sort order.</returns>
        public int CompareTo(OpcDaQuality that)
        {
            return (m_value < that.m_value) ? -1 : (m_value > that.m_value) ? +1 : 0;
        }

        /// <summary>
        /// Compares the current <see cref="OpcDaQuality"/> with another OpcDaQuality value and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="that">The quality.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Less than zero - This instance precedes obj in the sort order; Zero - This instance occurs in the same position in the sort order as obj; Greater than zero - This instance follows obj in the sort order.</returns>
        public int CompareTo(short that)
        {
            return (m_value < that) ? -1 : (m_value > that) ? +1 : 0;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="System.ArgumentException">obj</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return +1;
            if (obj is OpcDaQuality)
                return CompareTo((OpcDaQuality) obj);
            if (obj is short)
                return CompareTo((short) obj);
            throw new ArgumentException("obj");
        }

        #endregion
    }
}