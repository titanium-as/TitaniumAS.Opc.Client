using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents the OPC server description.
    /// </summary>
    /// <seealso cref="System.IEquatable{TitaniumAS.Opc.Common.OpcServerDescription}" />
    public class OpcServerDescription : IEquatable<OpcServerDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerDescription"/> class.
        /// </summary>
        public OpcServerDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerDescription"/> class.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="clsid">The OPC server class identifier.</param>
        /// <param name="progId">The OPC server programmatic identifier.</param>
        /// <param name="userType">The user type information.</param>
        /// <param name="vendorIndependentProgId">The vendor independent programmatic identifier of the OPC server.</param>
        public OpcServerDescription(string host, Guid clsid, string progId, string userType,
            string vendorIndependentProgId)
            : this(host, clsid, progId, userType)
        {
            VendorIndependentProgId = vendorIndependentProgId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerDescription"/> class.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="clsid">The OPC server class identifier.</param>
        /// <param name="progId">The OPC server programmatic identifier.</param>
        /// <param name="userType">The user type information.</param>
        public OpcServerDescription(string host, Guid clsid, string progId, string userType) : this(host, clsid)
        {
            ProgId = progId;
            UserType = userType;
            VendorIndependentProgId = string.Empty;
            Categories = new OpcServerCategory[0];
            Uri = UrlBuilder.Build(progId, host);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerDescription"/> class.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="clsid">The OPC server class identifier.</param>
        public OpcServerDescription(string host, Guid clsid)
        {
            Host = host ?? "localhost";
            CLSID = clsid;
            Uri = UrlBuilder.Build(clsid, host);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcServerDescription"/> class.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="progId">The OPC server programmatic identifier.</param>
        public OpcServerDescription(string host, string progId)
        {
            Host = host ?? "localhost";
            ProgId = progId;
            Uri = UrlBuilder.Build(progId, host);
        }

        /// <summary>
        /// Gets or sets the OPC server URI.
        /// </summary>
        /// <value>
        /// The OPC server URI.
        /// </value>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the OPC server host.
        /// </summary>
        /// <value>
        /// The OPC server host.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the OPC server class identifier.
        /// </summary>
        /// <value>
        /// The OPC server class identifier.
        /// </value>
        public Guid CLSID { get; set; }

        /// <summary>
        /// Gets or sets the OPC server programmatic identifier.
        /// </summary>
        /// <value>
        /// The OPC server programmatic identifier.
        /// </value>
        public string ProgId { get; set; }

        /// <summary>
        /// Gets or sets the user type information.
        /// </summary>
        /// <value>
        /// The user type information.
        /// </value>
        public string UserType { get; set; }

        /// <summary>
        /// Gets or sets the vendor independent programmatic identifier of the OPC server.
        /// </summary>
        /// <value>
        /// The vendor independent programmatic identifier.
        /// </value>
        public string VendorIndependentProgId { get; set; }

        /// <summary>
        /// Gets or sets the OPC server categories.
        /// </summary>
        /// <value>
        /// The OPC server categories.
        /// </value>
        public OpcServerCategory[] Categories { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(OpcServerDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CLSID.Equals(other.CLSID);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Uri.ToString();
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
            return Equals((OpcServerDescription) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return CLSID.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(OpcServerDescription left, OpcServerDescription right)
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
        public static bool operator !=(OpcServerDescription left, OpcServerDescription right)
        {
            return !Equals(left, right);
        }
    }
}