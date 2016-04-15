using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents the OPC server category.
    /// </summary>
    /// <seealso cref="System.IEquatable{TitaniumAS.Opc.Common.OpcServerCategory}" />
    public class OpcServerCategory : IEquatable<OpcServerCategory>
    {
        /// <summary>
        /// OPC Data Access Servers Version 1.0.
        /// </summary>
        public static readonly OpcServerCategory OpcDaServer10 =
            new OpcServerCategory(new Guid("63D5F430-CFE4-11D1-B2C8-0060083BA1FB"),
                "OPC Data Access Servers Version 1.0");

        /// <summary>
        /// OPC Data Access Servers Version 2.0.
        /// </summary>
        public static readonly OpcServerCategory OpcDaServer20 =
            new OpcServerCategory(new Guid("63D5F432-CFE4-11D1-B2C8-0060083BA1FB"),
                "OPC Data Access Servers Version 2.0");

        /// <summary>
        /// OPC Data Access Servers Version 3.0.
        /// </summary>
        public static readonly OpcServerCategory OpcDaServer30 =
            new OpcServerCategory(new Guid("CC603642-66D7-48F1-B69A-B625E73652D7"),
                "OPC Data Access Servers Version 3.0");

        /// <summary>
        /// OPC XML Data Access Servers Version 1.0.
        /// </summary>
        public static readonly OpcServerCategory OpcXmlDaServer10 =
            new OpcServerCategory(new Guid("3098EDA4-A006-48B2-A27F-247453959408"),
                "OPC XML Data Access Servers Version 1.0");

        /// <summary>
        /// OPC History Data Access Servers Version 1.0.
        /// </summary>
        public static readonly OpcServerCategory OpcHdaServer10 =
            new OpcServerCategory(new Guid("7DE5B060-E089-11D2-A5E6-000086339399"),
                "OPC History Data Access Servers Version 1.0");

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerCategory"/> class.
        /// </summary>
        /// <param name="catid">The category identifier.</param>
        /// <param name="description">The category description.</param>
        public OpcServerCategory(Guid catid, string description)
        {
            CATID = catid;
            Description = description;
        }

        /// <summary>
        /// Provides supported OPC server categories.
        /// </summary>
        /// <value>
        /// Array of supported OPC server categories.
        /// </value>
        public static OpcServerCategory[] OpcDaServers
        {
            get { return new[] {OpcDaServer10, OpcDaServer20, OpcDaServer30}; }
        }

        /// <summary>
        /// Gets the category identifier.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        public Guid CATID { get; private set; }

        /// <summary>
        /// Gets a text described the category.
        /// </summary>
        /// <value>
        /// The text described the category.
        /// </value>
        public string Description { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(OpcServerCategory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CATID.Equals(other.CATID);
        }

        /// <summary>
        /// Gets the OPC server category by category identifier.
        /// </summary>
        /// <param name="catid">The OPC server category identifier.</param>
        /// <returns>The OPC server category.</returns>
        public static OpcServerCategory Get(Guid catid)
        {
            if (OpcDaServer10.CATID == catid)
                return OpcDaServer10;
            if (OpcDaServer20.CATID == catid)
                return OpcDaServer20;
            if (OpcDaServer30.CATID == catid)
                return OpcDaServer30;
            if (OpcXmlDaServer10.CATID == catid)
                return OpcXmlDaServer10;
            if (OpcHdaServer10.CATID == catid)
                return OpcHdaServer10;
            return null;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Description;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OpcServerCategory) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return CATID.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(OpcServerCategory left, OpcServerCategory right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(OpcServerCategory left, OpcServerCategory right)
        {
            return !Equals(left, right);
        }
    }
}