using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Common
{
//#define DOTNET20 //uncomment for .NET 2.0

    #region HRESULT

    /// <summary>
    /// Represents the HRESULT of performed operation.
    /// </summary>
    /// <seealso cref="System.IComparable" />
    /// <seealso cref="System.IEquatable{TitaniumAS.Opc.Common.HRESULT}" />
    /// <seealso cref="System.IEquatable{System.Int32}" />
    /// <seealso cref="System.IComparable{TitaniumAS.Opc.Common.HRESULT}" />
    /// <seealso cref="System.IComparable{System.Int32}" />
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct HRESULT : IComparable, IEquatable<HRESULT>, IEquatable<int>, IComparable<HRESULT>, IComparable<int>
    {
        private int m_value;

        /// <summary>
        /// Adds the ITF facility error code within a range from 0x0200 to 0xFFFF with description.
        /// </summary>
        /// <param name="itfErrorCode">The ITF facility error code.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// The HRESULT corresponding to the ITF facility error code.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">itfErrorCode;The ITF facility error code values should be in the range of 0x0200-0xFFFF.</exception>
        public static HRESULT AddItfError(int itfErrorCode, string description)
        {
            if (itfErrorCode < 0x0200 || itfErrorCode > 0xFFFF)
                throw new ArgumentOutOfRangeException("itfErrorCode", "The ITF facility error code values should be in the range of 0x0200-0xFFFF.");

            // See: https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms690088(v=vs.85).aspx
            // and https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms679751(v=vs.85).aspx
            uint value = MakeHresult(1 /*1 - Failure*/, 4 /*FACILITY_ITF*/, (ushort) itfErrorCode);

            var hr = (int) value;
            valueToDescription[hr] = description;
            return hr;
        }

        // See: https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms694497(v=vs.85).aspx
        private static uint MakeHresult(ushort sev, ushort fac, ushort code)
        {
            return ((uint)(sev) << 31) | ((uint)(fac) << 16) | ((uint)(code));
        }

        /// <summary>
        /// Gets the description of HRESULT.
        /// </summary>
        /// <param name="hr">The HRESULT.</param>
        /// <returns>The description of HRESULT.</returns>
        public static string GetDescription(int hr)
        {
            string description;
            if (valueToDescription.TryGetValue(hr, out description))
                return description;

            return GetExceptionForHR(hr).Message;
        }

        /// <summary>
        /// Gets the Exception object for HRESULT.
        /// </summary>
        /// <param name="hr">The HRESULT.</param>
        /// <returns>The Exception object for HRESULT.</returns>
        public static Exception GetExceptionForHR(int hr)
        {
            return Marshal.GetExceptionForHR(hr);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HRESULT"/> struct.
        /// </summary>
        /// <param name="value">The value of HRESULT.</param>
        public HRESULT(int value)
        {
            m_value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HRESULT"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="This">The HRESULT.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator int(HRESULT This)
        {
            return This.m_value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="HRESULT"/>.
        /// </summary>
        /// <param name="This">The HRESULT.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator HRESULT(int This)
        {
            return new HRESULT(This);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HRESULT"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="This">The HRESULT.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator bool(HRESULT This)
        {
            if (This.m_value == S_OK) return true;
            if (This.m_value > 0) return false;
            throw GetExceptionForHR(This.m_value);
        }

        /// <summary>
        /// Implements the operator true.
        /// </summary>
        /// <param name="This">The HRESULT.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator true(HRESULT This)
        {
            return (bool) This == true;
        }

        /// <summary>
        /// Implements the operator false.
        /// </summary>
        /// <param name="This">The HRESULT.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator false(HRESULT This)
        {
            return (bool) This == false;
        }

        #region IEquatable<> Members

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified HRESULT object.
        /// </summary>
        /// <param name="that">The HRESULT.</param>
        /// <returns><c>true</c> if specified HRESULT object has the same value as this instance; otherwise, false.</returns>
        public bool Equals(HRESULT that)
        {
            return (m_value == that.m_value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified HRESULT value.
        /// </summary>
        /// <param name="that">The HRESULT.</param>
        /// <returns><c>true</c> if specified value is the same as value of this instance; otherwise, false.</returns>
        public bool Equals(int that)
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
            if (obj is HRESULT)
                return Equals((HRESULT) obj);
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string description;
            if (!valueToDescription.TryGetValue(m_value, out description))
            {
                description = m_value.ToString();
            }
            return description;
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this <see cref="HRESULT"/> is failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        public bool Failed
        {
            get { return m_value < 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="HRESULT"/> is succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        public bool Succeeded
        {
            get { return m_value >= 0; }
        }

        /// <summary>
        /// Gets a value indicating whether specified <see cref="HRESULT"/> value is failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        public static bool FAILED(int hr)
        {
            return hr < 0;
        }

        /// <summary>
        /// Gets a value indicating whether specified <see cref="HRESULT"/> value is succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        public static bool SUCCEEDED(int hr)
        {
            return hr >= 0;
        }

        #region IComparable<> Members

        /// <summary>
        /// Compares the current <see cref="HRESULT"/> with another HRESULT and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="that">The HRESULT.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Less than zero - This instance precedes obj in the sort order; Zero - This instance occurs in the same position in the sort order as obj; Greater than zero - This instance follows obj in the sort order.</returns>
        public int CompareTo(HRESULT that)
        {
            return (m_value < that.m_value) ? -1 : (m_value > that.m_value) ? +1 : 0;
        }

        /// <summary>
        /// Compares the current <see cref="HRESULT"/> with another HRESULT value and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="that">The HRESULT.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Less than zero - This instance precedes obj in the sort order; Zero - This instance occurs in the same position in the sort order as obj; Greater than zero - This instance follows obj in the sort order.</returns>
        public int CompareTo(int that)
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
        /// <exception cref="System.ArgumentException">Arg_MustBeHRESULT</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return +1;
            if (obj is HRESULT)
                return CompareTo((HRESULT) obj);
            if (obj is int)
                return CompareTo((int) obj);
            throw new ArgumentException("Arg_MustBeHRESULT");
        }

        #endregion

        #region Error Codes

        //  The RegExp strings (in MSVS syntax) were used to parse WINERR.H

        //Find what: ^{:b*}//:b*\n:b*\#define:b+{:i}:b+(_HRESULT_TYPEDEF_)\({0x:h}L\)
        //Replace with: \1//\n\1public const int \2 = unchecked((int)\3);

        //Find what: ^{:b*}//:b*\n:b*//:b*{MessageId\::b*:i}:b*\n:b*//:b*\n:b*//:b*MessageText\::b*\n:b*//:b*\n:b*//:b*{.*}\n:b*//:b*\n
        //Replace with: \1///<summary>\n\1/// \3\n\1///</summary>\n

        //Find what: ^{:b*}//:b*\n:b*//:b*(MessageId\::b*:i):b*\n(:b*//:b*\n)+:b*//:b*MessageText\::b*\n(:b*//:b*\n)*:b*//:b*{.*}\n(:b*//:b*\n)+:b*\#define:b+{:i}:b+(_HRESULT_TYPEDEF_)\({0x:h}L\)
        //Replace with: \1///<summary>\n\1///\2\n\1///</summary>\n\1public const int \3 = unchecked((int)\4);

        // {^{:b+}///\<summary\>\n:b+///:b*{.+}\n:b+///\</summary\>}
        // \1\n\2[Description("\3")]

        [AttributeUsage(AttributeTargets.All)]
        private class DescriptionAttribute : Attribute
        {
            protected string m_description;

            public DescriptionAttribute(string description)
            {
                m_description = description;
            }

            public string description
            {
                get { return m_description; }
            }
        }

        /// <summary>
        /// Success code.
        /// </summary>
        [Description("Success code")] public const int S_OK = unchecked((int) 0x00000000);

        /// <summary>
        /// Success code.
        /// </summary>
        [Description("Success code")] public const int NO_ERROR = unchecked((int) 0x00000000);

        /// <summary>
        /// Success code.
        /// </summary>
        [Description("Success code")] public const int NOERROR = unchecked((int) 0x00000000);

        /// <summary>
        /// Success code false.
        /// </summary>
        [Description("Success code false")] public const int S_FALSE = unchecked((int) 0x00000001);

        /// <summary>
        /// Catastrophic failure.
        /// </summary>
        [Description("Catastrophic failure")] public const int E_UNEXPECTED = unchecked((int) 0x8000FFFF);

        /// <summary>
        /// Not implemented.
        /// </summary>
        [Description("Not implemented")] public const int E_NOTIMPL = unchecked((int) 0x80004001);

        /// <summary>
        /// Ran out of memory.
        /// </summary>
        [Description("Ran out of memory")] public const int E_OUTOFMEMORY = unchecked((int) 0x8007000E);

        /// <summary>
        /// One or more arguments are invalid.
        /// </summary>
        [Description("One or more arguments are invalid")] public const int E_INVALIDARG = unchecked((int) 0x80070057);

        /// <summary>
        /// No such interface supported.
        /// </summary>
        [Description("No such interface supported")] public const int E_NOINTERFACE = unchecked((int) 0x80004002);

        /// <summary>
        /// Invalid pointer.
        /// </summary>
        [Description("Invalid pointer")] public const int E_POINTER = unchecked((int) 0x80004003);

        /// <summary>
        /// Invalid handle.
        /// </summary>
        [Description("Invalid handle")] public const int E_HANDLE = unchecked((int) 0x80070006);

        /// <summary>
        /// Operation aborted.
        /// </summary>
        [Description("Operation aborted")] public const int E_ABORT = unchecked((int) 0x80004004);

        /// <summary>
        /// Unspecified error.
        /// </summary>
        [Description("Unspecified error")] public const int E_FAIL = unchecked((int) 0x80004005);

        /// <summary>
        /// General access denied error.
        /// </summary>
        [Description("General access denied error")] public const int E_ACCESSDENIED = unchecked((int) 0x80070005);

        /// <summary>
        /// The data necessary to complete this operation is not yet available.
        /// </summary>
        [Description("The data necessary to complete this operation is not yet available.")] public const int E_PENDING
            = unchecked((int) 0x8000000A);

        // ******************
        // FACILITY_NULL
        // ******************

        #region (0x000000 - 0x00FFFF) FACILITY_NULL errors

        #region (0x004000 - 0x0040FF) CO errors

        ///<summary>
        ///Thread local storage failure
        ///</summary>
        [Description("Thread local storage failure")] public const int CO_E_INIT_TLS = unchecked((int) 0x80004006);

        ///<summary>
        ///Get shared memory allocator failure
        ///</summary>
        [Description("Get shared memory allocator failure")] public const int CO_E_INIT_SHARED_ALLOCATOR =
            unchecked((int) 0x80004007);

        ///<summary>
        ///Get memory allocator failure
        ///</summary>
        [Description("Get memory allocator failure")] public const int CO_E_INIT_MEMORY_ALLOCATOR =
            unchecked((int) 0x80004008);

        ///<summary>
        ///Unable to initialize class cache
        ///</summary>
        [Description("Unable to initialize class cache")] public const int CO_E_INIT_CLASS_CACHE =
            unchecked((int) 0x80004009);

        ///<summary>
        ///Unable to initialize RPC services
        ///</summary>
        [Description("Unable to initialize RPC services")] public const int CO_E_INIT_RPC_CHANNEL =
            unchecked((int) 0x8000400A);

        ///<summary>
        ///Cannot set thread local storage channel control
        ///</summary>
        [Description("Cannot set thread local storage channel control")] public const int
            CO_E_INIT_TLS_SET_CHANNEL_CONTROL = unchecked((int) 0x8000400B);

        ///<summary>
        ///Could not allocate thread local storage channel control
        ///</summary>
        [Description("Could not allocate thread local storage channel control")] public const int
            CO_E_INIT_TLS_CHANNEL_CONTROL = unchecked((int) 0x8000400C);

        ///<summary>
        ///The user supplied memory allocator is unacceptable
        ///</summary>
        [Description("The user supplied memory allocator is unacceptable")] public const int
            CO_E_INIT_UNACCEPTED_USER_ALLOCATOR = unchecked((int) 0x8000400D);

        ///<summary>
        ///The OLE service mutex already exists
        ///</summary>
        [Description("The OLE service mutex already exists")] public const int CO_E_INIT_SCM_MUTEX_EXISTS =
            unchecked((int) 0x8000400E);

        ///<summary>
        ///The OLE service file mapping already exists
        ///</summary>
        [Description("The OLE service file mapping already exists")] public const int CO_E_INIT_SCM_FILE_MAPPING_EXISTS
            = unchecked((int) 0x8000400F);

        ///<summary>
        ///Unable to map view of file for OLE service
        ///</summary>
        [Description("Unable to map view of file for OLE service")] public const int CO_E_INIT_SCM_MAP_VIEW_OF_FILE =
            unchecked((int) 0x80004010);

        ///<summary>
        ///Failure attempting to launch OLE service
        ///</summary>
        [Description("Failure attempting to launch OLE service")] public const int CO_E_INIT_SCM_EXEC_FAILURE =
            unchecked((int) 0x80004011);

        ///<summary>
        ///There was an attempt to call CoInitialize a second time while single threaded
        ///</summary>
        [Description("There was an attempt to call CoInitialize a second time while single threaded")] public const int
            CO_E_INIT_ONLY_SINGLE_THREADED = unchecked((int) 0x80004012);

        ///<summary>
        ///A Remote activation was necessary but was not allowed
        ///</summary>
        [Description("A Remote activation was necessary but was not allowed")] public const int CO_E_CANT_REMOTE =
            unchecked((int) 0x80004013);

        ///<summary>
        ///A Remote activation was necessary but the server name provided was invalid
        ///</summary>
        [Description("A Remote activation was necessary but the server name provided was invalid")] public const int
            CO_E_BAD_SERVER_NAME = unchecked((int) 0x80004014);

        ///<summary>
        ///The class is configured to run as a security id different from the caller
        ///</summary>
        [Description("The class is configured to run as a security id different from the caller")] public const int
            CO_E_WRONG_SERVER_IDENTITY = unchecked((int) 0x80004015);

        ///<summary>
        ///Use of Ole1 services requiring DDE windows is disabled
        ///</summary>
        [Description("Use of Ole1 services requiring DDE windows is disabled")] public const int CO_E_OLE1DDE_DISABLED =
            unchecked((int) 0x80004016);

        ///<summary>
        ///A RunAs specification must be &lt;domain name>\<user name> or simply &lt;user name>
        ///</summary>
        [Description("A RunAs specification must be <domain name>\\<user name> or simply <user name>")] public const int
            CO_E_RUNAS_SYNTAX = unchecked((int) 0x80004017);

        ///<summary>
        ///The server process could not be started.  The pathname may be incorrect.
        ///</summary>
        [Description("The server process could not be started.  The pathname may be incorrect.")] public const int
            CO_E_CREATEPROCESS_FAILURE = unchecked((int) 0x80004018);

        ///<summary>
        ///The server process could not be started as the configured identity.
        ///<para>The pathname may be incorrect or unavailable.</para>
        ///</summary>
        [Description(
            "The server process could not be started as the configured identity.\nThe pathname may be incorrect or unavailable."
            )] public const int CO_E_RUNAS_CREATEPROCESS_FAILURE = unchecked((int) 0x80004019);

        ///<summary>
        ///The server process could not be started because the configured identity is incorrect.
        ///<para>Check the username and password.</para>
        ///</summary>
        [Description(
            "The server process could not be started because the configured identity is incorrect.\nCheck the username and password."
            )] public const int CO_E_RUNAS_LOGON_FAILURE = unchecked((int) 0x8000401A);

        ///<summary>
        ///The client is not allowed to launch this server.
        ///</summary>
        [Description("The client is not allowed to launch this server.")] public const int CO_E_LAUNCH_PERMSSION_DENIED
            = unchecked((int) 0x8000401B);

        ///<summary>
        ///The service providing this server could not be started.
        ///</summary>
        [Description("The service providing this server could not be started.")] public const int
            CO_E_START_SERVICE_FAILURE = unchecked((int) 0x8000401C);

        ///<summary>
        ///This computer was unable to communicate with the computer providing the server.
        ///</summary>
        [Description("This computer was unable to communicate with the computer providing the server.")] public const
            int CO_E_REMOTE_COMMUNICATION_FAILURE = unchecked((int) 0x8000401D);

        ///<summary>
        ///The server did not respond after being launched.
        ///</summary>
        [Description("The server did not respond after being launched.")] public const int CO_E_SERVER_START_TIMEOUT =
            unchecked((int) 0x8000401E);

        ///<summary>
        ///The registration information for this server is inconsistent or incomplete.
        ///</summary>
        [Description("The registration information for this server is inconsistent or incomplete.")] public const int
            CO_E_CLSREG_INCONSISTENT = unchecked((int) 0x8000401F);

        ///<summary>
        ///The registration information for this interface is inconsistent or incomplete.
        ///</summary>
        [Description("The registration information for this interface is inconsistent or incomplete.")] public const int
            CO_E_IIDREG_INCONSISTENT = unchecked((int) 0x80004020);

        ///<summary>
        ///The operation attempted is not supported.
        ///</summary>
        [Description("The operation attempted is not supported.")] public const int CO_E_NOT_SUPPORTED =
            unchecked((int) 0x80004021);

        ///<summary>
        ///A dll must be loaded.
        ///</summary>
        [Description("A dll must be loaded.")] public const int CO_E_RELOAD_DLL = unchecked((int) 0x80004022);

        ///<summary>
        ///A Microsoft Software Installer error was encountered.
        ///</summary>
        [Description("A Microsoft Software Installer error was encountered.")] public const int CO_E_MSI_ERROR =
            unchecked((int) 0x80004023);

        ///<summary>
        ///The specified activation could not occur in the client context as specified.
        ///</summary>
        [Description("The specified activation could not occur in the client context as specified.")] public const int
            CO_E_ATTEMPT_TO_CREATE_OUTSIDE_CLIENT_CONTEXT = unchecked((int) 0x80004024);

        ///<summary>
        ///Activations on the server are paused.
        ///</summary>
        [Description("Activations on the server are paused.")] public const int CO_E_SERVER_PAUSED =
            unchecked((int) 0x80004025);

        ///<summary>
        ///Activations on the server are not paused.
        ///</summary>
        [Description("Activations on the server are not paused.")] public const int CO_E_SERVER_NOT_PAUSED =
            unchecked((int) 0x80004026);

        ///<summary>
        ///The component or application containing the component has been disabled.
        ///</summary>
        [Description("The component or application containing the component has been disabled.")] public const int
            CO_E_CLASS_DISABLED = unchecked((int) 0x80004027);

        ///<summary>
        ///The common language runtime is not available
        ///</summary>
        [Description("The common language runtime is not available")] public const int CO_E_CLRNOTAVAILABLE =
            unchecked((int) 0x80004028);

        ///<summary>
        ///The thread-pool rejected the submitted asynchronous work.
        ///</summary>
        [Description("The thread-pool rejected the submitted asynchronous work.")] public const int
            CO_E_ASYNC_WORK_REJECTED = unchecked((int) 0x80004029);

        ///<summary>
        ///The server started, but did not finish initializing in a timely fashion.
        ///</summary>
        [Description("The server started, but did not finish initializing in a timely fashion.")] public const int
            CO_E_SERVER_INIT_TIMEOUT = unchecked((int) 0x8000402A);

        ///<summary>
        ///Unable to complete the call since there is no COM+ security context inside IObjectControl.Activate.
        ///</summary>
        [Description(
            "Unable to complete the call since there is no COM+ security context inside IObjectControl.Activate.")] public const int CO_E_NO_SECCTX_IN_ACTIVATE = unchecked((int) 0x8000402B);

        ///<summary>
        ///The provided tracker configuration is invalid
        ///</summary>
        [Description("The provided tracker configuration is invalid")] public const int CO_E_TRACKER_CONFIG =
            unchecked((int) 0x80004030);

        ///<summary>
        ///The provided thread pool configuration is invalid
        ///</summary>
        [Description("The provided thread pool configuration is invalid")] public const int CO_E_THREADPOOL_CONFIG =
            unchecked((int) 0x80004031);

        ///<summary>
        ///The provided side-by-side configuration is invalid
        ///</summary>
        [Description("The provided side-by-side configuration is invalid")] public const int CO_E_SXS_CONFIG =
            unchecked((int) 0x80004032);

        ///<summary>
        ///The server principal name (SPN) obtained during security negotiation is malformed.
        ///</summary>
        [Description("The server principal name (SPN) obtained during security negotiation is malformed.")] public const
            int CO_E_MALFORMED_SPN = unchecked((int) 0x80004033);

        #endregion

        #endregion

        // ******************
        // FACILITY_RPC
        // ******************

        #region (0x010000 - 0x01FFFF) FACILITY_RPC errors

        //
        // Codes 0x0-0x11 are propagated from 16 bit OLE.
        //
        ///<summary>
        ///Call was rejected by callee.
        ///</summary>
        [Description("Call was rejected by callee.")] public const int RPC_E_CALL_REJECTED = unchecked((int) 0x80010001);

        ///<summary>
        ///Call was canceled by the message filter.
        ///</summary>
        [Description("Call was canceled by the message filter.")] public const int RPC_E_CALL_CANCELED =
            unchecked((int) 0x80010002);

        ///<summary>
        ///The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.
        ///</summary>
        [Description("The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.")] public const int RPC_E_CANTPOST_INSENDCALL = unchecked((int) 0x80010003);

        ///<summary>
        ///The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.
        ///</summary>
        [Description(
            "The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.")] public const int RPC_E_CANTCALLOUT_INASYNCCALL = unchecked((int) 0x80010004);

        ///<summary>
        ///It is illegal to call out while inside message filter.
        ///</summary>
        [Description("It is illegal to call out while inside message filter.")] public const int
            RPC_E_CANTCALLOUT_INEXTERNALCALL = unchecked((int) 0x80010005);

        ///<summary>
        ///The connection terminated or is in a bogus state and cannot be used any more. Other connections are still valid.
        ///</summary>
        [Description(
            "The connection terminated or is in a bogus state and cannot be used any more. Other connections are still valid."
            )] public const int RPC_E_CONNECTION_TERMINATED = unchecked((int) 0x80010006);

        ///<summary>
        ///The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call may have executed.
        ///</summary>
        [Description(
            "The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call may have executed."
            )] public const int RPC_E_SERVER_DIED = unchecked((int) 0x80010007);

        ///<summary>
        ///The caller (client) disappeared while the callee (server) was processing a call.
        ///</summary>
        [Description("The caller (client) disappeared while the callee (server) was processing a call.")] public const
            int RPC_E_CLIENT_DIED = unchecked((int) 0x80010008);

        ///<summary>
        ///The data packet with the marshalled parameter data is incorrect.
        ///</summary>
        [Description("The data packet with the marshalled parameter data is incorrect.")] public const int
            RPC_E_INVALID_DATAPACKET = unchecked((int) 0x80010009);

        ///<summary>
        ///The call was not transmitted properly; the message queue was full and was not emptied after yielding.
        ///</summary>
        [Description(
            "The call was not transmitted properly; the message queue was full and was not emptied after yielding.")] public const int RPC_E_CANTTRANSMIT_CALL = unchecked((int) 0x8001000A);

        ///<summary>
        ///The client (caller) cannot marshall the parameter data - low memory, etc.
        ///</summary>
        [Description("The client (caller) cannot marshall the parameter data - low memory, etc.")] public const int
            RPC_E_CLIENT_CANTMARSHAL_DATA = unchecked((int) 0x8001000B);

        ///<summary>
        ///The client (caller) cannot unmarshall the return data - low memory, etc.
        ///</summary>
        [Description("The client (caller) cannot unmarshall the return data - low memory, etc.")] public const int
            RPC_E_CLIENT_CANTUNMARSHAL_DATA = unchecked((int) 0x8001000C);

        ///<summary>
        ///The server (callee) cannot marshall the return data - low memory, etc.
        ///</summary>
        [Description("The server (callee) cannot marshall the return data - low memory, etc.")] public const int
            RPC_E_SERVER_CANTMARSHAL_DATA = unchecked((int) 0x8001000D);

        ///<summary>
        ///The server (callee) cannot unmarshall the parameter data - low memory, etc.
        ///</summary>
        [Description("The server (callee) cannot unmarshall the parameter data - low memory, etc.")] public const int
            RPC_E_SERVER_CANTUNMARSHAL_DATA = unchecked((int) 0x8001000E);

        ///<summary>
        ///Received data is invalid; could be server or client data.
        ///</summary>
        [Description("Received data is invalid; could be server or client data.")] public const int RPC_E_INVALID_DATA =
            unchecked((int) 0x8001000F);

        ///<summary>
        ///A particular parameter is invalid and cannot be (un)marshalled.
        ///</summary>
        [Description("A particular parameter is invalid and cannot be (un)marshalled.")] public const int
            RPC_E_INVALID_PARAMETER = unchecked((int) 0x80010010);

        ///<summary>
        ///There is no second outgoing call on same channel in DDE conversation.
        ///</summary>
        [Description("There is no second outgoing call on same channel in DDE conversation.")] public const int
            RPC_E_CANTCALLOUT_AGAIN = unchecked((int) 0x80010011);

        ///<summary>
        ///The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call did not execute.
        ///</summary>
        [Description(
            "The callee (server [not server application]) is not available and disappeared; all connections are invalid. The call did not execute."
            )] public const int RPC_E_SERVER_DIED_DNE = unchecked((int) 0x80010012);

        ///<summary>
        ///System call failed.
        ///</summary>
        [Description("System call failed.")] public const int RPC_E_SYS_CALL_FAILED = unchecked((int) 0x80010100);

        ///<summary>
        ///Could not allocate some required resource (memory, events, ...)
        ///</summary>
        [Description("Could not allocate some required resource (memory, events, ...)")] public const int
            RPC_E_OUT_OF_RESOURCES = unchecked((int) 0x80010101);

        ///<summary>
        ///Attempted to make calls on more than one thread in single threaded mode.
        ///</summary>
        [Description("Attempted to make calls on more than one thread in single threaded mode.")] public const int
            RPC_E_ATTEMPTED_MULTITHREAD = unchecked((int) 0x80010102);

        ///<summary>
        ///The requested interface is not registered on the server object.
        ///</summary>
        [Description("The requested interface is not registered on the server object.")] public const int
            RPC_E_NOT_REGISTERED = unchecked((int) 0x80010103);

        ///<summary>
        ///RPC could not call the server or could not return the results of calling the server.
        ///</summary>
        [Description("RPC could not call the server or could not return the results of calling the server.")] public
            const int RPC_E_FAULT = unchecked((int) 0x80010104);

        ///<summary>
        ///The server threw an exception.
        ///</summary>
        [Description("The server threw an exception.")] public const int RPC_E_SERVERFAULT = unchecked((int) 0x80010105);

        ///<summary>
        ///Cannot change thread mode after it is set.
        ///</summary>
        [Description("Cannot change thread mode after it is set.")] public const int RPC_E_CHANGED_MODE =
            unchecked((int) 0x80010106);

        ///<summary>
        ///The method called does not exist on the server.
        ///</summary>
        [Description("The method called does not exist on the server.")] public const int RPC_E_INVALIDMETHOD =
            unchecked((int) 0x80010107);

        ///<summary>
        ///The object invoked has disconnected from its clients.
        ///</summary>
        [Description("The object invoked has disconnected from its clients.")] public const int RPC_E_DISCONNECTED =
            unchecked((int) 0x80010108);

        ///<summary>
        ///The object invoked chose not to process the call now.  Try again later.
        ///</summary>
        [Description("The object invoked chose not to process the call now.  Try again later.")] public const int
            RPC_E_RETRY = unchecked((int) 0x80010109);

        ///<summary>
        ///The message filter indicated that the application is busy.
        ///</summary>
        [Description("The message filter indicated that the application is busy.")] public const int
            RPC_E_SERVERCALL_RETRYLATER = unchecked((int) 0x8001010A);

        ///<summary>
        ///The message filter rejected the call.
        ///</summary>
        [Description("The message filter rejected the call.")] public const int RPC_E_SERVERCALL_REJECTED =
            unchecked((int) 0x8001010B);

        ///<summary>
        ///A call control interfaces was called with invalid data.
        ///</summary>
        [Description("A call control interfaces was called with invalid data.")] public const int RPC_E_INVALID_CALLDATA
            = unchecked((int) 0x8001010C);

        ///<summary>
        ///An outgoing call cannot be made since the application is dispatching an input-synchronous call.
        ///</summary>
        [Description("An outgoing call cannot be made since the application is dispatching an input-synchronous call.")] public const int RPC_E_CANTCALLOUT_ININPUTSYNCCALL = unchecked((int) 0x8001010D);

        ///<summary>
        ///The application called an interface that was marshalled for a different thread.
        ///</summary>
        [Description("The application called an interface that was marshalled for a different thread.")] public const
            int RPC_E_WRONG_THREAD = unchecked((int) 0x8001010E);

        ///<summary>
        ///CoInitialize has not been called on the current thread.
        ///</summary>
        [Description("CoInitialize has not been called on the current thread.")] public const int RPC_E_THREAD_NOT_INIT
            = unchecked((int) 0x8001010F);

        ///<summary>
        ///The version of OLE on the client and server machines does not match.
        ///</summary>
        [Description("The version of OLE on the client and server machines does not match.")] public const int
            RPC_E_VERSION_MISMATCH = unchecked((int) 0x80010110);

        ///<summary>
        ///OLE received a packet with an invalid header.
        ///</summary>
        [Description("OLE received a packet with an invalid header.")] public const int RPC_E_INVALID_HEADER =
            unchecked((int) 0x80010111);

        ///<summary>
        ///OLE received a packet with an invalid extension.
        ///</summary>
        [Description("OLE received a packet with an invalid extension.")] public const int RPC_E_INVALID_EXTENSION =
            unchecked((int) 0x80010112);

        ///<summary>
        ///The requested object or interface does not exist.
        ///</summary>
        [Description("The requested object or interface does not exist.")] public const int RPC_E_INVALID_IPID =
            unchecked((int) 0x80010113);

        ///<summary>
        ///The requested object does not exist.
        ///</summary>
        [Description("The requested object does not exist.")] public const int RPC_E_INVALID_OBJECT =
            unchecked((int) 0x80010114);

        ///<summary>
        ///OLE has sent a request and is waiting for a reply.
        ///</summary>
        [Description("OLE has sent a request and is waiting for a reply.")] public const int RPC_S_CALLPENDING =
            unchecked((int) 0x80010115);

        ///<summary>
        ///OLE is waiting before retrying a request.
        ///</summary>
        [Description("OLE is waiting before retrying a request.")] public const int RPC_S_WAITONTIMER =
            unchecked((int) 0x80010116);

        ///<summary>
        ///Call context cannot be accessed after call completed.
        ///</summary>
        [Description("Call context cannot be accessed after call completed.")] public const int RPC_E_CALL_COMPLETE =
            unchecked((int) 0x80010117);

        ///<summary>
        ///Impersonate on unsecure calls is not supported.
        ///</summary>
        [Description("Impersonate on unsecure calls is not supported.")] public const int RPC_E_UNSECURE_CALL =
            unchecked((int) 0x80010118);

        ///<summary>
        ///Security must be initialized before any interfaces are marshalled or unmarshalled. It cannot be changed once initialized.
        ///</summary>
        [Description(
            "Security must be initialized before any interfaces are marshalled or unmarshalled. It cannot be changed once initialized."
            )] public const int RPC_E_TOO_LATE = unchecked((int) 0x80010119);

        ///<summary>
        ///No security packages are installed on this machine or the user is not logged on or there are no compatible security packages between the client and server.
        ///</summary>
        [Description(
            "No security packages are installed on this machine or the user is not logged on or there are no compatible security packages between the client and server."
            )] public const int RPC_E_NO_GOOD_SECURITY_PACKAGES = unchecked((int) 0x8001011A);

        ///<summary>
        ///Access is denied.
        ///</summary>
        [Description("Access is denied.")] public const int RPC_E_ACCESS_DENIED = unchecked((int) 0x8001011B);

        ///<summary>
        ///Remote calls are not allowed for this process.
        ///</summary>
        [Description("Remote calls are not allowed for this process.")] public const int RPC_E_REMOTE_DISABLED =
            unchecked((int) 0x8001011C);

        ///<summary>
        ///The marshaled interface data packet (OBJREF) has an invalid or unknown format.
        ///</summary>
        [Description("The marshaled interface data packet (OBJREF) has an invalid or unknown format.")] public const int
            RPC_E_INVALID_OBJREF = unchecked((int) 0x8001011D);

        ///<summary>
        ///No context is associated with this call. This happens for some custom marshalled calls and on the client side of the call.
        ///</summary>
        [Description(
            "No context is associated with this call. This happens for some custom marshalled calls and on the client side of the call."
            )] public const int RPC_E_NO_CONTEXT = unchecked((int) 0x8001011E);

        ///<summary>
        ///This operation returned because the timeout period expired.
        ///</summary>
        [Description("This operation returned because the timeout period expired.")] public const int RPC_E_TIMEOUT =
            unchecked((int) 0x8001011F);

        ///<summary>
        ///There are no synchronize objects to wait on.
        ///</summary>
        [Description("There are no synchronize objects to wait on.")] public const int RPC_E_NO_SYNC =
            unchecked((int) 0x80010120);

        ///<summary>
        ///Full subject issuer chain SSL principal name expected from the server.
        ///</summary>
        [Description("Full subject issuer chain SSL principal name expected from the server.")] public const int
            RPC_E_FULLSIC_REQUIRED = unchecked((int) 0x80010121);

        ///<summary>
        ///Principal name is not a valid MSSTD name.
        ///</summary>
        [Description("Principal name is not a valid MSSTD name.")] public const int RPC_E_INVALID_STD_NAME =
            unchecked((int) 0x80010122);

        ///<summary>
        ///Unable to impersonate DCOM client
        ///</summary>
        [Description("Unable to impersonate DCOM client")] public const int CO_E_FAILEDTOIMPERSONATE =
            unchecked((int) 0x80010123);

        ///<summary>
        ///Unable to obtain server's security context
        ///</summary>
        [Description("Unable to obtain server's security context")] public const int CO_E_FAILEDTOGETSECCTX =
            unchecked((int) 0x80010124);

        ///<summary>
        ///Unable to open the access token of the current thread
        ///</summary>
        [Description("Unable to open the access token of the current thread")] public const int
            CO_E_FAILEDTOOPENTHREADTOKEN = unchecked((int) 0x80010125);

        ///<summary>
        ///Unable to obtain user info from an access token
        ///</summary>
        [Description("Unable to obtain user info from an access token")] public const int CO_E_FAILEDTOGETTOKENINFO =
            unchecked((int) 0x80010126);

        ///<summary>
        ///The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method
        ///</summary>
        [Description(
            "The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method")] public const int CO_E_TRUSTEEDOESNTMATCHCLIENT = unchecked((int) 0x80010127);

        ///<summary>
        ///Unable to obtain the client's security blanket
        ///</summary>
        [Description("Unable to obtain the client's security blanket")] public const int CO_E_FAILEDTOQUERYCLIENTBLANKET
            = unchecked((int) 0x80010128);

        ///<summary>
        ///Unable to set a discretionary ACL into a security descriptor
        ///</summary>
        [Description("Unable to set a discretionary ACL into a security descriptor")] public const int
            CO_E_FAILEDTOSETDACL = unchecked((int) 0x80010129);

        ///<summary>
        ///The system function, AccessCheck, returned false
        ///</summary>
        [Description("The system function, AccessCheck, returned false")] public const int CO_E_ACCESSCHECKFAILED =
            unchecked((int) 0x8001012A);

        ///<summary>
        ///Either NetAccessDel or NetAccessAdd returned an error code.
        ///</summary>
        [Description("Either NetAccessDel or NetAccessAdd returned an error code.")] public const int
            CO_E_NETACCESSAPIFAILED = unchecked((int) 0x8001012B);

        ///<summary>
        ///One of the trustee strings provided by the user did not conform to the &lt;Domain>\<Name> syntax and it was not the "*" string
        ///</summary>
        [Description(
            "One of the trustee strings provided by the user did not conform to the <Domain>\\<Name> syntax and it was not the \"*\" string"
            )] public const int CO_E_WRONGTRUSTEENAMESYNTAX = unchecked((int) 0x8001012C);

        ///<summary>
        ///One of the security identifiers provided by the user was invalid
        ///</summary>
        [Description("One of the security identifiers provided by the user was invalid")] public const int
            CO_E_INVALIDSID = unchecked((int) 0x8001012D);

        ///<summary>
        ///Unable to convert a wide character trustee string to a multibyte trustee string
        ///</summary>
        [Description("Unable to convert a wide character trustee string to a multibyte trustee string")] public const
            int CO_E_CONVERSIONFAILED = unchecked((int) 0x8001012E);

        ///<summary>
        ///Unable to find a security identifier that corresponds to a trustee string provided by the user
        ///</summary>
        [Description("Unable to find a security identifier that corresponds to a trustee string provided by the user")] public const int CO_E_NOMATCHINGSIDFOUND = unchecked((int) 0x8001012F);

        ///<summary>
        ///The system function, LookupAccountSID, failed
        ///</summary>
        [Description("The system function, LookupAccountSID, failed")] public const int CO_E_LOOKUPACCSIDFAILED =
            unchecked((int) 0x80010130);

        ///<summary>
        ///Unable to find a trustee name that corresponds to a security identifier provided by the user
        ///</summary>
        [Description("Unable to find a trustee name that corresponds to a security identifier provided by the user")] public const int CO_E_NOMATCHINGNAMEFOUND = unchecked((int) 0x80010131);

        ///<summary>
        ///The system function, LookupAccountName, failed
        ///</summary>
        [Description("The system function, LookupAccountName, failed")] public const int CO_E_LOOKUPACCNAMEFAILED =
            unchecked((int) 0x80010132);

        ///<summary>
        ///Unable to set or reset a serialization handle
        ///</summary>
        [Description("Unable to set or reset a serialization handle")] public const int CO_E_SETSERLHNDLFAILED =
            unchecked((int) 0x80010133);

        ///<summary>
        ///Unable to obtain the Windows directory
        ///</summary>
        [Description("Unable to obtain the Windows directory")] public const int CO_E_FAILEDTOGETWINDIR =
            unchecked((int) 0x80010134);

        ///<summary>
        ///Path too long
        ///</summary>
        [Description("Path too long")] public const int CO_E_PATHTOOLONG = unchecked((int) 0x80010135);

        ///<summary>
        ///Unable to generate a uuid.
        ///</summary>
        [Description("Unable to generate a uuid.")] public const int CO_E_FAILEDTOGENUUID = unchecked((int) 0x80010136);

        ///<summary>
        ///Unable to create file
        ///</summary>
        [Description("Unable to create file")] public const int CO_E_FAILEDTOCREATEFILE = unchecked((int) 0x80010137);

        ///<summary>
        ///Unable to close a serialization handle or a file handle.
        ///</summary>
        [Description("Unable to close a serialization handle or a file handle.")] public const int
            CO_E_FAILEDTOCLOSEHANDLE = unchecked((int) 0x80010138);

        ///<summary>
        ///The number of ACEs in an ACL exceeds the system limit.
        ///</summary>
        [Description("The number of ACEs in an ACL exceeds the system limit.")] public const int CO_E_EXCEEDSYSACLLIMIT
            = unchecked((int) 0x80010139);

        ///<summary>
        ///Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.
        ///</summary>
        [Description("Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.")] public const int CO_E_ACESINWRONGORDER = unchecked((int) 0x8001013A);

        ///<summary>
        ///The version of ACL format in the stream is not supported by this implementation of IAccessControl
        ///</summary>
        [Description("The version of ACL format in the stream is not supported by this implementation of IAccessControl"
            )] public const int CO_E_INCOMPATIBLESTREAMVERSION = unchecked((int) 0x8001013B);

        ///<summary>
        ///Unable to open the access token of the server process
        ///</summary>
        [Description("Unable to open the access token of the server process")] public const int
            CO_E_FAILEDTOOPENPROCESSTOKEN = unchecked((int) 0x8001013C);

        ///<summary>
        ///Unable to decode the ACL in the stream provided by the user
        ///</summary>
        [Description("Unable to decode the ACL in the stream provided by the user")] public const int CO_E_DECODEFAILED
            = unchecked((int) 0x8001013D);

        ///<summary>
        ///The COM IAccessControl object is not initialized
        ///</summary>
        [Description("The COM IAccessControl object is not initialized")] public const int CO_E_ACNOTINITIALIZED =
            unchecked((int) 0x8001013F);

        ///<summary>
        ///Call Cancellation is disabled
        ///</summary>
        [Description("Call Cancellation is disabled")] public const int CO_E_CANCEL_DISABLED =
            unchecked((int) 0x80010140);

        ///<summary>
        ///An internal error occurred.
        ///</summary>
        [Description("An internal error occurred.")] public const int RPC_E_UNEXPECTED = unchecked((int) 0x8001FFFF);

        #endregion

        // ******************
        // FACILITY_DISPATCH
        // ******************

        #region (0x020000 - 0x02FFFF) FACILITY_DISPATCH errors

        #region (0x020000 - 0x0200FF) DISP errors

        ///<summary>
        ///Unknown interface.
        ///</summary>
        [Description("Unknown interface.")] public const int DISP_E_UNKNOWNINTERFACE = unchecked((int) 0x80020001);

        ///<summary>
        ///Member not found.
        ///</summary>
        [Description("Member not found.")] public const int DISP_E_MEMBERNOTFOUND = unchecked((int) 0x80020003);

        ///<summary>
        ///Parameter not found.
        ///</summary>
        [Description("Parameter not found.")] public const int DISP_E_PARAMNOTFOUND = unchecked((int) 0x80020004);

        ///<summary>
        ///Type mismatch.
        ///</summary>
        [Description("Type mismatch.")] public const int DISP_E_TYPEMISMATCH = unchecked((int) 0x80020005);

        ///<summary>
        ///Unknown name.
        ///</summary>
        [Description("Unknown name.")] public const int DISP_E_UNKNOWNNAME = unchecked((int) 0x80020006);

        ///<summary>
        ///No named arguments.
        ///</summary>
        [Description("No named arguments.")] public const int DISP_E_NONAMEDARGS = unchecked((int) 0x80020007);

        ///<summary>
        ///Bad variable type.
        ///</summary>
        [Description("Bad variable type.")] public const int DISP_E_BADVARTYPE = unchecked((int) 0x80020008);

        ///<summary>
        ///Exception occurred.
        ///</summary>
        [Description("Exception occurred.")] public const int DISP_E_EXCEPTION = unchecked((int) 0x80020009);

        ///<summary>
        ///Out of present range.
        ///</summary>
        [Description("Out of present range.")] public const int DISP_E_OVERFLOW = unchecked((int) 0x8002000A);

        ///<summary>
        ///Invalid index.
        ///</summary>
        [Description("Invalid index.")] public const int DISP_E_BADINDEX = unchecked((int) 0x8002000B);

        ///<summary>
        ///Unknown language.
        ///</summary>
        [Description("Unknown language.")] public const int DISP_E_UNKNOWNLCID = unchecked((int) 0x8002000C);

        ///<summary>
        ///Memory is locked.
        ///</summary>
        [Description("Memory is locked.")] public const int DISP_E_ARRAYISLOCKED = unchecked((int) 0x8002000D);

        ///<summary>
        ///Invalid number of parameters.
        ///</summary>
        [Description("Invalid number of parameters.")] public const int DISP_E_BADPARAMCOUNT =
            unchecked((int) 0x8002000E);

        ///<summary>
        ///Parameter not optional.
        ///</summary>
        [Description("Parameter not optional.")] public const int DISP_E_PARAMNOTOPTIONAL = unchecked((int) 0x8002000F);

        ///<summary>
        ///Invalid callee.
        ///</summary>
        [Description("Invalid callee.")] public const int DISP_E_BADCALLEE = unchecked((int) 0x80020010);

        ///<summary>
        ///Does not support a collection.
        ///</summary>
        [Description("Does not support a collection.")] public const int DISP_E_NOTACOLLECTION =
            unchecked((int) 0x80020011);

        ///<summary>
        ///Division by zero.
        ///</summary>
        [Description("Division by zero.")] public const int DISP_E_DIVBYZERO = unchecked((int) 0x80020012);

        ///<summary>
        ///Buffer too small
        ///</summary>
        [Description("Buffer too small")] public const int DISP_E_BUFFERTOOSMALL = unchecked((int) 0x80020013);

        #endregion

        #region (0x028000 - 0x029FFF) TYPE errors

        ///<summary>
        ///Buffer too small.
        ///</summary>
        [Description("Buffer too small.")] public const int TYPE_E_BUFFERTOOSMALL = unchecked((int) 0x80028016);

        ///<summary>
        ///Field name not defined in the record.
        ///</summary>
        [Description("Field name not defined in the record.")] public const int TYPE_E_FIELDNOTFOUND =
            unchecked((int) 0x80028017);

        ///<summary>
        ///Old format or invalid type library.
        ///</summary>
        [Description("Old format or invalid type library.")] public const int TYPE_E_INVDATAREAD =
            unchecked((int) 0x80028018);

        ///<summary>
        ///Old format or invalid type library.
        ///</summary>
        [Description("Old format or invalid type library.")] public const int TYPE_E_UNSUPFORMAT =
            unchecked((int) 0x80028019);

        ///<summary>
        ///Error accessing the OLE registry.
        ///</summary>
        [Description("Error accessing the OLE registry.")] public const int TYPE_E_REGISTRYACCESS =
            unchecked((int) 0x8002801C);

        ///<summary>
        ///Library not registered.
        ///</summary>
        [Description("Library not registered.")] public const int TYPE_E_LIBNOTREGISTERED = unchecked((int) 0x8002801D);

        ///<summary>
        ///Bound to unknown type.
        ///</summary>
        [Description("Bound to unknown type.")] public const int TYPE_E_UNDEFINEDTYPE = unchecked((int) 0x80028027);

        ///<summary>
        ///Qualified name disallowed.
        ///</summary>
        [Description("Qualified name disallowed.")] public const int TYPE_E_QUALIFIEDNAMEDISALLOWED =
            unchecked((int) 0x80028028);

        ///<summary>
        ///Invalid forward reference, or reference to uncompiled type.
        ///</summary>
        [Description("Invalid forward reference, or reference to uncompiled type.")] public const int
            TYPE_E_INVALIDSTATE = unchecked((int) 0x80028029);

        ///<summary>
        ///Type mismatch.
        ///</summary>
        [Description("Type mismatch.")] public const int TYPE_E_WRONGTYPEKIND = unchecked((int) 0x8002802A);

        ///<summary>
        ///Element not found.
        ///</summary>
        [Description("Element not found.")] public const int TYPE_E_ELEMENTNOTFOUND = unchecked((int) 0x8002802B);

        ///<summary>
        ///Ambiguous name.
        ///</summary>
        [Description("Ambiguous name.")] public const int TYPE_E_AMBIGUOUSNAME = unchecked((int) 0x8002802C);

        ///<summary>
        ///Name already exists in the library.
        ///</summary>
        [Description("Name already exists in the library.")] public const int TYPE_E_NAMECONFLICT =
            unchecked((int) 0x8002802D);

        ///<summary>
        ///Unknown LCID.
        ///</summary>
        [Description("Unknown LCID.")] public const int TYPE_E_UNKNOWNLCID = unchecked((int) 0x8002802E);

        ///<summary>
        ///Function not defined in specified DLL.
        ///</summary>
        [Description("Function not defined in specified DLL.")] public const int TYPE_E_DLLFUNCTIONNOTFOUND =
            unchecked((int) 0x8002802F);

        ///<summary>
        ///Wrong module kind for the operation.
        ///</summary>
        [Description("Wrong module kind for the operation.")] public const int TYPE_E_BADMODULEKIND =
            unchecked((int) 0x800288BD);

        ///<summary>
        ///Size may not exceed 64K.
        ///</summary>
        [Description("Size may not exceed 64K.")] public const int TYPE_E_SIZETOOBIG = unchecked((int) 0x800288C5);

        ///<summary>
        ///Duplicate ID in inheritance hierarchy.
        ///</summary>
        [Description("Duplicate ID in inheritance hierarchy.")] public const int TYPE_E_DUPLICATEID =
            unchecked((int) 0x800288C6);

        ///<summary>
        ///Incorrect inheritance depth in standard OLE hmember.
        ///</summary>
        [Description("Incorrect inheritance depth in standard OLE hmember.")] public const int TYPE_E_INVALIDID =
            unchecked((int) 0x800288CF);

        ///<summary>
        ///Type mismatch.
        ///</summary>
        [Description("Type mismatch.")] public const int TYPE_E_TYPEMISMATCH = unchecked((int) 0x80028CA0);

        ///<summary>
        ///Invalid number of arguments.
        ///</summary>
        [Description("Invalid number of arguments.")] public const int TYPE_E_OUTOFBOUNDS = unchecked((int) 0x80028CA1);

        ///<summary>
        ///I/O Error.
        ///</summary>
        [Description("I/O Error.")] public const int TYPE_E_IOERROR = unchecked((int) 0x80028CA2);

        ///<summary>
        ///Error creating unique tmp file.
        ///</summary>
        [Description("Error creating unique tmp file.")] public const int TYPE_E_CANTCREATETMPFILE =
            unchecked((int) 0x80028CA3);

        ///<summary>
        ///Error loading type library/DLL.
        ///</summary>
        [Description("Error loading type library/DLL.")] public const int TYPE_E_CANTLOADLIBRARY =
            unchecked((int) 0x80029C4A);

        ///<summary>
        ///Inconsistent property functions.
        ///</summary>
        [Description("Inconsistent property functions.")] public const int TYPE_E_INCONSISTENTPROPFUNCS =
            unchecked((int) 0x80029C83);

        ///<summary>
        ///Circular dependency between types/modules.
        ///</summary>
        [Description("Circular dependency between types/modules.")] public const int TYPE_E_CIRCULARTYPE =
            unchecked((int) 0x80029C84);

        #endregion

        #endregion

        // ******************
        // FACILITY_STORAGE
        // ******************

        #region (0x030000 - 0x03FFFF) FACILITY_STORAGE errors

        ///<summary>
        ///Unable to perform requested operation.
        ///</summary>
        [Description("Unable to perform requested operation.")] public const int STG_E_INVALIDFUNCTION =
            unchecked((int) 0x80030001);

        ///<summary>
        ///%1 could not be found.
        ///</summary>
        [Description("%1 could not be found.")] public const int STG_E_FILENOTFOUND = unchecked((int) 0x80030002);

        ///<summary>
        ///The path %1 could not be found.
        ///</summary>
        [Description("The path %1 could not be found.")] public const int STG_E_PATHNOTFOUND =
            unchecked((int) 0x80030003);

        ///<summary>
        ///There are insufficient resources to open another file.
        ///</summary>
        [Description("There are insufficient resources to open another file.")] public const int STG_E_TOOMANYOPENFILES
            = unchecked((int) 0x80030004);

        ///<summary>
        ///Access Denied.
        ///</summary>
        [Description("Access Denied.")] public const int STG_E_ACCESSDENIED = unchecked((int) 0x80030005);

        ///<summary>
        ///Attempted an operation on an invalid object.
        ///</summary>
        [Description("Attempted an operation on an invalid object.")] public const int STG_E_INVALIDHANDLE =
            unchecked((int) 0x80030006);

        ///<summary>
        ///There is insufficient memory available to complete operation.
        ///</summary>
        [Description("There is insufficient memory available to complete operation.")] public const int
            STG_E_INSUFFICIENTMEMORY = unchecked((int) 0x80030008);

        ///<summary>
        ///Invalid pointer error.
        ///</summary>
        [Description("Invalid pointer error.")] public const int STG_E_INVALIDPOINTER = unchecked((int) 0x80030009);

        ///<summary>
        ///There are no more entries to return.
        ///</summary>
        [Description("There are no more entries to return.")] public const int STG_E_NOMOREFILES =
            unchecked((int) 0x80030012);

        ///<summary>
        ///Disk is write-protected.
        ///</summary>
        [Description("Disk is write-protected.")] public const int STG_E_DISKISWRITEPROTECTED =
            unchecked((int) 0x80030013);

        ///<summary>
        ///An error occurred during a seek operation.
        ///</summary>
        [Description("An error occurred during a seek operation.")] public const int STG_E_SEEKERROR =
            unchecked((int) 0x80030019);

        ///<summary>
        ///A disk error occurred during a write operation.
        ///</summary>
        [Description("A disk error occurred during a write operation.")] public const int STG_E_WRITEFAULT =
            unchecked((int) 0x8003001D);

        ///<summary>
        ///A disk error occurred during a read operation.
        ///</summary>
        [Description("A disk error occurred during a read operation.")] public const int STG_E_READFAULT =
            unchecked((int) 0x8003001E);

        ///<summary>
        ///A share violation has occurred.
        ///</summary>
        [Description("A share violation has occurred.")] public const int STG_E_SHAREVIOLATION =
            unchecked((int) 0x80030020);

        ///<summary>
        ///A lock violation has occurred.
        ///</summary>
        [Description("A lock violation has occurred.")] public const int STG_E_LOCKVIOLATION =
            unchecked((int) 0x80030021);

        ///<summary>
        ///%1 already exists.
        ///</summary>
        [Description("%1 already exists.")] public const int STG_E_FILEALREADYEXISTS = unchecked((int) 0x80030050);

        ///<summary>
        ///Invalid parameter error.
        ///</summary>
        [Description("Invalid parameter error.")] public const int STG_E_INVALIDPARAMETER = unchecked((int) 0x80030057);

        ///<summary>
        ///There is insufficient disk space to complete operation.
        ///</summary>
        [Description("There is insufficient disk space to complete operation.")] public const int STG_E_MEDIUMFULL =
            unchecked((int) 0x80030070);

        ///<summary>
        ///Illegal write of non-simple property to simple property set.
        ///</summary>
        [Description("Illegal write of non-simple property to simple property set.")] public const int
            STG_E_PROPSETMISMATCHED = unchecked((int) 0x800300F0);

        ///<summary>
        ///An API call exited abnormally.
        ///</summary>
        [Description("An API call exited abnormally.")] public const int STG_E_ABNORMALAPIEXIT =
            unchecked((int) 0x800300FA);

        ///<summary>
        ///The file %1 is not a valid compound file.
        ///</summary>
        [Description("The file %1 is not a valid compound file.")] public const int STG_E_INVALIDHEADER =
            unchecked((int) 0x800300FB);

        ///<summary>
        ///The name %1 is not valid.
        ///</summary>
        [Description("The name %1 is not valid.")] public const int STG_E_INVALIDNAME = unchecked((int) 0x800300FC);

        ///<summary>
        ///An unexpected error occurred.
        ///</summary>
        [Description("An unexpected error occurred.")] public const int STG_E_UNKNOWN = unchecked((int) 0x800300FD);

        ///<summary>
        ///That function is not implemented.
        ///</summary>
        [Description("That function is not implemented.")] public const int STG_E_UNIMPLEMENTEDFUNCTION =
            unchecked((int) 0x800300FE);

        ///<summary>
        ///Invalid flag error.
        ///</summary>
        [Description("Invalid flag error.")] public const int STG_E_INVALIDFLAG = unchecked((int) 0x800300FF);

        ///<summary>
        ///Attempted to use an object that is busy.
        ///</summary>
        [Description("Attempted to use an object that is busy.")] public const int STG_E_INUSE =
            unchecked((int) 0x80030100);

        ///<summary>
        ///The storage has been changed since the last commit.
        ///</summary>
        [Description("The storage has been changed since the last commit.")] public const int STG_E_NOTCURRENT =
            unchecked((int) 0x80030101);

        ///<summary>
        ///Attempted to use an object that has ceased to exist.
        ///</summary>
        [Description("Attempted to use an object that has ceased to exist.")] public const int STG_E_REVERTED =
            unchecked((int) 0x80030102);

        ///<summary>
        ///Can't save.
        ///</summary>
        [Description("Can't save.")] public const int STG_E_CANTSAVE = unchecked((int) 0x80030103);

        ///<summary>
        ///The compound file %1 was produced with an incompatible version of storage.
        ///</summary>
        [Description("The compound file %1 was produced with an incompatible version of storage.")] public const int
            STG_E_OLDFORMAT = unchecked((int) 0x80030104);

        ///<summary>
        ///The compound file %1 was produced with a newer version of storage.
        ///</summary>
        [Description("The compound file %1 was produced with a newer version of storage.")] public const int
            STG_E_OLDDLL = unchecked((int) 0x80030105);

        ///<summary>
        ///Share.exe or equivalent is required for operation.
        ///</summary>
        [Description("Share.exe or equivalent is required for operation.")] public const int STG_E_SHAREREQUIRED =
            unchecked((int) 0x80030106);

        ///<summary>
        ///Illegal operation called on non-file based storage.
        ///</summary>
        [Description("Illegal operation called on non-file based storage.")] public const int STG_E_NOTFILEBASEDSTORAGE
            = unchecked((int) 0x80030107);

        ///<summary>
        ///Illegal operation called on object with extant marshallings.
        ///</summary>
        [Description("Illegal operation called on object with extant marshallings.")] public const int
            STG_E_EXTANTMARSHALLINGS = unchecked((int) 0x80030108);

        ///<summary>
        ///The docfile has been corrupted.
        ///</summary>
        [Description("The docfile has been corrupted.")] public const int STG_E_DOCFILECORRUPT =
            unchecked((int) 0x80030109);

        ///<summary>
        ///OLE32.DLL has been loaded at the wrong address.
        ///</summary>
        [Description("OLE32.DLL has been loaded at the wrong address.")] public const int STG_E_BADBASEADDRESS =
            unchecked((int) 0x80030110);

        ///<summary>
        ///The compound file is too large for the current implementation
        ///</summary>
        [Description("The compound file is too large for the current implementation")] public const int
            STG_E_DOCFILETOOLARGE = unchecked((int) 0x80030111);

        ///<summary>
        ///The compound file was not created with the STGM_SIMPLE flag
        ///</summary>
        [Description("The compound file was not created with the STGM_SIMPLE flag")] public const int
            STG_E_NOTSIMPLEFORMAT = unchecked((int) 0x80030112);

        ///<summary>
        ///The file download was aborted abnormally.  The file is incomplete.
        ///</summary>
        [Description("The file download was aborted abnormally.  The file is incomplete.")] public const int
            STG_E_INCOMPLETE = unchecked((int) 0x80030201);

        ///<summary>
        ///The file download has been terminated.
        ///</summary>
        [Description("The file download has been terminated.")] public const int STG_E_TERMINATED =
            unchecked((int) 0x80030202);

        ///<summary>
        ///The underlying file was converted to compound file format.
        ///</summary>
        [Description("The underlying file was converted to compound file format.")] public const int STG_S_CONVERTED =
            unchecked((int) 0x00030200);

        ///<summary>
        ///The storage operation should block until more data is available.
        ///</summary>
        [Description("The storage operation should block until more data is available.")] public const int STG_S_BLOCK =
            unchecked((int) 0x00030201);

        ///<summary>
        ///The storage operation should retry immediately.
        ///</summary>
        [Description("The storage operation should retry immediately.")] public const int STG_S_RETRYNOW =
            unchecked((int) 0x00030202);

        ///<summary>
        ///The notified event sink will not influence the storage operation.
        ///</summary>
        [Description("The notified event sink will not influence the storage operation.")] public const int
            STG_S_MONITORING = unchecked((int) 0x00030203);

        ///<summary>
        ///Multiple opens prevent consolidated. (commit succeeded).
        ///</summary>
        [Description("Multiple opens prevent consolidated. (commit succeeded).")] public const int STG_S_MULTIPLEOPENS =
            unchecked((int) 0x00030204);

        ///<summary>
        ///Consolidation of the storage file failed. (commit succeeded).
        ///</summary>
        [Description("Consolidation of the storage file failed. (commit succeeded).")] public const int
            STG_S_CONSOLIDATIONFAILED = unchecked((int) 0x00030205);

        ///<summary>
        ///Consolidation of the storage file is inappropriate. (commit succeeded).
        ///</summary>
        [Description("Consolidation of the storage file is inappropriate. (commit succeeded).")] public const int
            STG_S_CANNOTCONSOLIDATE = unchecked((int) 0x00030206);

        /*++

     MessageId's 0x0305 - 0x031f (inclusive) are reserved for **STORAGE**
     copy protection errors.

    --*/

        ///<summary>
        ///Generic Copy Protection Error.
        ///</summary>
        [Description("Generic Copy Protection Error.")] public const int STG_E_STATUS_COPY_PROTECTION_FAILURE =
            unchecked((int) 0x80030305);

        ///<summary>
        ///Copy Protection Error - DVD CSS Authentication failed.
        ///</summary>
        [Description("Copy Protection Error - DVD CSS Authentication failed.")] public const int
            STG_E_CSS_AUTHENTICATION_FAILURE = unchecked((int) 0x80030306);

        ///<summary>
        ///Copy Protection Error - The given sector does not have a valid CSS key.
        ///</summary>
        [Description("Copy Protection Error - The given sector does not have a valid CSS key.")] public const int
            STG_E_CSS_KEY_NOT_PRESENT = unchecked((int) 0x80030307);

        ///<summary>
        ///Copy Protection Error - DVD session key not established.
        ///</summary>
        [Description("Copy Protection Error - DVD session key not established.")] public const int
            STG_E_CSS_KEY_NOT_ESTABLISHED = unchecked((int) 0x80030308);

        ///<summary>
        ///Copy Protection Error - The read failed because the sector is encrypted.
        ///</summary>
        [Description("Copy Protection Error - The read failed because the sector is encrypted.")] public const int
            STG_E_CSS_SCRAMBLED_SECTOR = unchecked((int) 0x80030309);

        ///<summary>
        ///Copy Protection Error - The current DVD's region does not correspond to the region setting of the drive.
        ///</summary>
        [Description(
            "Copy Protection Error - The current DVD's region does not correspond to the region setting of the drive.")] public const int STG_E_CSS_REGION_MISMATCH = unchecked((int) 0x8003030A);

        ///<summary>
        ///Copy Protection Error - The drive's region setting may be permanent or the number of user resets has been exhausted.
        ///</summary>
        [Description(
            "Copy Protection Error - The drive's region setting may be permanent or the number of user resets has been exhausted."
            )] public const int STG_E_RESETS_EXHAUSTED = unchecked((int) 0x8003030B);

        /*++

     MessageId's 0x0305 - 0x031f (inclusive) are reserved for **STORAGE**
     copy protection errors.

    --*/

        #endregion

        // ******************
        // FACILITY_ITF
        // ******************

        #region (0x040000 - 0x04FFFF) FACILITY_ITF errors

        #region (0x040000 - 0x0400FF) Old OLE errors

        ///<summary>
        ///Generic OLE errors that may be returned by many inerfaces
        ///</summary>
        [Description("Generic OLE errors that may be returned by many inerfaces")] public const int OLE_E_FIRST =
            unchecked((int) 0x80040000);

        public const int OLE_E_LAST = unchecked((int) 0x800400FF);
        public const int OLE_S_FIRST = unchecked((int) 0x00040000);
        public const int OLE_S_LAST = unchecked((int) 0x000400FF);

        ///<summary>
        ///Invalid OLEVERB structure
        ///</summary>
        [Description("Invalid OLEVERB structure")] public const int OLE_E_OLEVERB = unchecked((int) 0x80040000);

        ///<summary>
        ///Invalid advise flags
        ///</summary>
        [Description("Invalid advise flags")] public const int OLE_E_ADVF = unchecked((int) 0x80040001);

        ///<summary>
        ///Can't enumerate any more, because the associated data is missing
        ///</summary>
        [Description("Can't enumerate any more, because the associated data is missing")] public const int
            OLE_E_ENUM_NOMORE = unchecked((int) 0x80040002);

        ///<summary>
        ///This implementation doesn't take advises
        ///</summary>
        [Description("This implementation doesn't take advises")] public const int OLE_E_ADVISENOTSUPPORTED =
            unchecked((int) 0x80040003);

        ///<summary>
        ///There is no connection for this connection ID
        ///</summary>
        [Description("There is no connection for this connection ID")] public const int OLE_E_NOCONNECTION =
            unchecked((int) 0x80040004);

        ///<summary>
        ///Need to run the object to perform this operation
        ///</summary>
        [Description("Need to run the object to perform this operation")] public const int OLE_E_NOTRUNNING =
            unchecked((int) 0x80040005);

        ///<summary>
        ///There is no cache to operate on
        ///</summary>
        [Description("There is no cache to operate on")] public const int OLE_E_NOCACHE = unchecked((int) 0x80040006);

        ///<summary>
        ///Uninitialized object
        ///</summary>
        [Description("Uninitialized object")] public const int OLE_E_BLANK = unchecked((int) 0x80040007);

        ///<summary>
        ///Linked object's source class has changed
        ///</summary>
        [Description("Linked object's source class has changed")] public const int OLE_E_CLASSDIFF =
            unchecked((int) 0x80040008);

        ///<summary>
        ///Not able to get the moniker of the object
        ///</summary>
        [Description("Not able to get the moniker of the object")] public const int OLE_E_CANT_GETMONIKER =
            unchecked((int) 0x80040009);

        ///<summary>
        ///Not able to bind to the source
        ///</summary>
        [Description("Not able to bind to the source")] public const int OLE_E_CANT_BINDTOSOURCE =
            unchecked((int) 0x8004000A);

        ///<summary>
        ///Object is static; operation not allowed
        ///</summary>
        [Description("Object is static; operation not allowed")] public const int OLE_E_STATIC =
            unchecked((int) 0x8004000B);

        ///<summary>
        ///User canceled out of save dialog
        ///</summary>
        [Description("User canceled out of save dialog")] public const int OLE_E_PROMPTSAVECANCELLED =
            unchecked((int) 0x8004000C);

        ///<summary>
        ///Invalid rectangle
        ///</summary>
        [Description("Invalid rectangle")] public const int OLE_E_INVALIDRECT = unchecked((int) 0x8004000D);

        ///<summary>
        ///compobj.dll is too old for the ole2.dll initialized
        ///</summary>
        [Description("compobj.dll is too old for the ole2.dll initialized")] public const int OLE_E_WRONGCOMPOBJ =
            unchecked((int) 0x8004000E);

        ///<summary>
        ///Invalid window handle
        ///</summary>
        [Description("Invalid window handle")] public const int OLE_E_INVALIDHWND = unchecked((int) 0x8004000F);

        ///<summary>
        ///Object is not in any of the inplace active states
        ///</summary>
        [Description("Object is not in any of the inplace active states")] public const int OLE_E_NOT_INPLACEACTIVE =
            unchecked((int) 0x80040010);

        ///<summary>
        ///Not able to convert object
        ///</summary>
        [Description("Not able to convert object")] public const int OLE_E_CANTCONVERT = unchecked((int) 0x80040011);

        ///<summary>
        ///Not able to perform the operation because object is not given storage yet
        ///</summary>
        [Description("Not able to perform the operation because object is not given storage yet")] public const int
            OLE_E_NOSTORAGE = unchecked((int) 0x80040012);

        ///<summary>
        ///Invalid FORMATETC structure
        ///</summary>
        [Description("Invalid FORMATETC structure")] public const int DV_E_FORMATETC = unchecked((int) 0x80040064);

        ///<summary>
        ///Invalid DVTARGETDEVICE structure
        ///</summary>
        [Description("Invalid DVTARGETDEVICE structure")] public const int DV_E_DVTARGETDEVICE =
            unchecked((int) 0x80040065);

        ///<summary>
        ///Invalid STDGMEDIUM structure
        ///</summary>
        [Description("Invalid STDGMEDIUM structure")] public const int DV_E_STGMEDIUM = unchecked((int) 0x80040066);

        ///<summary>
        ///Invalid STATDATA structure
        ///</summary>
        [Description("Invalid STATDATA structure")] public const int DV_E_STATDATA = unchecked((int) 0x80040067);

        ///<summary>
        ///Invalid lindex
        ///</summary>
        [Description("Invalid lindex")] public const int DV_E_LINDEX = unchecked((int) 0x80040068);

        ///<summary>
        ///Invalid tymed
        ///</summary>
        [Description("Invalid tymed")] public const int DV_E_TYMED = unchecked((int) 0x80040069);

        ///<summary>
        ///Invalid clipboard format
        ///</summary>
        [Description("Invalid clipboard format")] public const int DV_E_CLIPFORMAT = unchecked((int) 0x8004006A);

        ///<summary>
        ///Invalid aspect(s)
        ///</summary>
        [Description("Invalid aspect(s)")] public const int DV_E_DVASPECT = unchecked((int) 0x8004006B);

        ///<summary>
        ///tdSize parameter of the DVTARGETDEVICE structure is invalid
        ///</summary>
        [Description("tdSize parameter of the DVTARGETDEVICE structure is invalid")] public const int
            DV_E_DVTARGETDEVICE_SIZE = unchecked((int) 0x8004006C);

        ///<summary>
        ///Object doesn't support IViewObject interface
        ///</summary>
        [Description("Object doesn't support IViewObject interface")] public const int DV_E_NOIVIEWOBJECT =
            unchecked((int) 0x8004006D);

        #endregion

        #region (0x040100 - 0x04010F) DRAGDROP errors

        public const int DRAGDROP_E_FIRST = unchecked((int) 0x80040100);
        public const int DRAGDROP_E_LAST = unchecked((int) 0x8004010F);
        public const int DRAGDROP_S_FIRST = unchecked((int) 0x00040100);
        public const int DRAGDROP_S_LAST = unchecked((int) 0x0004010F);

        ///<summary>
        ///Trying to revoke a drop target that has not been registered
        ///</summary>
        [Description("Trying to revoke a drop target that has not been registered")] public const int
            DRAGDROP_E_NOTREGISTERED = unchecked((int) 0x80040100);

        ///<summary>
        ///This window has already been registered as a drop target
        ///</summary>
        [Description("This window has already been registered as a drop target")] public const int
            DRAGDROP_E_ALREADYREGISTERED = unchecked((int) 0x80040101);

        ///<summary>
        ///Invalid window handle
        ///</summary>
        [Description("Invalid window handle")] public const int DRAGDROP_E_INVALIDHWND = unchecked((int) 0x80040102);

        #endregion

        #region (0x040110 - 0x04011F) CLASS errors

        public const int CLASSFACTORY_E_FIRST = unchecked((int) 0x80040110);
        public const int CLASSFACTORY_E_LAST = unchecked((int) 0x8004011F);
        public const int CLASSFACTORY_S_FIRST = unchecked((int) 0x00040110);
        public const int CLASSFACTORY_S_LAST = unchecked((int) 0x0004011F);

        ///<summary>
        ///Class does not support aggregation (or class object is remote)
        ///</summary>
        [Description("Class does not support aggregation (or class object is remote)")] public const int
            CLASS_E_NOAGGREGATION = unchecked((int) 0x80040110);

        ///<summary>
        ///ClassFactory cannot supply requested class
        ///</summary>
        [Description("ClassFactory cannot supply requested class")] public const int CLASS_E_CLASSNOTAVAILABLE =
            unchecked((int) 0x80040111);

        ///<summary>
        ///Class is not licensed for use
        ///</summary>
        [Description("Class is not licensed for use")] public const int CLASS_E_NOTLICENSED =
            unchecked((int) 0x80040112);

        #endregion

        #region (0x040120 - 0x04012F) MARSHAL errors

        public const int MARSHAL_E_FIRST = unchecked((int) 0x80040120);
        public const int MARSHAL_E_LAST = unchecked((int) 0x8004012F);
        public const int MARSHAL_S_FIRST = unchecked((int) 0x00040120);
        public const int MARSHAL_S_LAST = unchecked((int) 0x0004012F);

        #endregion

        #region (0x040130 - 0x04013F) DATA errors

        public const int DATA_E_FIRST = unchecked((int) 0x80040130);
        public const int DATA_E_LAST = unchecked((int) 0x8004013F);
        public const int DATA_S_FIRST = unchecked((int) 0x00040130);
        public const int DATA_S_LAST = unchecked((int) 0x0004013F);

        #endregion

        #region (0x040140 - 0x04014F) VIEW errors

        public const int VIEW_E_FIRST = unchecked((int) 0x80040140);
        public const int VIEW_E_LAST = unchecked((int) 0x8004014F);
        public const int VIEW_S_FIRST = unchecked((int) 0x00040140);
        public const int VIEW_S_LAST = unchecked((int) 0x0004014F);

        ///<summary>
        ///Error drawing view
        ///</summary>
        [Description("Error drawing view")] public const int VIEW_E_DRAW = unchecked((int) 0x80040140);

        #endregion

        #region (0x040150 - 0x04015F) REGDB errors

        public const int REGDB_E_FIRST = unchecked((int) 0x80040150);
        public const int REGDB_E_LAST = unchecked((int) 0x8004015F);
        public const int REGDB_S_FIRST = unchecked((int) 0x00040150);
        public const int REGDB_S_LAST = unchecked((int) 0x0004015F);

        ///<summary>
        ///Could not read key from registry
        ///</summary>
        [Description("Could not read key from registry")] public const int REGDB_E_READREGDB =
            unchecked((int) 0x80040150);

        ///<summary>
        ///Could not write key to registry
        ///</summary>
        [Description("Could not write key to registry")] public const int REGDB_E_WRITEREGDB =
            unchecked((int) 0x80040151);

        ///<summary>
        ///Could not find the key in the registry
        ///</summary>
        [Description("Could not find the key in the registry")] public const int REGDB_E_KEYMISSING =
            unchecked((int) 0x80040152);

        ///<summary>
        ///Invalid value for registry
        ///</summary>
        [Description("Invalid value for registry")] public const int REGDB_E_INVALIDVALUE = unchecked((int) 0x80040153);

        ///<summary>
        ///Class not registered
        ///</summary>
        [Description("Class not registered")] public const int REGDB_E_CLASSNOTREG = unchecked((int) 0x80040154);

        ///<summary>
        ///Interface not registered
        ///</summary>
        [Description("Interface not registered")] public const int REGDB_E_IIDNOTREG = unchecked((int) 0x80040155);

        ///<summary>
        ///Threading model entry is not valid
        ///</summary>
        [Description("Threading model entry is not valid")] public const int REGDB_E_BADTHREADINGMODEL =
            unchecked((int) 0x80040156);

        #endregion

        #region (0x040160 - 0x040161) CAT errors

        public const int CAT_E_FIRST = unchecked((int) 0x80040160);
        public const int CAT_E_LAST = unchecked((int) 0x80040161);

        ///<summary>
        ///CATID does not exist
        ///</summary>
        [Description("CATID does not exist")] public const int CAT_E_CATIDNOEXIST = unchecked((int) 0x80040160);

        ///<summary>
        ///Description not found
        ///</summary>
        [Description("Description not found")] public const int CAT_E_NODESCRIPTION = unchecked((int) 0x80040161);

        #endregion

        #region (0x040164 - 0x04016F) Class Store Error Codes

        public const int CS_E_FIRST = unchecked((int) 0x80040164);
        public const int CS_E_LAST = unchecked((int) 0x8004016F);

        ///<summary>
        ///No package in the software installation data in the Active Directory meets this criteria.
        ///</summary>
        [Description("No package in the software installation data in the Active Directory meets this criteria.")] public const int CS_E_PACKAGE_NOTFOUND = unchecked((int) 0x80040164);

        ///<summary>
        ///Deleting this will break the referential integrity of the software installation data in the Active Directory.
        ///</summary>
        [Description(
            "Deleting this will break the referential integrity of the software installation data in the Active Directory."
            )] public const int CS_E_NOT_DELETABLE = unchecked((int) 0x80040165);

        ///<summary>
        ///The CLSID was not found in the software installation data in the Active Directory.
        ///</summary>
        [Description("The CLSID was not found in the software installation data in the Active Directory.")] public const
            int CS_E_CLASS_NOTFOUND = unchecked((int) 0x80040166);

        ///<summary>
        ///The software installation data in the Active Directory is corrupt.
        ///</summary>
        [Description("The software installation data in the Active Directory is corrupt.")] public const int
            CS_E_INVALID_VERSION = unchecked((int) 0x80040167);

        ///<summary>
        ///There is no software installation data in the Active Directory.
        ///</summary>
        [Description("There is no software installation data in the Active Directory.")] public const int
            CS_E_NO_CLASSSTORE = unchecked((int) 0x80040168);

        ///<summary>
        ///There is no software installation data object in the Active Directory.
        ///</summary>
        [Description("There is no software installation data object in the Active Directory.")] public const int
            CS_E_OBJECT_NOTFOUND = unchecked((int) 0x80040169);

        ///<summary>
        ///The software installation data object in the Active Directory already exists.
        ///</summary>
        [Description("The software installation data object in the Active Directory already exists.")] public const int
            CS_E_OBJECT_ALREADY_EXISTS = unchecked((int) 0x8004016A);

        ///<summary>
        ///The path to the software installation data in the Active Directory is not correct.
        ///</summary>
        [Description("The path to the software installation data in the Active Directory is not correct.")] public const
            int CS_E_INVALID_PATH = unchecked((int) 0x8004016B);

        ///<summary>
        ///A network error interrupted the operation.
        ///</summary>
        [Description("A network error interrupted the operation.")] public const int CS_E_NETWORK_ERROR =
            unchecked((int) 0x8004016C);

        ///<summary>
        ///The size of this object exceeds the maximum size set by the Administrator.
        ///</summary>
        [Description("The size of this object exceeds the maximum size set by the Administrator.")] public const int
            CS_E_ADMIN_LIMIT_EXCEEDED = unchecked((int) 0x8004016D);

        ///<summary>
        ///The schema for the software installation data in the Active Directory does not match the required schema.
        ///</summary>
        [Description(
            "The schema for the software installation data in the Active Directory does not match the required schema.")
        ] public const int CS_E_SCHEMA_MISMATCH = unchecked((int) 0x8004016E);

        ///<summary>
        ///An error occurred in the software installation data in the Active Directory.
        ///</summary>
        [Description("An error occurred in the software installation data in the Active Directory.")] public const int
            CS_E_INTERNAL_ERROR = unchecked((int) 0x8004016F);

        #endregion

        #region (0x040170 - 0x04017F) CACHE errors

        public const int CACHE_E_FIRST = unchecked((int) 0x80040170);
        public const int CACHE_E_LAST = unchecked((int) 0x8004017F);
        public const int CACHE_S_FIRST = unchecked((int) 0x00040170);
        public const int CACHE_S_LAST = unchecked((int) 0x0004017F);

        ///<summary>
        ///Cache not updated
        ///</summary>
        [Description("Cache not updated")] public const int CACHE_E_NOCACHE_UPDATED = unchecked((int) 0x80040170);

        #endregion

        #region (0x040180 - 0x04018F) OLEOBJ errors

        public const int OLEOBJ_E_FIRST = unchecked((int) 0x80040180);
        public const int OLEOBJ_E_LAST = unchecked((int) 0x8004018F);
        public const int OLEOBJ_S_FIRST = unchecked((int) 0x00040180);
        public const int OLEOBJ_S_LAST = unchecked((int) 0x0004018F);

        ///<summary>
        ///No verbs for OLE object
        ///</summary>
        [Description("No verbs for OLE object")] public const int OLEOBJ_E_NOVERBS = unchecked((int) 0x80040180);

        ///<summary>
        ///Invalid verb for OLE object
        ///</summary>
        [Description("Invalid verb for OLE object")] public const int OLEOBJ_E_INVALIDVERB = unchecked((int) 0x80040181);

        #endregion

        #region (0x040190 - 0x04019F) CLIENTSITE errors

        public const int CLIENTSITE_E_FIRST = unchecked((int) 0x80040190);
        public const int CLIENTSITE_E_LAST = unchecked((int) 0x8004019F);
        public const int CLIENTSITE_S_FIRST = unchecked((int) 0x00040190);
        public const int CLIENTSITE_S_LAST = unchecked((int) 0x0004019F);

        #endregion

        #region (0x0401A0 - 0x0401AF) INPLACE errors

        public const int INPLACE_E_FIRST = unchecked((int) 0x800401A0);
        public const int INPLACE_E_LAST = unchecked((int) 0x800401AF);
        public const int INPLACE_S_FIRST = unchecked((int) 0x000401A0);
        public const int INPLACE_S_LAST = unchecked((int) 0x000401AF);

        ///<summary>
        ///Undo is not available
        ///</summary>
        [Description("Undo is not available")] public const int INPLACE_E_NOTUNDOABLE = unchecked((int) 0x800401A0);

        ///<summary>
        ///Space for tools is not available
        ///</summary>
        [Description("Space for tools is not available")] public const int INPLACE_E_NOTOOLSPACE =
            unchecked((int) 0x800401A1);

        #endregion

        #region (0x0401B0 - 0x0401BF) ENUM errors

        public const int ENUM_E_FIRST = unchecked((int) 0x800401B0);
        public const int ENUM_E_LAST = unchecked((int) 0x800401BF);
        public const int ENUM_S_FIRST = unchecked((int) 0x000401B0);
        public const int ENUM_S_LAST = unchecked((int) 0x000401BF);

        #endregion

        #region (0x0401C0 - 0x0401CF) CONVERT10 errors

        public const int CONVERT10_E_FIRST = unchecked((int) 0x800401C0);
        public const int CONVERT10_E_LAST = unchecked((int) 0x800401CF);
        public const int CONVERT10_S_FIRST = unchecked((int) 0x000401C0);
        public const int CONVERT10_S_LAST = unchecked((int) 0x000401CF);

        ///<summary>
        ///OLESTREAM Get method failed
        ///</summary>
        [Description("OLESTREAM Get method failed")] public const int CONVERT10_E_OLESTREAM_GET =
            unchecked((int) 0x800401C0);

        ///<summary>
        ///OLESTREAM Put method failed
        ///</summary>
        [Description("OLESTREAM Put method failed")] public const int CONVERT10_E_OLESTREAM_PUT =
            unchecked((int) 0x800401C1);

        ///<summary>
        ///Contents of the OLESTREAM not in correct format
        ///</summary>
        [Description("Contents of the OLESTREAM not in correct format")] public const int CONVERT10_E_OLESTREAM_FMT =
            unchecked((int) 0x800401C2);

        ///<summary>
        ///There was an error in a Windows GDI call while converting the bitmap to a DIB
        ///</summary>
        [Description("There was an error in a Windows GDI call while converting the bitmap to a DIB")] public const int
            CONVERT10_E_OLESTREAM_BITMAP_TO_DIB = unchecked((int) 0x800401C3);

        ///<summary>
        ///Contents of the IStorage not in correct format
        ///</summary>
        [Description("Contents of the IStorage not in correct format")] public const int CONVERT10_E_STG_FMT =
            unchecked((int) 0x800401C4);

        ///<summary>
        ///Contents of IStorage is missing one of the standard streams
        ///</summary>
        [Description("Contents of IStorage is missing one of the standard streams")] public const int
            CONVERT10_E_STG_NO_STD_STREAM = unchecked((int) 0x800401C5);

        ///<summary>
        ///There was an error in a Windows GDI call while converting the DIB to a bitmap.
        ///</summary>
        [Description("There was an error in a Windows GDI call while converting the DIB to a bitmap.")] public const int
            CONVERT10_E_STG_DIB_TO_BITMAP = unchecked((int) 0x800401C6);

        #endregion

        #region (0x0401D0 - 0x0401DF) CLIPBRD errors

        public const int CLIPBRD_E_FIRST = unchecked((int) 0x800401D0);
        public const int CLIPBRD_E_LAST = unchecked((int) 0x800401DF);
        public const int CLIPBRD_S_FIRST = unchecked((int) 0x000401D0);
        public const int CLIPBRD_S_LAST = unchecked((int) 0x000401DF);

        ///<summary>
        ///OpenClipboard Failed
        ///</summary>
        [Description("OpenClipboard Failed")] public const int CLIPBRD_E_CANT_OPEN = unchecked((int) 0x800401D0);

        ///<summary>
        ///EmptyClipboard Failed
        ///</summary>
        [Description("EmptyClipboard Failed")] public const int CLIPBRD_E_CANT_EMPTY = unchecked((int) 0x800401D1);

        ///<summary>
        ///SetClipboard Failed
        ///</summary>
        [Description("SetClipboard Failed")] public const int CLIPBRD_E_CANT_SET = unchecked((int) 0x800401D2);

        ///<summary>
        ///Data on clipboard is invalid
        ///</summary>
        [Description("Data on clipboard is invalid")] public const int CLIPBRD_E_BAD_DATA = unchecked((int) 0x800401D3);

        ///<summary>
        ///CloseClipboard Failed
        ///</summary>
        [Description("CloseClipboard Failed")] public const int CLIPBRD_E_CANT_CLOSE = unchecked((int) 0x800401D4);

        #endregion

        #region (0x0401E0 - 0x0401EF) MK (moniker) errors

        public const int MK_E_FIRST = unchecked((int) 0x800401E0);
        public const int MK_E_LAST = unchecked((int) 0x800401EF);
        public const int MK_S_FIRST = unchecked((int) 0x000401E0);
        public const int MK_S_LAST = unchecked((int) 0x000401EF);

        ///<summary>
        ///Moniker needs to be connected manually
        ///</summary>
        [Description("Moniker needs to be connected manually")] public const int MK_E_CONNECTMANUALLY =
            unchecked((int) 0x800401E0);

        ///<summary>
        ///Operation exceeded deadline
        ///</summary>
        [Description("Operation exceeded deadline")] public const int MK_E_EXCEEDEDDEADLINE =
            unchecked((int) 0x800401E1);

        ///<summary>
        ///Moniker needs to be generic
        ///</summary>
        [Description("Moniker needs to be generic")] public const int MK_E_NEEDGENERIC = unchecked((int) 0x800401E2);

        ///<summary>
        ///Operation unavailable
        ///</summary>
        [Description("Operation unavailable")] public const int MK_E_UNAVAILABLE = unchecked((int) 0x800401E3);

        ///<summary>
        ///Invalid syntax
        ///</summary>
        [Description("Invalid syntax")] public const int MK_E_SYNTAX = unchecked((int) 0x800401E4);

        ///<summary>
        ///No object for moniker
        ///</summary>
        [Description("No object for moniker")] public const int MK_E_NOOBJECT = unchecked((int) 0x800401E5);

        ///<summary>
        ///Bad extension for file
        ///</summary>
        [Description("Bad extension for file")] public const int MK_E_INVALIDEXTENSION = unchecked((int) 0x800401E6);

        ///<summary>
        ///Intermediate operation failed
        ///</summary>
        [Description("Intermediate operation failed")] public const int MK_E_INTERMEDIATEINTERFACENOTSUPPORTED =
            unchecked((int) 0x800401E7);

        ///<summary>
        ///Moniker is not bindable
        ///</summary>
        [Description("Moniker is not bindable")] public const int MK_E_NOTBINDABLE = unchecked((int) 0x800401E8);

        ///<summary>
        ///Moniker is not bound
        ///</summary>
        [Description("Moniker is not bound")] public const int MK_E_NOTBOUND = unchecked((int) 0x800401E9);

        ///<summary>
        ///Moniker cannot open file
        ///</summary>
        [Description("Moniker cannot open file")] public const int MK_E_CANTOPENFILE = unchecked((int) 0x800401EA);

        ///<summary>
        ///User input required for operation to succeed
        ///</summary>
        [Description("User input required for operation to succeed")] public const int MK_E_MUSTBOTHERUSER =
            unchecked((int) 0x800401EB);

        ///<summary>
        ///Moniker class has no inverse
        ///</summary>
        [Description("Moniker class has no inverse")] public const int MK_E_NOINVERSE = unchecked((int) 0x800401EC);

        ///<summary>
        ///Moniker does not refer to storage
        ///</summary>
        [Description("Moniker does not refer to storage")] public const int MK_E_NOSTORAGE = unchecked((int) 0x800401ED);

        ///<summary>
        ///No common prefix
        ///</summary>
        [Description("No common prefix")] public const int MK_E_NOPREFIX = unchecked((int) 0x800401EE);

        ///<summary>
        ///Moniker could not be enumerated
        ///</summary>
        [Description("Moniker could not be enumerated")] public const int MK_E_ENUMERATION_FAILED =
            unchecked((int) 0x800401EF);

        #endregion

        #region (0x0401F0 - 0x0401FF) CO errors

        public const int CO_E_FIRST = unchecked((int) 0x800401F0);
        public const int CO_E_LAST = unchecked((int) 0x800401FF);
        public const int CO_S_FIRST = unchecked((int) 0x000401F0);
        public const int CO_S_LAST = unchecked((int) 0x000401FF);

        ///<summary>
        ///CoInitialize has not been called.
        ///</summary>
        [Description("CoInitialize has not been called.")] public const int CO_E_NOTINITIALIZED =
            unchecked((int) 0x800401F0);

        ///<summary>
        ///CoInitialize has already been called.
        ///</summary>
        [Description("CoInitialize has already been called.")] public const int CO_E_ALREADYINITIALIZED =
            unchecked((int) 0x800401F1);

        ///<summary>
        ///Class of object cannot be determined
        ///</summary>
        [Description("Class of object cannot be determined")] public const int CO_E_CANTDETERMINECLASS =
            unchecked((int) 0x800401F2);

        ///<summary>
        ///Invalid class string
        ///</summary>
        [Description("Invalid class string")] public const int CO_E_CLASSSTRING = unchecked((int) 0x800401F3);

        ///<summary>
        ///Invalid interface string
        ///</summary>
        [Description("Invalid interface string")] public const int CO_E_IIDSTRING = unchecked((int) 0x800401F4);

        ///<summary>
        ///Application not found
        ///</summary>
        [Description("Application not found")] public const int CO_E_APPNOTFOUND = unchecked((int) 0x800401F5);

        ///<summary>
        ///Application cannot be run more than once
        ///</summary>
        [Description("Application cannot be run more than once")] public const int CO_E_APPSINGLEUSE =
            unchecked((int) 0x800401F6);

        ///<summary>
        ///Some error in application program
        ///</summary>
        [Description("Some error in application program")] public const int CO_E_ERRORINAPP =
            unchecked((int) 0x800401F7);

        ///<summary>
        ///DLL for class not found
        ///</summary>
        [Description("DLL for class not found")] public const int CO_E_DLLNOTFOUND = unchecked((int) 0x800401F8);

        ///<summary>
        ///Error in the DLL
        ///</summary>
        [Description("Error in the DLL")] public const int CO_E_ERRORINDLL = unchecked((int) 0x800401F9);

        ///<summary>
        ///Wrong OS or OS version for application
        ///</summary>
        [Description("Wrong OS or OS version for application")] public const int CO_E_WRONGOSFORAPP =
            unchecked((int) 0x800401FA);

        ///<summary>
        ///Object is not registered
        ///</summary>
        [Description("Object is not registered")] public const int CO_E_OBJNOTREG = unchecked((int) 0x800401FB);

        ///<summary>
        ///Object is already registered
        ///</summary>
        [Description("Object is already registered")] public const int CO_E_OBJISREG = unchecked((int) 0x800401FC);

        ///<summary>
        ///Object is not connected to server
        ///</summary>
        [Description("Object is not connected to server")] public const int CO_E_OBJNOTCONNECTED =
            unchecked((int) 0x800401FD);

        ///<summary>
        ///Application was launched but it didn't register a class factory
        ///</summary>
        [Description("Application was launched but it didn't register a class factory")] public const int
            CO_E_APPDIDNTREG = unchecked((int) 0x800401FE);

        ///<summary>
        ///Object has been released
        ///</summary>
        [Description("Object has been released")] public const int CO_E_RELEASED = unchecked((int) 0x800401FF);

        #endregion

        #region (0x040200 - 0x04020F) EVENT errors

        public const int EVENT_E_FIRST = unchecked((int) 0x80040200);
        public const int EVENT_E_LAST = unchecked((int) 0x8004021F);
        public const int EVENT_S_FIRST = unchecked((int) 0x00040200);
        public const int EVENT_S_LAST = unchecked((int) 0x0004021F);

        ///<summary>
        ///An event was able to invoke some but not all of the subscribers
        ///</summary>
        [Description("An event was able to invoke some but not all of the subscribers")] public const int
            EVENT_S_SOME_SUBSCRIBERS_FAILED = unchecked((int) 0x00040200);

        ///<summary>
        ///An event was unable to invoke any of the subscribers
        ///</summary>
        [Description("An event was unable to invoke any of the subscribers")] public const int
            EVENT_E_ALL_SUBSCRIBERS_FAILED = unchecked((int) 0x80040201);

        ///<summary>
        ///An event was delivered but there were no subscribers
        ///</summary>
        [Description("An event was delivered but there were no subscribers")] public const int EVENT_S_NOSUBSCRIBERS =
            unchecked((int) 0x00040202);

        ///<summary>
        ///A syntax error occurred trying to evaluate a query string
        ///</summary>
        [Description("A syntax error occurred trying to evaluate a query string")] public const int EVENT_E_QUERYSYNTAX
            = unchecked((int) 0x80040203);

        ///<summary>
        ///An invalid field name was used in a query string
        ///</summary>
        [Description("An invalid field name was used in a query string")] public const int EVENT_E_QUERYFIELD =
            unchecked((int) 0x80040204);

        ///<summary>
        ///An unexpected exception was raised
        ///</summary>
        [Description("An unexpected exception was raised")] public const int EVENT_E_INTERNALEXCEPTION =
            unchecked((int) 0x80040205);

        ///<summary>
        ///An unexpected internal error was detected
        ///</summary>
        [Description("An unexpected internal error was detected")] public const int EVENT_E_INTERNALERROR =
            unchecked((int) 0x80040206);

        ///<summary>
        ///The owner SID on a per-user subscription doesn't exist
        ///</summary>
        [Description("The owner SID on a per-user subscription doesn't exist")] public const int
            EVENT_E_INVALID_PER_USER_SID = unchecked((int) 0x80040207);

        ///<summary>
        ///A user-supplied component or subscriber raised an exception
        ///</summary>
        [Description("A user-supplied component or subscriber raised an exception")] public const int
            EVENT_E_USER_EXCEPTION = unchecked((int) 0x80040208);

        ///<summary>
        ///An interface has too many methods to fire events from
        ///</summary>
        [Description("An interface has too many methods to fire events from")] public const int EVENT_E_TOO_MANY_METHODS
            = unchecked((int) 0x80040209);

        ///<summary>
        ///A subscription cannot be stored unless its event class already exists
        ///</summary>
        [Description("A subscription cannot be stored unless its event class already exists")] public const int
            EVENT_E_MISSING_EVENTCLASS = unchecked((int) 0x8004020A);

        ///<summary>
        ///Not all the objects requested could be removed
        ///</summary>
        [Description("Not all the objects requested could be removed")] public const int EVENT_E_NOT_ALL_REMOVED =
            unchecked((int) 0x8004020B);

        ///<summary>
        ///COM+ is required for this operation, but is not installed
        ///</summary>
        [Description("COM+ is required for this operation, but is not installed")] public const int
            EVENT_E_COMPLUS_NOT_INSTALLED = unchecked((int) 0x8004020C);

        ///<summary>
        ///Cannot modify or delete an object that was not added using the COM+ Admin SDK
        ///</summary>
        [Description("Cannot modify or delete an object that was not added using the COM+ Admin SDK")] public const int
            EVENT_E_CANT_MODIFY_OR_DELETE_UNCONFIGURED_OBJECT = unchecked((int) 0x8004020D);

        ///<summary>
        ///Cannot modify or delete an object that was added using the COM+ Admin SDK
        ///</summary>
        [Description("Cannot modify or delete an object that was added using the COM+ Admin SDK")] public const int
            EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT = unchecked((int) 0x8004020E);

        ///<summary>
        ///The event class for this subscription is in an invalid partition
        ///</summary>
        [Description("The event class for this subscription is in an invalid partition")] public const int
            EVENT_E_INVALID_EVENT_CLASS_PARTITION = unchecked((int) 0x8004020F);

        ///<summary>
        ///The owner of the PerUser subscription is not logged on to the system specified
        ///</summary>
        [Description("The owner of the PerUser subscription is not logged on to the system specified")] public const int
            EVENT_E_PER_USER_SID_NOT_LOGGED_ON = unchecked((int) 0x80040210);

        #endregion

        #region (0x04D000 - 0x04D029) XACT errors

        public const int XACT_E_FIRST = unchecked((int) 0x8004D000);
        public const int XACT_E_LAST = unchecked((int) 0x8004D029);
        public const int XACT_S_FIRST = unchecked((int) 0x0004D000);
        public const int XACT_S_LAST = unchecked((int) 0x0004D010);

        ///<summary>
        ///Another single phase resource manager has already been enlisted in this transaction.
        ///</summary>
        [Description("Another single phase resource manager has already been enlisted in this transaction.")] public
            const int XACT_E_ALREADYOTHERSINGLEPHASE = unchecked((int) 0x8004D000);

        ///<summary>
        ///A retaining commit or abort is not supported
        ///</summary>
        [Description("A retaining commit or abort is not supported")] public const int XACT_E_CANTRETAIN =
            unchecked((int) 0x8004D001);

        ///<summary>
        ///The transaction failed to commit for an unknown reason. The transaction was aborted.
        ///</summary>
        [Description("The transaction failed to commit for an unknown reason. The transaction was aborted.")] public
            const int XACT_E_COMMITFAILED = unchecked((int) 0x8004D002);

        ///<summary>
        ///Cannot call commit on this transaction object because the calling application did not initiate the transaction.
        ///</summary>
        [Description(
            "Cannot call commit on this transaction object because the calling application did not initiate the transaction."
            )] public const int XACT_E_COMMITPREVENTED = unchecked((int) 0x8004D003);

        ///<summary>
        ///Instead of committing, the resource heuristically aborted.
        ///</summary>
        [Description("Instead of committing, the resource heuristically aborted.")] public const int
            XACT_E_HEURISTICABORT = unchecked((int) 0x8004D004);

        ///<summary>
        ///Instead of aborting, the resource heuristically committed.
        ///</summary>
        [Description("Instead of aborting, the resource heuristically committed.")] public const int
            XACT_E_HEURISTICCOMMIT = unchecked((int) 0x8004D005);

        ///<summary>
        ///Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions.
        ///</summary>
        [Description(
            "Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions."
            )] public const int XACT_E_HEURISTICDAMAGE = unchecked((int) 0x8004D006);

        ///<summary>
        ///Some of the states of the resource may have been committed while others may have been aborted, likely because of heuristic decisions.
        ///</summary>
        [Description(
            "Some of the states of the resource may have been committed while others may have been aborted, likely because of heuristic decisions."
            )] public const int XACT_E_HEURISTICDANGER = unchecked((int) 0x8004D007);

        ///<summary>
        ///The requested isolation level is not valid or supported.
        ///</summary>
        [Description("The requested isolation level is not valid or supported.")] public const int XACT_E_ISOLATIONLEVEL
            = unchecked((int) 0x8004D008);

        ///<summary>
        ///The transaction manager doesn't support an asynchronous operation for this method.
        ///</summary>
        [Description("The transaction manager doesn't support an asynchronous operation for this method.")] public const
            int XACT_E_NOASYNC = unchecked((int) 0x8004D009);

        ///<summary>
        ///Unable to enlist in the transaction.
        ///</summary>
        [Description("Unable to enlist in the transaction.")] public const int XACT_E_NOENLIST =
            unchecked((int) 0x8004D00A);

        ///<summary>
        ///The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to zero.
        ///</summary>
        [Description(
            "The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to zero."
            )] public const int XACT_E_NOISORETAIN = unchecked((int) 0x8004D00B);

        ///<summary>
        ///There is no resource presently associated with this enlistment
        ///</summary>
        [Description("There is no resource presently associated with this enlistment")] public const int
            XACT_E_NORESOURCE = unchecked((int) 0x8004D00C);

        ///<summary>
        ///The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers.
        ///</summary>
        [Description(
            "The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers."
            )] public const int XACT_E_NOTCURRENT = unchecked((int) 0x8004D00D);

        ///<summary>
        ///The transaction has already been implicitly or explicitly committed or aborted
        ///</summary>
        [Description("The transaction has already been implicitly or explicitly committed or aborted")] public const int
            XACT_E_NOTRANSACTION = unchecked((int) 0x8004D00E);

        ///<summary>
        ///An invalid combination of flags was specified
        ///</summary>
        [Description("An invalid combination of flags was specified")] public const int XACT_E_NOTSUPPORTED =
            unchecked((int) 0x8004D00F);

        ///<summary>
        ///The resource manager id is not associated with this transaction or the transaction manager.
        ///</summary>
        [Description("The resource manager id is not associated with this transaction or the transaction manager.")] public const int XACT_E_UNKNOWNRMGRID = unchecked((int) 0x8004D010);

        ///<summary>
        ///This method was called in the wrong state
        ///</summary>
        [Description("This method was called in the wrong state")] public const int XACT_E_WRONGSTATE =
            unchecked((int) 0x8004D011);

        ///<summary>
        ///The indicated unit of work does not match the unit of work expected by the resource manager.
        ///</summary>
        [Description("The indicated unit of work does not match the unit of work expected by the resource manager.")] public const int XACT_E_WRONGUOW = unchecked((int) 0x8004D012);

        ///<summary>
        ///An enlistment in a transaction already exists.
        ///</summary>
        [Description("An enlistment in a transaction already exists.")] public const int XACT_E_XTIONEXISTS =
            unchecked((int) 0x8004D013);

        ///<summary>
        ///An import object for the transaction could not be found.
        ///</summary>
        [Description("An import object for the transaction could not be found.")] public const int XACT_E_NOIMPORTOBJECT
            = unchecked((int) 0x8004D014);

        ///<summary>
        ///The transaction cookie is invalid.
        ///</summary>
        [Description("The transaction cookie is invalid.")] public const int XACT_E_INVALIDCOOKIE =
            unchecked((int) 0x8004D015);

        ///<summary>
        ///The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed
        ///</summary>
        [Description(
            "The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed"
            )] public const int XACT_E_INDOUBT = unchecked((int) 0x8004D016);

        ///<summary>
        ///A time-out was specified, but time-outs are not supported.
        ///</summary>
        [Description("A time-out was specified, but time-outs are not supported.")] public const int XACT_E_NOTIMEOUT =
            unchecked((int) 0x8004D017);

        ///<summary>
        ///The requested operation is already in progress for the transaction.
        ///</summary>
        [Description("The requested operation is already in progress for the transaction.")] public const int
            XACT_E_ALREADYINPROGRESS = unchecked((int) 0x8004D018);

        ///<summary>
        ///The transaction has already been aborted.
        ///</summary>
        [Description("The transaction has already been aborted.")] public const int XACT_E_ABORTED =
            unchecked((int) 0x8004D019);

        ///<summary>
        ///The Transaction Manager returned a log full error.
        ///</summary>
        [Description("The Transaction Manager returned a log full error.")] public const int XACT_E_LOGFULL =
            unchecked((int) 0x8004D01A);

        ///<summary>
        ///The Transaction Manager is not available.
        ///</summary>
        [Description("The Transaction Manager is not available.")] public const int XACT_E_TMNOTAVAILABLE =
            unchecked((int) 0x8004D01B);

        ///<summary>
        ///A connection with the transaction manager was lost.
        ///</summary>
        [Description("A connection with the transaction manager was lost.")] public const int XACT_E_CONNECTION_DOWN =
            unchecked((int) 0x8004D01C);

        ///<summary>
        ///A request to establish a connection with the transaction manager was denied.
        ///</summary>
        [Description("A request to establish a connection with the transaction manager was denied.")] public const int
            XACT_E_CONNECTION_DENIED = unchecked((int) 0x8004D01D);

        ///<summary>
        ///Resource manager reenlistment to determine transaction status timed out.
        ///</summary>
        [Description("Resource manager reenlistment to determine transaction status timed out.")] public const int
            XACT_E_REENLISTTIMEOUT = unchecked((int) 0x8004D01E);

        ///<summary>
        ///This transaction manager failed to establish a connection with another TIP transaction manager.
        ///</summary>
        [Description("This transaction manager failed to establish a connection with another TIP transaction manager.")] public const int XACT_E_TIP_CONNECT_FAILED = unchecked((int) 0x8004D01F);

        ///<summary>
        ///This transaction manager encountered a protocol error with another TIP transaction manager.
        ///</summary>
        [Description("This transaction manager encountered a protocol error with another TIP transaction manager.")] public const int XACT_E_TIP_PROTOCOL_ERROR = unchecked((int) 0x8004D020);

        ///<summary>
        ///This transaction manager could not propagate a transaction from another TIP transaction manager.
        ///</summary>
        [Description("This transaction manager could not propagate a transaction from another TIP transaction manager.")
        ] public const int XACT_E_TIP_PULL_FAILED = unchecked((int) 0x8004D021);

        ///<summary>
        ///The Transaction Manager on the destination machine is not available.
        ///</summary>
        [Description("The Transaction Manager on the destination machine is not available.")] public const int
            XACT_E_DEST_TMNOTAVAILABLE = unchecked((int) 0x8004D022);

        ///<summary>
        ///The Transaction Manager has disabled its support for TIP.
        ///</summary>
        [Description("The Transaction Manager has disabled its support for TIP.")] public const int XACT_E_TIP_DISABLED
            = unchecked((int) 0x8004D023);

        ///<summary>
        ///The transaction manager has disabled its support for remote/network transactions.
        ///</summary>
        [Description("The transaction manager has disabled its support for remote/network transactions.")] public const
            int XACT_E_NETWORK_TX_DISABLED = unchecked((int) 0x8004D024);

        ///<summary>
        ///The partner transaction manager has disabled its support for remote/network transactions.
        ///</summary>
        [Description("The partner transaction manager has disabled its support for remote/network transactions.")] public const int XACT_E_PARTNER_NETWORK_TX_DISABLED = unchecked((int) 0x8004D025);

        ///<summary>
        ///The transaction manager has disabled its support for XA transactions.
        ///</summary>
        [Description("The transaction manager has disabled its support for XA transactions.")] public const int
            XACT_E_XA_TX_DISABLED = unchecked((int) 0x8004D026);

        ///<summary>
        ///MSDTC was unable to read its configuration information.
        ///</summary>
        [Description("MSDTC was unable to read its configuration information.")] public const int
            XACT_E_UNABLE_TO_READ_DTC_CONFIG = unchecked((int) 0x8004D027);

        ///<summary>
        ///MSDTC was unable to load the dtc proxy dll.
        ///</summary>
        [Description("MSDTC was unable to load the dtc proxy dll.")] public const int XACT_E_UNABLE_TO_LOAD_DTC_PROXY =
            unchecked((int) 0x8004D028);

        ///<summary>
        ///The local transaction has aborted.
        ///</summary>
        [Description("The local transaction has aborted.")] public const int XACT_E_ABORTING =
            unchecked((int) 0x8004D029);

        //
        // TXF & CRM errors start 4d080.
        ///<summary>
        ///XACT_E_CLERKNOTFOUND
        ///</summary>
        [Description("XACT_E_CLERKNOTFOUND")] public const int XACT_E_CLERKNOTFOUND = unchecked((int) 0x8004D080);

        ///<summary>
        ///XACT_E_CLERKEXISTS
        ///</summary>
        [Description("XACT_E_CLERKEXISTS")] public const int XACT_E_CLERKEXISTS = unchecked((int) 0x8004D081);

        ///<summary>
        ///XACT_E_RECOVERYINPROGRESS
        ///</summary>
        [Description("XACT_E_RECOVERYINPROGRESS")] public const int XACT_E_RECOVERYINPROGRESS =
            unchecked((int) 0x8004D082);

        ///<summary>
        ///XACT_E_TRANSACTIONCLOSED
        ///</summary>
        [Description("XACT_E_TRANSACTIONCLOSED")] public const int XACT_E_TRANSACTIONCLOSED =
            unchecked((int) 0x8004D083);

        ///<summary>
        ///XACT_E_INVALIDLSN
        ///</summary>
        [Description("XACT_E_INVALIDLSN")] public const int XACT_E_INVALIDLSN = unchecked((int) 0x8004D084);

        ///<summary>
        ///XACT_E_REPLAYREQUEST
        ///</summary>
        [Description("XACT_E_REPLAYREQUEST")] public const int XACT_E_REPLAYREQUEST = unchecked((int) 0x8004D085);

        //
        // OleTx Success codes.
        //
        ///<summary>
        ///An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.
        ///</summary>
        [Description(
            "An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.")] public const int XACT_S_ASYNC = unchecked((int) 0x0004D000);

        ///<summary>
        ///XACT_S_DEFECT
        ///</summary>
        [Description("XACT_S_DEFECT")] public const int XACT_S_DEFECT = unchecked((int) 0x0004D001);

        ///<summary>
        ///The method call succeeded because the transaction was read-only.
        ///</summary>
        [Description("The method call succeeded because the transaction was read-only.")] public const int
            XACT_S_READONLY = unchecked((int) 0x0004D002);

        ///<summary>
        ///The transaction was successfully aborted. However, this is a coordinated transaction, and some number of enlisted resources were aborted outright because they could not support abort-retaining semantics
        ///</summary>
        [Description(
            "The transaction was successfully aborted. However, this is a coordinated transaction, and some number of enlisted resources were aborted outright because they could not support abort-retaining semantics"
            )] public const int XACT_S_SOMENORETAIN = unchecked((int) 0x0004D003);

        ///<summary>
        ///No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes.
        ///</summary>
        [Description(
            "No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes."
            )] public const int XACT_S_OKINFORM = unchecked((int) 0x0004D004);

        ///<summary>
        ///The sink is content and wishes the transaction to proceed. Changes were made to one or more resources during this call.
        ///</summary>
        [Description(
            "The sink is content and wishes the transaction to proceed. Changes were made to one or more resources during this call."
            )] public const int XACT_S_MADECHANGESCONTENT = unchecked((int) 0x0004D005);

        ///<summary>
        ///The sink is for the moment and wishes the transaction to proceed, but if other changes are made following this return by other event sinks then this sink wants another chance to look
        ///</summary>
        [Description(
            "The sink is for the moment and wishes the transaction to proceed, but if other changes are made following this return by other event sinks then this sink wants another chance to look"
            )] public const int XACT_S_MADECHANGESINFORM = unchecked((int) 0x0004D006);

        ///<summary>
        ///The transaction was successfully aborted. However, the abort was non-retaining.
        ///</summary>
        [Description("The transaction was successfully aborted. However, the abort was non-retaining.")] public const
            int XACT_S_ALLNORETAIN = unchecked((int) 0x0004D007);

        ///<summary>
        ///An abort operation was already in progress.
        ///</summary>
        [Description("An abort operation was already in progress.")] public const int XACT_S_ABORTING =
            unchecked((int) 0x0004D008);

        ///<summary>
        ///The resource manager has performed a single-phase commit of the transaction.
        ///</summary>
        [Description("The resource manager has performed a single-phase commit of the transaction.")] public const int
            XACT_S_SINGLEPHASE = unchecked((int) 0x0004D009);

        ///<summary>
        ///The local transaction has not aborted.
        ///</summary>
        [Description("The local transaction has not aborted.")] public const int XACT_S_LOCALLY_OK =
            unchecked((int) 0x0004D00A);

        ///<summary>
        ///The resource manager has requested to be the coordinator (last resource manager) for the transaction.
        ///</summary>
        [Description(
            "The resource manager has requested to be the coordinator (last resource manager) for the transaction.")] public const int XACT_S_LASTRESOURCEMANAGER = unchecked((int) 0x0004D010);

        #endregion

        #region (0x04E000 - 0x04E02F) CONTEXT errors

        public const int CONTEXT_E_FIRST = unchecked((int) 0x8004E000);
        public const int CONTEXT_E_LAST = unchecked((int) 0x8004E02F);
        public const int CONTEXT_S_FIRST = unchecked((int) 0x0004E000);
        public const int CONTEXT_S_LAST = unchecked((int) 0x0004E02F);

        ///<summary>
        ///The root transaction wanted to commit, but transaction aborted
        ///</summary>
        [Description("The root transaction wanted to commit, but transaction aborted")] public const int
            CONTEXT_E_ABORTED = unchecked((int) 0x8004E002);

        ///<summary>
        ///You made a method call on a COM+ component that has a transaction that has already aborted or in the process of aborting.
        ///</summary>
        [Description(
            "You made a method call on a COM+ component that has a transaction that has already aborted or in the process of aborting."
            )] public const int CONTEXT_E_ABORTING = unchecked((int) 0x8004E003);

        ///<summary>
        ///There is no MTS object context
        ///</summary>
        [Description("There is no MTS object context")] public const int CONTEXT_E_NOCONTEXT =
            unchecked((int) 0x8004E004);

        ///<summary>
        ///The component is configured to use synchronization and this method call would cause a deadlock to occur.
        ///</summary>
        [Description(
            "The component is configured to use synchronization and this method call would cause a deadlock to occur.")] public const int CONTEXT_E_WOULD_DEADLOCK = unchecked((int) 0x8004E005);

        ///<summary>
        ///The component is configured to use synchronization and a thread has timed out waiting to enter the context.
        ///</summary>
        [Description(
            "The component is configured to use synchronization and a thread has timed out waiting to enter the context."
            )] public const int CONTEXT_E_SYNCH_TIMEOUT = unchecked((int) 0x8004E006);

        ///<summary>
        ///You made a method call on a COM+ component that has a transaction that has already committed or aborted.
        ///</summary>
        [Description(
            "You made a method call on a COM+ component that has a transaction that has already committed or aborted.")] public const int CONTEXT_E_OLDREF = unchecked((int) 0x8004E007);

        ///<summary>
        ///The specified role was not configured for the application
        ///</summary>
        [Description("The specified role was not configured for the application")] public const int
            CONTEXT_E_ROLENOTFOUND = unchecked((int) 0x8004E00C);

        ///<summary>
        ///COM+ was unable to talk to the Microsoft Distributed Transaction Coordinator
        ///</summary>
        [Description("COM+ was unable to talk to the Microsoft Distributed Transaction Coordinator")] public const int
            CONTEXT_E_TMNOTAVAILABLE = unchecked((int) 0x8004E00F);

        ///<summary>
        ///An unexpected error occurred during COM+ Activation.
        ///</summary>
        [Description("An unexpected error occurred during COM+ Activation.")] public const int CO_E_ACTIVATIONFAILED =
            unchecked((int) 0x8004E021);

        ///<summary>
        ///COM+ Activation failed. Check the event log for more information
        ///</summary>
        [Description("COM+ Activation failed. Check the event log for more information")] public const int
            CO_E_ACTIVATIONFAILED_EVENTLOGGED = unchecked((int) 0x8004E022);

        ///<summary>
        ///COM+ Activation failed due to a catalog or configuration error.
        ///</summary>
        [Description("COM+ Activation failed due to a catalog or configuration error.")] public const int
            CO_E_ACTIVATIONFAILED_CATALOGERROR = unchecked((int) 0x8004E023);

        ///<summary>
        ///COM+ activation failed because the activation could not be completed in the specified amount of time.
        ///</summary>
        [Description(
            "COM+ activation failed because the activation could not be completed in the specified amount of time.")] public const int CO_E_ACTIVATIONFAILED_TIMEOUT = unchecked((int) 0x8004E024);

        ///<summary>
        ///COM+ Activation failed because an initialization function failed.  Check the event log for more information.
        ///</summary>
        [Description(
            "COM+ Activation failed because an initialization function failed.  Check the event log for more information."
            )] public const int CO_E_INITIALIZATIONFAILED = unchecked((int) 0x8004E025);

        ///<summary>
        ///The requested operation requires that JIT be in the current context and it is not
        ///</summary>
        [Description("The requested operation requires that JIT be in the current context and it is not")] public const
            int CONTEXT_E_NOJIT = unchecked((int) 0x8004E026);

        ///<summary>
        ///The requested operation requires that the current context have a Transaction, and it does not
        ///</summary>
        [Description("The requested operation requires that the current context have a Transaction, and it does not")] public const int CONTEXT_E_NOTRANSACTION = unchecked((int) 0x8004E027);

        ///<summary>
        ///The components threading model has changed after install into a COM+ Application.  Please re-install component.
        ///</summary>
        [Description(
            "The components threading model has changed after install into a COM+ Application.  Please re-install component."
            )] public const int CO_E_THREADINGMODEL_CHANGED = unchecked((int) 0x8004E028);

        ///<summary>
        ///IIS intrinsics not available.  Start your work with IIS.
        ///</summary>
        [Description("IIS intrinsics not available.  Start your work with IIS.")] public const int CO_E_NOIISINTRINSICS
            = unchecked((int) 0x8004E029);

        ///<summary>
        ///An attempt to write a cookie failed.
        ///</summary>
        [Description("An attempt to write a cookie failed.")] public const int CO_E_NOCOOKIES =
            unchecked((int) 0x8004E02A);

        ///<summary>
        ///An attempt to use a database generated a database specific error.
        ///</summary>
        [Description("An attempt to use a database generated a database specific error.")] public const int CO_E_DBERROR
            = unchecked((int) 0x8004E02B);

        ///<summary>
        ///The COM+ component you created must use object pooling to work.
        ///</summary>
        [Description("The COM+ component you created must use object pooling to work.")] public const int CO_E_NOTPOOLED
            = unchecked((int) 0x8004E02C);

        ///<summary>
        ///The COM+ component you created must use object construction to work correctly.
        ///</summary>
        [Description("The COM+ component you created must use object construction to work correctly.")] public const int
            CO_E_NOTCONSTRUCTED = unchecked((int) 0x8004E02D);

        ///<summary>
        ///The COM+ component requires synchronization, and it is not configured for it.
        ///</summary>
        [Description("The COM+ component requires synchronization, and it is not configured for it.")] public const int
            CO_E_NOSYNCHRONIZATION = unchecked((int) 0x8004E02E);

        ///<summary>
        ///The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the "root" component for the transaction.  The creation failed.
        ///</summary>
        [Description(
            "The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the \"root\" component for the transaction.  The creation failed."
            )] public const int CO_E_ISOLEVELMISMATCH = unchecked((int) 0x8004E02F);

        #endregion

        #region (0x040000 - 0x040200) Old OLE Success Codes

        //
        // Old OLE Success Codes
        //
        ///<summary>
        ///Use the registry database to provide the requested information
        ///</summary>
        [Description("Use the registry database to provide the requested information")] public const int OLE_S_USEREG =
            unchecked((int) 0x00040000);

        ///<summary>
        ///Success, but static
        ///</summary>
        [Description("Success, but static")] public const int OLE_S_STATIC = unchecked((int) 0x00040001);

        ///<summary>
        ///Macintosh clipboard format
        ///</summary>
        [Description("Macintosh clipboard format")] public const int OLE_S_MAC_CLIPFORMAT = unchecked((int) 0x00040002);

        ///<summary>
        ///Successful drop took place
        ///</summary>
        [Description("Successful drop took place")] public const int DRAGDROP_S_DROP = unchecked((int) 0x00040100);

        ///<summary>
        ///Drag-drop operation canceled
        ///</summary>
        [Description("Drag-drop operation canceled")] public const int DRAGDROP_S_CANCEL = unchecked((int) 0x00040101);

        ///<summary>
        ///Use the default cursor
        ///</summary>
        [Description("Use the default cursor")] public const int DRAGDROP_S_USEDEFAULTCURSORS =
            unchecked((int) 0x00040102);

        ///<summary>
        ///Data has same FORMATETC
        ///</summary>
        [Description("Data has same FORMATETC")] public const int DATA_S_SAMEFORMATETC = unchecked((int) 0x00040130);

        ///<summary>
        ///View is already frozen
        ///</summary>
        [Description("View is already frozen")] public const int VIEW_S_ALREADY_FROZEN = unchecked((int) 0x00040140);

        ///<summary>
        ///FORMATETC not supported
        ///</summary>
        [Description("FORMATETC not supported")] public const int CACHE_S_FORMATETC_NOTSUPPORTED =
            unchecked((int) 0x00040170);

        ///<summary>
        ///Same cache
        ///</summary>
        [Description("Same cache")] public const int CACHE_S_SAMECACHE = unchecked((int) 0x00040171);

        ///<summary>
        ///Some cache(s) not updated
        ///</summary>
        [Description("Some cache(s) not updated")] public const int CACHE_S_SOMECACHES_NOTUPDATED =
            unchecked((int) 0x00040172);

        ///<summary>
        ///Invalid verb for OLE object
        ///</summary>
        [Description("Invalid verb for OLE object")] public const int OLEOBJ_S_INVALIDVERB = unchecked((int) 0x00040180);

        ///<summary>
        ///Verb number is valid but verb cannot be done now
        ///</summary>
        [Description("Verb number is valid but verb cannot be done now")] public const int OLEOBJ_S_CANNOT_DOVERB_NOW =
            unchecked((int) 0x00040181);

        ///<summary>
        ///Invalid window handle passed
        ///</summary>
        [Description("Invalid window handle passed")] public const int OLEOBJ_S_INVALIDHWND =
            unchecked((int) 0x00040182);

        ///<summary>
        ///Message is too long; some of it had to be truncated before displaying
        ///</summary>
        [Description("Message is too long; some of it had to be truncated before displaying")] public const int
            INPLACE_S_TRUNCATED = unchecked((int) 0x000401A0);

        ///<summary>
        ///Unable to convert OLESTREAM to IStorage
        ///</summary>
        [Description("Unable to convert OLESTREAM to IStorage")] public const int CONVERT10_S_NO_PRESENTATION =
            unchecked((int) 0x000401C0);

        ///<summary>
        ///Moniker reduced to itself
        ///</summary>
        [Description("Moniker reduced to itself")] public const int MK_S_REDUCED_TO_SELF = unchecked((int) 0x000401E2);

        ///<summary>
        ///Common prefix is this moniker
        ///</summary>
        [Description("Common prefix is this moniker")] public const int MK_S_ME = unchecked((int) 0x000401E4);

        ///<summary>
        ///Common prefix is input moniker
        ///</summary>
        [Description("Common prefix is input moniker")] public const int MK_S_HIM = unchecked((int) 0x000401E5);

        ///<summary>
        ///Common prefix is both monikers
        ///</summary>
        [Description("Common prefix is both monikers")] public const int MK_S_US = unchecked((int) 0x000401E6);

        ///<summary>
        ///Moniker is already registered in running object table
        ///</summary>
        [Description("Moniker is already registered in running object table")] public const int
            MK_S_MONIKERALREADYREGISTERED = unchecked((int) 0x000401E7);

        #endregion

        #region (0x041300 - 0x041315) SCHED (Task Scheduler) errors

        //
        // Task Scheduler errors
        //
        ///<summary>
        ///The task is ready to run at its next scheduled time.
        ///</summary>
        [Description("The task is ready to run at its next scheduled time.")] public const int SCHED_S_TASK_READY =
            unchecked((int) 0x00041300);

        ///<summary>
        ///The task is currently running.
        ///</summary>
        [Description("The task is currently running.")] public const int SCHED_S_TASK_RUNNING =
            unchecked((int) 0x00041301);

        ///<summary>
        ///The task will not run at the scheduled times because it has been disabled.
        ///</summary>
        [Description("The task will not run at the scheduled times because it has been disabled.")] public const int
            SCHED_S_TASK_DISABLED = unchecked((int) 0x00041302);

        ///<summary>
        ///The task has not yet run.
        ///</summary>
        [Description("The task has not yet run.")] public const int SCHED_S_TASK_HAS_NOT_RUN =
            unchecked((int) 0x00041303);

        ///<summary>
        ///There are no more runs scheduled for this task.
        ///</summary>
        [Description("There are no more runs scheduled for this task.")] public const int SCHED_S_TASK_NO_MORE_RUNS =
            unchecked((int) 0x00041304);

        ///<summary>
        ///One or more of the properties that are needed to run this task on a schedule have not been set.
        ///</summary>
        [Description("One or more of the properties that are needed to run this task on a schedule have not been set.")] public const int SCHED_S_TASK_NOT_SCHEDULED = unchecked((int) 0x00041305);

        ///<summary>
        ///The last run of the task was terminated by the user.
        ///</summary>
        [Description("The last run of the task was terminated by the user.")] public const int SCHED_S_TASK_TERMINATED =
            unchecked((int) 0x00041306);

        ///<summary>
        ///Either the task has no triggers or the existing triggers are disabled or not set.
        ///</summary>
        [Description("Either the task has no triggers or the existing triggers are disabled or not set.")] public const
            int SCHED_S_TASK_NO_VALID_TRIGGERS = unchecked((int) 0x00041307);

        ///<summary>
        ///Event triggers don't have set run times.
        ///</summary>
        [Description("Event triggers don't have set run times.")] public const int SCHED_S_EVENT_TRIGGER =
            unchecked((int) 0x00041308);

        ///<summary>
        ///Trigger not found.
        ///</summary>
        [Description("Trigger not found.")] public const int SCHED_E_TRIGGER_NOT_FOUND = unchecked((int) 0x80041309);

        ///<summary>
        ///One or more of the properties that are needed to run this task have not been set.
        ///</summary>
        [Description("One or more of the properties that are needed to run this task have not been set.")] public const
            int SCHED_E_TASK_NOT_READY = unchecked((int) 0x8004130A);

        ///<summary>
        ///There is no running instance of the task to terminate.
        ///</summary>
        [Description("There is no running instance of the task to terminate.")] public const int
            SCHED_E_TASK_NOT_RUNNING = unchecked((int) 0x8004130B);

        ///<summary>
        ///The Task Scheduler Service is not installed on this computer.
        ///</summary>
        [Description("The Task Scheduler Service is not installed on this computer.")] public const int
            SCHED_E_SERVICE_NOT_INSTALLED = unchecked((int) 0x8004130C);

        ///<summary>
        ///The task object could not be opened.
        ///</summary>
        [Description("The task object could not be opened.")] public const int SCHED_E_CANNOT_OPEN_TASK =
            unchecked((int) 0x8004130D);

        ///<summary>
        ///The object is either an invalid task object or is not a task object.
        ///</summary>
        [Description("The object is either an invalid task object or is not a task object.")] public const int
            SCHED_E_INVALID_TASK = unchecked((int) 0x8004130E);

        ///<summary>
        ///No account information could be found in the Task Scheduler security database for the task indicated.
        ///</summary>
        [Description(
            "No account information could be found in the Task Scheduler security database for the task indicated.")] public const int SCHED_E_ACCOUNT_INFORMATION_NOT_SET = unchecked((int) 0x8004130F);

        ///<summary>
        ///Unable to establish existence of the account specified.
        ///</summary>
        [Description("Unable to establish existence of the account specified.")] public const int
            SCHED_E_ACCOUNT_NAME_NOT_FOUND = unchecked((int) 0x80041310);

        ///<summary>
        ///Corruption was detected in the Task Scheduler security database; the database has been reset.
        ///</summary>
        [Description("Corruption was detected in the Task Scheduler security database; the database has been reset.")] public const int SCHED_E_ACCOUNT_DBASE_CORRUPT = unchecked((int) 0x80041311);

        ///<summary>
        ///Task Scheduler security services are available only on Windows NT.
        ///</summary>
        [Description("Task Scheduler security services are available only on Windows NT.")] public const int
            SCHED_E_NO_SECURITY_SERVICES = unchecked((int) 0x80041312);

        ///<summary>
        ///The task object version is either unsupported or invalid.
        ///</summary>
        [Description("The task object version is either unsupported or invalid.")] public const int
            SCHED_E_UNKNOWN_OBJECT_VERSION = unchecked((int) 0x80041313);

        ///<summary>
        ///The task has been configured with an unsupported combination of account settings and run time options.
        ///</summary>
        [Description(
            "The task has been configured with an unsupported combination of account settings and run time options.")] public const int SCHED_E_UNSUPPORTED_ACCOUNT_OPTION = unchecked((int) 0x80041314);

        ///<summary>
        ///The Task Scheduler Service is not running.
        ///</summary>
        [Description("The Task Scheduler Service is not running.")] public const int SCHED_E_SERVICE_NOT_RUNNING =
            unchecked((int) 0x80041315);

        #endregion

        #endregion

        // ******************
        // FACILITY_WIN32
        // ******************

        #region (0x070000 - 0x07FFFF) FACILITY_WIN32 errors

        #endregion

        // ******************
        // FACILITY_WINDOWS
        // ******************

        #region (0x080000 - 0x0801FF) FACILITY_WINDOWS errors

        //
        // Codes 0x0-0x01ff are reserved for the OLE group of
        // interfaces.
        //
        ///<summary>
        ///Attempt to create a class object failed
        ///</summary>
        [Description("Attempt to create a class object failed")] public const int CO_E_CLASS_CREATE_FAILED =
            unchecked((int) 0x80080001);

        ///<summary>
        ///OLE service could not bind object
        ///</summary>
        [Description("OLE service could not bind object")] public const int CO_E_SCM_ERROR = unchecked((int) 0x80080002);

        ///<summary>
        ///RPC communication failed with OLE service
        ///</summary>
        [Description("RPC communication failed with OLE service")] public const int CO_E_SCM_RPC_FAILURE =
            unchecked((int) 0x80080003);

        ///<summary>
        ///Bad path to object
        ///</summary>
        [Description("Bad path to object")] public const int CO_E_BAD_PATH = unchecked((int) 0x80080004);

        ///<summary>
        ///Server execution failed
        ///</summary>
        [Description("Server execution failed")] public const int CO_E_SERVER_EXEC_FAILURE = unchecked((int) 0x80080005);

        ///<summary>
        ///OLE service could not communicate with the object server
        ///</summary>
        [Description("OLE service could not communicate with the object server")] public const int
            CO_E_OBJSRV_RPC_FAILURE = unchecked((int) 0x80080006);

        ///<summary>
        ///Moniker path could not be normalized
        ///</summary>
        [Description("Moniker path could not be normalized")] public const int MK_E_NO_NORMALIZED =
            unchecked((int) 0x80080007);

        ///<summary>
        ///Object server is stopping when OLE service contacts it
        ///</summary>
        [Description("Object server is stopping when OLE service contacts it")] public const int CO_E_SERVER_STOPPING =
            unchecked((int) 0x80080008);

        ///<summary>
        ///An invalid root block pointer was specified
        ///</summary>
        [Description("An invalid root block pointer was specified")] public const int MEM_E_INVALID_ROOT =
            unchecked((int) 0x80080009);

        ///<summary>
        ///An allocation chain contained an invalid link pointer
        ///</summary>
        [Description("An allocation chain contained an invalid link pointer")] public const int MEM_E_INVALID_LINK =
            unchecked((int) 0x80080010);

        ///<summary>
        ///The requested allocation size was too large
        ///</summary>
        [Description("The requested allocation size was too large")] public const int MEM_E_INVALID_SIZE =
            unchecked((int) 0x80080011);

        ///<summary>
        ///Not all the requested interfaces were available
        ///</summary>
        [Description("Not all the requested interfaces were available")] public const int CO_S_NOTALLINTERFACES =
            unchecked((int) 0x00080012);

        ///<summary>
        ///The specified machine name was not found in the cache.
        ///</summary>
        [Description("The specified machine name was not found in the cache.")] public const int
            CO_S_MACHINENAMENOTFOUND = unchecked((int) 0x00080013);

        #endregion

        // ******************
        //  FACILITY_SSPI
        // ******************

        #region (0x090000 - 0x09FFFF) FACILITY_SSPI (FACILITY_SECURITY) errors

        public const int NTE_OP_OK = NO_ERROR;

        //////////////////////////////////////
        //                  //
        // Additional Security Status Codes //
        //                  //
        // Facility=Security        //
        //                  //
        //////////////////////////////////////

        ///<summary>
        ///The specified event is currently not being audited.
        ///</summary>
        [Description("The specified event is currently not being audited.")] public const int ERROR_AUDITING_DISABLED =
            unchecked((int) 0xC0090001);

        ///<summary>
        ///The SID filtering operation removed all SIDs.
        ///</summary>
        [Description("The SID filtering operation removed all SIDs.")] public const int ERROR_ALL_SIDS_FILTERED =
            unchecked((int) 0xC0090002);

        /////////////////////////////////////////////
        //                     //
        // end of Additional Security Status Codes //
        //                     //
        /////////////////////////////////////////////

        /////////////////
        //
        //  FACILITY_SSPI
        //
        /////////////////

        ///<summary>
        ///Bad UID.
        ///</summary>
        [Description("Bad UID.")] public const int NTE_BAD_UID = unchecked((int) 0x80090001);

        ///<summary>
        ///Bad Hash.
        ///</summary>
        [Description("Bad Hash.")] public const int NTE_BAD_HASH = unchecked((int) 0x80090002);

        ///<summary>
        ///Bad Key.
        ///</summary>
        [Description("Bad Key.")] public const int NTE_BAD_KEY = unchecked((int) 0x80090003);

        ///<summary>
        ///Bad Length.
        ///</summary>
        [Description("Bad Length.")] public const int NTE_BAD_LEN = unchecked((int) 0x80090004);

        ///<summary>
        ///Bad Data.
        ///</summary>
        [Description("Bad Data.")] public const int NTE_BAD_DATA = unchecked((int) 0x80090005);

        ///<summary>
        ///Invalid Signature.
        ///</summary>
        [Description("Invalid Signature.")] public const int NTE_BAD_SIGNATURE = unchecked((int) 0x80090006);

        ///<summary>
        ///Bad Version of provider.
        ///</summary>
        [Description("Bad Version of provider.")] public const int NTE_BAD_VER = unchecked((int) 0x80090007);

        ///<summary>
        ///Invalid algorithm specified.
        ///</summary>
        [Description("Invalid algorithm specified.")] public const int NTE_BAD_ALGID = unchecked((int) 0x80090008);

        ///<summary>
        ///Invalid flags specified.
        ///</summary>
        [Description("Invalid flags specified.")] public const int NTE_BAD_FLAGS = unchecked((int) 0x80090009);

        ///<summary>
        ///Invalid type specified.
        ///</summary>
        [Description("Invalid type specified.")] public const int NTE_BAD_TYPE = unchecked((int) 0x8009000A);

        ///<summary>
        ///Key not valid for use in specified state.
        ///</summary>
        [Description("Key not valid for use in specified state.")] public const int NTE_BAD_KEY_STATE =
            unchecked((int) 0x8009000B);

        ///<summary>
        ///Hash not valid for use in specified state.
        ///</summary>
        [Description("Hash not valid for use in specified state.")] public const int NTE_BAD_HASH_STATE =
            unchecked((int) 0x8009000C);

        ///<summary>
        ///Key does not exist.
        ///</summary>
        [Description("Key does not exist.")] public const int NTE_NO_KEY = unchecked((int) 0x8009000D);

        ///<summary>
        ///Insufficient memory available for the operation.
        ///</summary>
        [Description("Insufficient memory available for the operation.")] public const int NTE_NO_MEMORY =
            unchecked((int) 0x8009000E);

        ///<summary>
        ///Object already exists.
        ///</summary>
        [Description("Object already exists.")] public const int NTE_EXISTS = unchecked((int) 0x8009000F);

        ///<summary>
        ///Access denied.
        ///</summary>
        [Description("Access denied.")] public const int NTE_PERM = unchecked((int) 0x80090010);

        ///<summary>
        ///Object was not found.
        ///</summary>
        [Description("Object was not found.")] public const int NTE_NOT_FOUND = unchecked((int) 0x80090011);

        ///<summary>
        ///Data already encrypted.
        ///</summary>
        [Description("Data already encrypted.")] public const int NTE_DOUBLE_ENCRYPT = unchecked((int) 0x80090012);

        ///<summary>
        ///Invalid provider specified.
        ///</summary>
        [Description("Invalid provider specified.")] public const int NTE_BAD_PROVIDER = unchecked((int) 0x80090013);

        ///<summary>
        ///Invalid provider type specified.
        ///</summary>
        [Description("Invalid provider type specified.")] public const int NTE_BAD_PROV_TYPE =
            unchecked((int) 0x80090014);

        ///<summary>
        ///Provider's public key is invalid.
        ///</summary>
        [Description("Provider's public key is invalid.")] public const int NTE_BAD_PUBLIC_KEY =
            unchecked((int) 0x80090015);

        ///<summary>
        ///Keyset does not exist
        ///</summary>
        [Description("Keyset does not exist")] public const int NTE_BAD_KEYSET = unchecked((int) 0x80090016);

        ///<summary>
        ///Provider type not defined.
        ///</summary>
        [Description("Provider type not defined.")] public const int NTE_PROV_TYPE_NOT_DEF = unchecked((int) 0x80090017);

        ///<summary>
        ///Provider type as registered is invalid.
        ///</summary>
        [Description("Provider type as registered is invalid.")] public const int NTE_PROV_TYPE_ENTRY_BAD =
            unchecked((int) 0x80090018);

        ///<summary>
        ///The keyset is not defined.
        ///</summary>
        [Description("The keyset is not defined.")] public const int NTE_KEYSET_NOT_DEF = unchecked((int) 0x80090019);

        ///<summary>
        ///Keyset as registered is invalid.
        ///</summary>
        [Description("Keyset as registered is invalid.")] public const int NTE_KEYSET_ENTRY_BAD =
            unchecked((int) 0x8009001A);

        ///<summary>
        ///Provider type does not match registered value.
        ///</summary>
        [Description("Provider type does not match registered value.")] public const int NTE_PROV_TYPE_NO_MATCH =
            unchecked((int) 0x8009001B);

        ///<summary>
        ///The digital signature file is corrupt.
        ///</summary>
        [Description("The digital signature file is corrupt.")] public const int NTE_SIGNATURE_FILE_BAD =
            unchecked((int) 0x8009001C);

        ///<summary>
        ///Provider DLL failed to initialize correctly.
        ///</summary>
        [Description("Provider DLL failed to initialize correctly.")] public const int NTE_PROVIDER_DLL_FAIL =
            unchecked((int) 0x8009001D);

        ///<summary>
        ///Provider DLL could not be found.
        ///</summary>
        [Description("Provider DLL could not be found.")] public const int NTE_PROV_DLL_NOT_FOUND =
            unchecked((int) 0x8009001E);

        ///<summary>
        ///The Keyset parameter is invalid.
        ///</summary>
        [Description("The Keyset parameter is invalid.")] public const int NTE_BAD_KEYSET_PARAM =
            unchecked((int) 0x8009001F);

        ///<summary>
        ///An internal error occurred.
        ///</summary>
        [Description("An internal error occurred.")] public const int NTE_FAIL = unchecked((int) 0x80090020);

        ///<summary>
        ///A base error occurred.
        ///</summary>
        [Description("A base error occurred.")] public const int NTE_SYS_ERR = unchecked((int) 0x80090021);

        ///<summary>
        ///Provider could not perform the action since the context was acquired as silent.
        ///</summary>
        [Description("Provider could not perform the action since the context was acquired as silent.")] public const
            int NTE_SILENT_CONTEXT = unchecked((int) 0x80090022);

        ///<summary>
        ///The security token does not have storage space available for an additional container.
        ///</summary>
        [Description("The security token does not have storage space available for an additional container.")] public
            const int NTE_TOKEN_KEYSET_STORAGE_FULL = unchecked((int) 0x80090023);

        ///<summary>
        ///The profile for the user is a temporary profile.
        ///</summary>
        [Description("The profile for the user is a temporary profile.")] public const int NTE_TEMPORARY_PROFILE =
            unchecked((int) 0x80090024);

        ///<summary>
        ///The key parameters could not be set because the CSP uses fixed parameters.
        ///</summary>
        [Description("The key parameters could not be set because the CSP uses fixed parameters.")] public const int
            NTE_FIXEDPARAMETER = unchecked((int) 0x80090025);

        ///<summary>
        ///Not enough memory is available to complete this request
        ///</summary>
        [Description("Not enough memory is available to complete this request")] public const int
            SEC_E_INSUFFICIENT_MEMORY = unchecked((int) 0x80090300);

        ///<summary>
        ///The handle specified is invalid
        ///</summary>
        [Description("The handle specified is invalid")] public const int SEC_E_INVALID_HANDLE =
            unchecked((int) 0x80090301);

        ///<summary>
        ///The function requested is not supported
        ///</summary>
        [Description("The function requested is not supported")] public const int SEC_E_UNSUPPORTED_FUNCTION =
            unchecked((int) 0x80090302);

        ///<summary>
        ///The specified target is unknown or unreachable
        ///</summary>
        [Description("The specified target is unknown or unreachable")] public const int SEC_E_TARGET_UNKNOWN =
            unchecked((int) 0x80090303);

        ///<summary>
        ///The Local Security Authority cannot be contacted
        ///</summary>
        [Description("The Local Security Authority cannot be contacted")] public const int SEC_E_INTERNAL_ERROR =
            unchecked((int) 0x80090304);

        ///<summary>
        ///The requested security package does not exist
        ///</summary>
        [Description("The requested security package does not exist")] public const int SEC_E_SECPKG_NOT_FOUND =
            unchecked((int) 0x80090305);

        ///<summary>
        ///The caller is not the owner of the desired credentials
        ///</summary>
        [Description("The caller is not the owner of the desired credentials")] public const int SEC_E_NOT_OWNER =
            unchecked((int) 0x80090306);

        ///<summary>
        ///The security package failed to initialize, and cannot be installed
        ///</summary>
        [Description("The security package failed to initialize, and cannot be installed")] public const int
            SEC_E_CANNOT_INSTALL = unchecked((int) 0x80090307);

        ///<summary>
        ///The token supplied to the function is invalid
        ///</summary>
        [Description("The token supplied to the function is invalid")] public const int SEC_E_INVALID_TOKEN =
            unchecked((int) 0x80090308);

        ///<summary>
        ///The security package is not able to marshall the logon buffer, so the logon attempt has failed
        ///</summary>
        [Description("The security package is not able to marshall the logon buffer, so the logon attempt has failed")] public const int SEC_E_CANNOT_PACK = unchecked((int) 0x80090309);

        ///<summary>
        ///The per-message Quality of Protection is not supported by the security package
        ///</summary>
        [Description("The per-message Quality of Protection is not supported by the security package")] public const int
            SEC_E_QOP_NOT_SUPPORTED = unchecked((int) 0x8009030A);

        ///<summary>
        ///The security context does not allow impersonation of the client
        ///</summary>
        [Description("The security context does not allow impersonation of the client")] public const int
            SEC_E_NO_IMPERSONATION = unchecked((int) 0x8009030B);

        ///<summary>
        ///The logon attempt failed
        ///</summary>
        [Description("The logon attempt failed")] public const int SEC_E_LOGON_DENIED = unchecked((int) 0x8009030C);

        ///<summary>
        ///The credentials supplied to the package were not recognized
        ///</summary>
        [Description("The credentials supplied to the package were not recognized")] public const int
            SEC_E_UNKNOWN_CREDENTIALS = unchecked((int) 0x8009030D);

        ///<summary>
        ///No credentials are available in the security package
        ///</summary>
        [Description("No credentials are available in the security package")] public const int SEC_E_NO_CREDENTIALS =
            unchecked((int) 0x8009030E);

        ///<summary>
        ///The message or signature supplied for verification has been altered
        ///</summary>
        [Description("The message or signature supplied for verification has been altered")] public const int
            SEC_E_MESSAGE_ALTERED = unchecked((int) 0x8009030F);

        ///<summary>
        ///The message supplied for verification is out of sequence
        ///</summary>
        [Description("The message supplied for verification is out of sequence")] public const int SEC_E_OUT_OF_SEQUENCE
            = unchecked((int) 0x80090310);

        ///<summary>
        ///No authority could be contacted for authentication.
        ///</summary>
        [Description("No authority could be contacted for authentication.")] public const int
            SEC_E_NO_AUTHENTICATING_AUTHORITY = unchecked((int) 0x80090311);

        ///<summary>
        ///The function completed successfully, but must be called again to complete the context
        ///</summary>
        [Description("The function completed successfully, but must be called again to complete the context")] public
            const int SEC_I_CONTINUE_NEEDED = unchecked((int) 0x00090312);

        ///<summary>
        ///The function completed successfully, but CompleteToken must be called
        ///</summary>
        [Description("The function completed successfully, but CompleteToken must be called")] public const int
            SEC_I_COMPLETE_NEEDED = unchecked((int) 0x00090313);

        ///<summary>
        ///The function completed successfully, but both CompleteToken and this function must be called to complete the context
        ///</summary>
        [Description(
            "The function completed successfully, but both CompleteToken and this function must be called to complete the context"
            )] public const int SEC_I_COMPLETE_AND_CONTINUE = unchecked((int) 0x00090314);

        ///<summary>
        ///The logon was completed, but no network authority was available. The logon was made using locally known information
        ///</summary>
        [Description(
            "The logon was completed, but no network authority was available. The logon was made using locally known information"
            )] public const int SEC_I_LOCAL_LOGON = unchecked((int) 0x00090315);

        ///<summary>
        ///The requested security package does not exist
        ///</summary>
        [Description("The requested security package does not exist")] public const int SEC_E_BAD_PKGID =
            unchecked((int) 0x80090316);

        ///<summary>
        ///The context has expired and can no longer be used.
        ///</summary>
        [Description("The context has expired and can no longer be used.")] public const int SEC_E_CONTEXT_EXPIRED =
            unchecked((int) 0x80090317);

        ///<summary>
        ///The context has expired and can no longer be used.
        ///</summary>
        [Description("The context has expired and can no longer be used.")] public const int SEC_I_CONTEXT_EXPIRED =
            unchecked((int) 0x00090317);

        ///<summary>
        ///The supplied message is incomplete.  The signature was not verified.
        ///</summary>
        [Description("The supplied message is incomplete.  The signature was not verified.")] public const int
            SEC_E_INCOMPLETE_MESSAGE = unchecked((int) 0x80090318);

        ///<summary>
        ///The credentials supplied were not complete, and could not be verified. The context could not be initialized.
        ///</summary>
        [Description(
            "The credentials supplied were not complete, and could not be verified. The context could not be initialized."
            )] public const int SEC_E_INCOMPLETE_CREDENTIALS = unchecked((int) 0x80090320);

        ///<summary>
        ///The buffers supplied to a function was too small.
        ///</summary>
        [Description("The buffers supplied to a function was too small.")] public const int SEC_E_BUFFER_TOO_SMALL =
            unchecked((int) 0x80090321);

        ///<summary>
        ///The credentials supplied were not complete, and could not be verified. Additional information can be returned from the context.
        ///</summary>
        [Description(
            "The credentials supplied were not complete, and could not be verified. Additional information can be returned from the context."
            )] public const int SEC_I_INCOMPLETE_CREDENTIALS = unchecked((int) 0x00090320);

        ///<summary>
        ///The context data must be renegotiated with the peer.
        ///</summary>
        [Description("The context data must be renegotiated with the peer.")] public const int SEC_I_RENEGOTIATE =
            unchecked((int) 0x00090321);

        ///<summary>
        ///The target principal name is incorrect.
        ///</summary>
        [Description("The target principal name is incorrect.")] public const int SEC_E_WRONG_PRINCIPAL =
            unchecked((int) 0x80090322);

        ///<summary>
        ///There is no LSA mode context associated with this context.
        ///</summary>
        [Description("There is no LSA mode context associated with this context.")] public const int
            SEC_I_NO_LSA_CONTEXT = unchecked((int) 0x00090323);

        ///<summary>
        ///The clocks on the client and server machines are skewed.
        ///</summary>
        [Description("The clocks on the client and server machines are skewed.")] public const int SEC_E_TIME_SKEW =
            unchecked((int) 0x80090324);

        ///<summary>
        ///The certificate chain was issued by an authority that is not trusted.
        ///</summary>
        [Description("The certificate chain was issued by an authority that is not trusted.")] public const int
            SEC_E_UNTRUSTED_ROOT = unchecked((int) 0x80090325);

        ///<summary>
        ///The message received was unexpected or badly formatted.
        ///</summary>
        [Description("The message received was unexpected or badly formatted.")] public const int SEC_E_ILLEGAL_MESSAGE
            = unchecked((int) 0x80090326);

        ///<summary>
        ///An unknown error occurred while processing the certificate.
        ///</summary>
        [Description("An unknown error occurred while processing the certificate.")] public const int SEC_E_CERT_UNKNOWN
            = unchecked((int) 0x80090327);

        ///<summary>
        ///The received certificate has expired.
        ///</summary>
        [Description("The received certificate has expired.")] public const int SEC_E_CERT_EXPIRED =
            unchecked((int) 0x80090328);

        ///<summary>
        ///The specified data could not be encrypted.
        ///</summary>
        [Description("The specified data could not be encrypted.")] public const int SEC_E_ENCRYPT_FAILURE =
            unchecked((int) 0x80090329);

        ///<summary>
        ///The specified data could not be decrypted.
        ///</summary>
        [Description("The specified data could not be decrypted.")] public const int SEC_E_DECRYPT_FAILURE =
            unchecked((int) 0x80090330);

        ///<summary>
        ///The client and server cannot communicate, because they do not possess a common algorithm.
        ///</summary>
        [Description("The client and server cannot communicate, because they do not possess a common algorithm.")] public const int SEC_E_ALGORITHM_MISMATCH = unchecked((int) 0x80090331);

        ///<summary>
        ///The security context could not be established due to a failure in the requested quality of service (e.g. mutual authentication or delegation).
        ///</summary>
        [Description(
            "The security context could not be established due to a failure in the requested quality of service (e.g. mutual authentication or delegation)."
            )] public const int SEC_E_SECURITY_QOS_FAILED = unchecked((int) 0x80090332);

        ///<summary>
        ///A security context was deleted before the context was completed.  This is considered a logon failure.
        ///</summary>
        [Description(
            "A security context was deleted before the context was completed.  This is considered a logon failure.")] public const int SEC_E_UNFINISHED_CONTEXT_DELETED = unchecked((int) 0x80090333);

        ///<summary>
        ///The client is trying to negotiate a context and the server requires user-to-user but didn't send a TGT reply.
        ///</summary>
        [Description(
            "The client is trying to negotiate a context and the server requires user-to-user but didn't send a TGT reply."
            )] public const int SEC_E_NO_TGT_REPLY = unchecked((int) 0x80090334);

        ///<summary>
        ///Unable to accomplish the requested task because the local machine does not have any IP addresses.
        ///</summary>
        [Description("Unable to accomplish the requested task because the local machine does not have any IP addresses."
            )] public const int SEC_E_NO_IP_ADDRESSES = unchecked((int) 0x80090335);

        ///<summary>
        ///The supplied credential handle does not match the credential associated with the security context.
        ///</summary>
        [Description(
            "The supplied credential handle does not match the credential associated with the security context.")] public const int SEC_E_WRONG_CREDENTIAL_HANDLE = unchecked((int) 0x80090336);

        ///<summary>
        ///The crypto system or checksum function is invalid because a required function is unavailable.
        ///</summary>
        [Description("The crypto system or checksum function is invalid because a required function is unavailable.")] public const int SEC_E_CRYPTO_SYSTEM_INVALID = unchecked((int) 0x80090337);

        ///<summary>
        ///The number of maximum ticket referrals has been exceeded.
        ///</summary>
        [Description("The number of maximum ticket referrals has been exceeded.")] public const int
            SEC_E_MAX_REFERRALS_EXCEEDED = unchecked((int) 0x80090338);

        ///<summary>
        ///The local machine must be a Kerberos KDC (domain controller) and it is not.
        ///</summary>
        [Description("The local machine must be a Kerberos KDC (domain controller) and it is not.")] public const int
            SEC_E_MUST_BE_KDC = unchecked((int) 0x80090339);

        ///<summary>
        ///The other end of the security negotiation is requires strong crypto but it is not supported on the local machine.
        ///</summary>
        [Description(
            "The other end of the security negotiation is requires strong crypto but it is not supported on the local machine."
            )] public const int SEC_E_STRONG_CRYPTO_NOT_SUPPORTED = unchecked((int) 0x8009033A);

        ///<summary>
        ///The KDC reply contained more than one principal name.
        ///</summary>
        [Description("The KDC reply contained more than one principal name.")] public const int
            SEC_E_TOO_MANY_PRINCIPALS = unchecked((int) 0x8009033B);

        ///<summary>
        ///Expected to find PA data for a hint of what etype to use, but it was not found.
        ///</summary>
        [Description("Expected to find PA data for a hint of what etype to use, but it was not found.")] public const
            int SEC_E_NO_PA_DATA = unchecked((int) 0x8009033C);

        ///<summary>
        ///The client certificate does not contain a valid UPN, or does not match the client name in the logon request.  Please contact your administrator.
        ///</summary>
        [Description(
            "The client certificate does not contain a valid UPN, or does not match the client name in the logon request.  Please contact your administrator."
            )] public const int SEC_E_PKINIT_NAME_MISMATCH = unchecked((int) 0x8009033D);

        ///<summary>
        ///Smartcard logon is required and was not used.
        ///</summary>
        [Description("Smartcard logon is required and was not used.")] public const int SEC_E_SMARTCARD_LOGON_REQUIRED =
            unchecked((int) 0x8009033E);

        ///<summary>
        ///A system shutdown is in progress.
        ///</summary>
        [Description("A system shutdown is in progress.")] public const int SEC_E_SHUTDOWN_IN_PROGRESS =
            unchecked((int) 0x8009033F);

        ///<summary>
        ///An invalid request was sent to the KDC.
        ///</summary>
        [Description("An invalid request was sent to the KDC.")] public const int SEC_E_KDC_INVALID_REQUEST =
            unchecked((int) 0x80090340);

        ///<summary>
        ///The KDC was unable to generate a referral for the service requested.
        ///</summary>
        [Description("The KDC was unable to generate a referral for the service requested.")] public const int
            SEC_E_KDC_UNABLE_TO_REFER = unchecked((int) 0x80090341);

        ///<summary>
        ///The encryption type requested is not supported by the KDC.
        ///</summary>
        [Description("The encryption type requested is not supported by the KDC.")] public const int
            SEC_E_KDC_UNKNOWN_ETYPE = unchecked((int) 0x80090342);

        ///<summary>
        ///An unsupported preauthentication mechanism was presented to the kerberos package.
        ///</summary>
        [Description("An unsupported preauthentication mechanism was presented to the kerberos package.")] public const
            int SEC_E_UNSUPPORTED_PREAUTH = unchecked((int) 0x80090343);

        ///<summary>
        ///The requested operation cannot be completed.  The computer must be trusted for delegation and the current user account must be configured to allow delegation.
        ///</summary>
        [Description(
            "The requested operation cannot be completed.  The computer must be trusted for delegation and the current user account must be configured to allow delegation."
            )] public const int SEC_E_DELEGATION_REQUIRED = unchecked((int) 0x80090345);

        ///<summary>
        ///Client's supplied SSPI channel bindings were incorrect.
        ///</summary>
        [Description("Client's supplied SSPI channel bindings were incorrect.")] public const int SEC_E_BAD_BINDINGS =
            unchecked((int) 0x80090346);

        ///<summary>
        ///The received certificate was mapped to multiple accounts.
        ///</summary>
        [Description("The received certificate was mapped to multiple accounts.")] public const int
            SEC_E_MULTIPLE_ACCOUNTS = unchecked((int) 0x80090347);

        ///<summary>
        ///SEC_E_NO_KERB_KEY
        ///</summary>
        [Description("SEC_E_NO_KERB_KEY")] public const int SEC_E_NO_KERB_KEY = unchecked((int) 0x80090348);

        ///<summary>
        ///The certificate is not valid for the requested usage.
        ///</summary>
        [Description("The certificate is not valid for the requested usage.")] public const int SEC_E_CERT_WRONG_USAGE =
            unchecked((int) 0x80090349);

        ///<summary>
        ///The system detected a possible attempt to compromise security.  Please ensure that you can contact the server that authenticated you.
        ///</summary>
        [Description(
            "The system detected a possible attempt to compromise security.  Please ensure that you can contact the server that authenticated you."
            )] public const int SEC_E_DOWNGRADE_DETECTED = unchecked((int) 0x80090350);

        ///<summary>
        ///The smartcard certificate used for authentication has been revoked. Please contact your system administrator.  There may be additional information in the event log.
        ///</summary>
        [Description(
            "The smartcard certificate used for authentication has been revoked. Please contact your system administrator.  There may be additional information in the event log."
            )] public const int SEC_E_SMARTCARD_CERT_REVOKED = unchecked((int) 0x80090351);

        ///<summary>
        ///An untrusted certificate authority was detected While processing the smartcard certificate used for authentication.  Please contact your system administrator.
        ///</summary>
        [Description(
            "An untrusted certificate authority was detected While processing the smartcard certificate used for authentication.  Please contact your system administrator."
            )] public const int SEC_E_ISSUING_CA_UNTRUSTED = unchecked((int) 0x80090352);

        ///<summary>
        ///The revocation status of the smartcard certificate used for authentication could not be determined. Please contact your system administrator.
        ///</summary>
        [Description(
            "The revocation status of the smartcard certificate used for authentication could not be determined. Please contact your system administrator."
            )] public const int SEC_E_REVOCATION_OFFLINE_C = unchecked((int) 0x80090353);

        ///<summary>
        ///The smartcard certificate used for authentication was not trusted.  Please contact your system administrator.
        ///</summary>
        [Description(
            "The smartcard certificate used for authentication was not trusted.  Please contact your system administrator."
            )] public const int SEC_E_PKINIT_CLIENT_FAILURE = unchecked((int) 0x80090354);

        ///<summary>
        ///The smartcard certificate used for authentication has expired.  Please contact your system administrator.
        ///</summary>
        [Description(
            "The smartcard certificate used for authentication has expired.  Please contact your system administrator.")
        ] public const int SEC_E_SMARTCARD_CERT_EXPIRED = unchecked((int) 0x80090355);

        ///<summary>
        ///The Kerberos subsystem encountered an error.  A service for user protocol request was made against a domain controller which does not support service for user.
        ///</summary>
        [Description(
            "The Kerberos subsystem encountered an error.  A service for user protocol request was made against a domain controller which does not support service for user."
            )] public const int SEC_E_NO_S4U_PROT_SUPPORT = unchecked((int) 0x80090356);

        ///<summary>
        ///An attempt was made by this server to make a Kerberos constrained delegation request for a target outside of the server's realm.  This is not supported, and indicates a misconfiguration on this server's allowed to delegate to list.  Please contact your administrator.
        ///</summary>
        [Description(
            "An attempt was made by this server to make a Kerberos constrained delegation request for a target outside of the server's realm.  This is not supported, and indicates a misconfiguration on this server's allowed to delegate to list.  Please contact your administrator."
            )] public const int SEC_E_CROSSREALM_DELEGATION_FAILURE = unchecked((int) 0x80090357);

        //
        // Provided for backwards compatibility
        //

        public const int SEC_E_NO_SPM = SEC_E_INTERNAL_ERROR;
        public const int SEC_E_NOT_SUPPORTED = SEC_E_UNSUPPORTED_FUNCTION;

        ///<summary>
        ///An error occurred while performing an operation on a cryptographic message.
        ///</summary>
        [Description("An error occurred while performing an operation on a cryptographic message.")] public const int
            CRYPT_E_MSG_ERROR = unchecked((int) 0x80091001);

        ///<summary>
        ///Unknown cryptographic algorithm.
        ///</summary>
        [Description("Unknown cryptographic algorithm.")] public const int CRYPT_E_UNKNOWN_ALGO =
            unchecked((int) 0x80091002);

        ///<summary>
        ///The object identifier is poorly formatted.
        ///</summary>
        [Description("The object identifier is poorly formatted.")] public const int CRYPT_E_OID_FORMAT =
            unchecked((int) 0x80091003);

        ///<summary>
        ///Invalid cryptographic message type.
        ///</summary>
        [Description("Invalid cryptographic message type.")] public const int CRYPT_E_INVALID_MSG_TYPE =
            unchecked((int) 0x80091004);

        ///<summary>
        ///Unexpected cryptographic message encoding.
        ///</summary>
        [Description("Unexpected cryptographic message encoding.")] public const int CRYPT_E_UNEXPECTED_ENCODING =
            unchecked((int) 0x80091005);

        ///<summary>
        ///The cryptographic message does not contain an expected authenticated attribute.
        ///</summary>
        [Description("The cryptographic message does not contain an expected authenticated attribute.")] public const
            int CRYPT_E_AUTH_ATTR_MISSING = unchecked((int) 0x80091006);

        ///<summary>
        ///The hash value is not correct.
        ///</summary>
        [Description("The hash value is not correct.")] public const int CRYPT_E_HASH_VALUE =
            unchecked((int) 0x80091007);

        ///<summary>
        ///The index value is not valid.
        ///</summary>
        [Description("The index value is not valid.")] public const int CRYPT_E_INVALID_INDEX =
            unchecked((int) 0x80091008);

        ///<summary>
        ///The content of the cryptographic message has already been decrypted.
        ///</summary>
        [Description("The content of the cryptographic message has already been decrypted.")] public const int
            CRYPT_E_ALREADY_DECRYPTED = unchecked((int) 0x80091009);

        ///<summary>
        ///The content of the cryptographic message has not been decrypted yet.
        ///</summary>
        [Description("The content of the cryptographic message has not been decrypted yet.")] public const int
            CRYPT_E_NOT_DECRYPTED = unchecked((int) 0x8009100A);

        ///<summary>
        ///The enveloped-data message does not contain the specified recipient.
        ///</summary>
        [Description("The enveloped-data message does not contain the specified recipient.")] public const int
            CRYPT_E_RECIPIENT_NOT_FOUND = unchecked((int) 0x8009100B);

        ///<summary>
        ///Invalid control type.
        ///</summary>
        [Description("Invalid control type.")] public const int CRYPT_E_CONTROL_TYPE = unchecked((int) 0x8009100C);

        ///<summary>
        ///Invalid issuer and/or serial number.
        ///</summary>
        [Description("Invalid issuer and/or serial number.")] public const int CRYPT_E_ISSUER_SERIALNUMBER =
            unchecked((int) 0x8009100D);

        ///<summary>
        ///Cannot find the original signer.
        ///</summary>
        [Description("Cannot find the original signer.")] public const int CRYPT_E_SIGNER_NOT_FOUND =
            unchecked((int) 0x8009100E);

        ///<summary>
        ///The cryptographic message does not contain all of the requested attributes.
        ///</summary>
        [Description("The cryptographic message does not contain all of the requested attributes.")] public const int
            CRYPT_E_ATTRIBUTES_MISSING = unchecked((int) 0x8009100F);

        ///<summary>
        ///The streamed cryptographic message is not ready to return data.
        ///</summary>
        [Description("The streamed cryptographic message is not ready to return data.")] public const int
            CRYPT_E_STREAM_MSG_NOT_READY = unchecked((int) 0x80091010);

        ///<summary>
        ///The streamed cryptographic message requires more data to complete the decode operation.
        ///</summary>
        [Description("The streamed cryptographic message requires more data to complete the decode operation.")] public
            const int CRYPT_E_STREAM_INSUFFICIENT_DATA = unchecked((int) 0x80091011);

        ///<summary>
        ///The protected data needs to be re-protected.
        ///</summary>
        [Description("The protected data needs to be re-protected.")] public const int CRYPT_I_NEW_PROTECTION_REQUIRED =
            unchecked((int) 0x00091012);

        ///<summary>
        ///The length specified for the output data was insufficient.
        ///</summary>
        [Description("The length specified for the output data was insufficient.")] public const int CRYPT_E_BAD_LEN =
            unchecked((int) 0x80092001);

        ///<summary>
        ///An error occurred during encode or decode operation.
        ///</summary>
        [Description("An error occurred during encode or decode operation.")] public const int CRYPT_E_BAD_ENCODE =
            unchecked((int) 0x80092002);

        ///<summary>
        ///An error occurred while reading or writing to a file.
        ///</summary>
        [Description("An error occurred while reading or writing to a file.")] public const int CRYPT_E_FILE_ERROR =
            unchecked((int) 0x80092003);

        ///<summary>
        ///Cannot find object or property.
        ///</summary>
        [Description("Cannot find object or property.")] public const int CRYPT_E_NOT_FOUND =
            unchecked((int) 0x80092004);

        ///<summary>
        ///The object or property already exists.
        ///</summary>
        [Description("The object or property already exists.")] public const int CRYPT_E_EXISTS =
            unchecked((int) 0x80092005);

        ///<summary>
        ///No provider was specified for the store or object.
        ///</summary>
        [Description("No provider was specified for the store or object.")] public const int CRYPT_E_NO_PROVIDER =
            unchecked((int) 0x80092006);

        ///<summary>
        ///The specified certificate is self signed.
        ///</summary>
        [Description("The specified certificate is self signed.")] public const int CRYPT_E_SELF_SIGNED =
            unchecked((int) 0x80092007);

        ///<summary>
        ///The previous certificate or CRL context was deleted.
        ///</summary>
        [Description("The previous certificate or CRL context was deleted.")] public const int CRYPT_E_DELETED_PREV =
            unchecked((int) 0x80092008);

        ///<summary>
        ///Cannot find the requested object.
        ///</summary>
        [Description("Cannot find the requested object.")] public const int CRYPT_E_NO_MATCH =
            unchecked((int) 0x80092009);

        ///<summary>
        ///The certificate does not have a property that references a private key.
        ///</summary>
        [Description("The certificate does not have a property that references a private key.")] public const int
            CRYPT_E_UNEXPECTED_MSG_TYPE = unchecked((int) 0x8009200A);

        ///<summary>
        ///Cannot find the certificate and private key for decryption.
        ///</summary>
        [Description("Cannot find the certificate and private key for decryption.")] public const int
            CRYPT_E_NO_KEY_PROPERTY = unchecked((int) 0x8009200B);

        ///<summary>
        ///Cannot find the certificate and private key to use for decryption.
        ///</summary>
        [Description("Cannot find the certificate and private key to use for decryption.")] public const int
            CRYPT_E_NO_DECRYPT_CERT = unchecked((int) 0x8009200C);

        ///<summary>
        ///Not a cryptographic message or the cryptographic message is not formatted correctly.
        ///</summary>
        [Description("Not a cryptographic message or the cryptographic message is not formatted correctly.")] public
            const int CRYPT_E_BAD_MSG = unchecked((int) 0x8009200D);

        ///<summary>
        ///The signed cryptographic message does not have a signer for the specified signer index.
        ///</summary>
        [Description("The signed cryptographic message does not have a signer for the specified signer index.")] public
            const int CRYPT_E_NO_SIGNER = unchecked((int) 0x8009200E);

        ///<summary>
        ///Final closure is pending until additional frees or closes.
        ///</summary>
        [Description("Final closure is pending until additional frees or closes.")] public const int
            CRYPT_E_PENDING_CLOSE = unchecked((int) 0x8009200F);

        ///<summary>
        ///The certificate is revoked.
        ///</summary>
        [Description("The certificate is revoked.")] public const int CRYPT_E_REVOKED = unchecked((int) 0x80092010);

        ///<summary>
        ///No Dll or exported function was found to verify revocation.
        ///</summary>
        [Description("No Dll or exported function was found to verify revocation.")] public const int
            CRYPT_E_NO_REVOCATION_DLL = unchecked((int) 0x80092011);

        ///<summary>
        ///The revocation function was unable to check revocation for the certificate.
        ///</summary>
        [Description("The revocation function was unable to check revocation for the certificate.")] public const int
            CRYPT_E_NO_REVOCATION_CHECK = unchecked((int) 0x80092012);

        ///<summary>
        ///The revocation function was unable to check revocation because the revocation server was offline.
        ///</summary>
        [Description("The revocation function was unable to check revocation because the revocation server was offline."
            )] public const int CRYPT_E_REVOCATION_OFFLINE = unchecked((int) 0x80092013);

        ///<summary>
        ///The certificate is not in the revocation server's database.
        ///</summary>
        [Description("The certificate is not in the revocation server's database.")] public const int
            CRYPT_E_NOT_IN_REVOCATION_DATABASE = unchecked((int) 0x80092014);

        ///<summary>
        ///The string contains a non-numeric character.
        ///</summary>
        [Description("The string contains a non-numeric character.")] public const int CRYPT_E_INVALID_NUMERIC_STRING =
            unchecked((int) 0x80092020);

        ///<summary>
        ///The string contains a non-printable character.
        ///</summary>
        [Description("The string contains a non-printable character.")] public const int
            CRYPT_E_INVALID_PRINTABLE_STRING = unchecked((int) 0x80092021);

        ///<summary>
        ///The string contains a character not in the 7 bit ASCII character set.
        ///</summary>
        [Description("The string contains a character not in the 7 bit ASCII character set.")] public const int
            CRYPT_E_INVALID_IA5_STRING = unchecked((int) 0x80092022);

        ///<summary>
        ///The string contains an invalid X500 name attribute key, oid, value or delimiter.
        ///</summary>
        [Description("The string contains an invalid X500 name attribute key, oid, value or delimiter.")] public const
            int CRYPT_E_INVALID_X500_STRING = unchecked((int) 0x80092023);

        ///<summary>
        ///The dwValueType for the CERT_NAME_VALUE is not one of the character strings.  Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING.
        ///</summary>
        [Description(
            "The dwValueType for the CERT_NAME_VALUE is not one of the character strings.  Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING."
            )] public const int CRYPT_E_NOT_CHAR_STRING = unchecked((int) 0x80092024);

        ///<summary>
        ///The Put operation can not continue.  The file needs to be resized.  However, there is already a signature present.  A complete signing operation must be done.
        ///</summary>
        [Description(
            "The Put operation can not continue.  The file needs to be resized.  However, there is already a signature present.  A complete signing operation must be done."
            )] public const int CRYPT_E_FILERESIZED = unchecked((int) 0x80092025);

        ///<summary>
        ///The cryptographic operation failed due to a local security option setting.
        ///</summary>
        [Description("The cryptographic operation failed due to a local security option setting.")] public const int
            CRYPT_E_SECURITY_SETTINGS = unchecked((int) 0x80092026);

        ///<summary>
        ///No DLL or exported function was found to verify subject usage.
        ///</summary>
        [Description("No DLL or exported function was found to verify subject usage.")] public const int
            CRYPT_E_NO_VERIFY_USAGE_DLL = unchecked((int) 0x80092027);

        ///<summary>
        ///The called function was unable to do a usage check on the subject.
        ///</summary>
        [Description("The called function was unable to do a usage check on the subject.")] public const int
            CRYPT_E_NO_VERIFY_USAGE_CHECK = unchecked((int) 0x80092028);

        ///<summary>
        ///Since the server was offline, the called function was unable to complete the usage check.
        ///</summary>
        [Description("Since the server was offline, the called function was unable to complete the usage check.")] public const int CRYPT_E_VERIFY_USAGE_OFFLINE = unchecked((int) 0x80092029);

        ///<summary>
        ///The subject was not found in a Certificate Trust List (CTL).
        ///</summary>
        [Description("The subject was not found in a Certificate Trust List (CTL).")] public const int
            CRYPT_E_NOT_IN_CTL = unchecked((int) 0x8009202A);

        ///<summary>
        ///None of the signers of the cryptographic message or certificate trust list is trusted.
        ///</summary>
        [Description("None of the signers of the cryptographic message or certificate trust list is trusted.")] public
            const int CRYPT_E_NO_TRUSTED_SIGNER = unchecked((int) 0x8009202B);

        ///<summary>
        ///The public key's algorithm parameters are missing.
        ///</summary>
        [Description("The public key's algorithm parameters are missing.")] public const int CRYPT_E_MISSING_PUBKEY_PARA
            = unchecked((int) 0x8009202C);

        //  See asn1code.h for a definition of the OSS runtime errors. The OSS 
        //  error values are offset by CRYPT_E_OSS_ERROR.
        ///<summary>
        ///OSS Certificate encode/decode error code base
        ///</summary>
        [Description("OSS Certificate encode/decode error code base")] public const int CRYPT_E_OSS_ERROR =
            unchecked((int) 0x80093000);

        ///<summary>
        ///OSS ASN.1 Error: Output Buffer is too small.
        ///</summary>
        [Description("OSS ASN.1 Error: Output Buffer is too small.")] public const int OSS_MORE_BUF =
            unchecked((int) 0x80093001);

        ///<summary>
        ///OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.
        ///</summary>
        [Description("OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.")] public const int
            OSS_NEGATIVE_UINTEGER = unchecked((int) 0x80093002);

        ///<summary>
        ///OSS ASN.1 Error: Unknown ASN.1 data type.
        ///</summary>
        [Description("OSS ASN.1 Error: Unknown ASN.1 data type.")] public const int OSS_PDU_RANGE =
            unchecked((int) 0x80093003);

        ///<summary>
        ///OSS ASN.1 Error: Output buffer is too small, the decoded data has been truncated.
        ///</summary>
        [Description("OSS ASN.1 Error: Output buffer is too small, the decoded data has been truncated.")] public const
            int OSS_MORE_INPUT = unchecked((int) 0x80093004);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_DATA_ERROR = unchecked((int) 0x80093005);

        ///<summary>
        ///OSS ASN.1 Error: Invalid argument.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid argument.")] public const int OSS_BAD_ARG = unchecked((int) 0x80093006);

        ///<summary>
        ///OSS ASN.1 Error: Encode/Decode version mismatch.
        ///</summary>
        [Description("OSS ASN.1 Error: Encode/Decode version mismatch.")] public const int OSS_BAD_VERSION =
            unchecked((int) 0x80093007);

        ///<summary>
        ///OSS ASN.1 Error: Out of memory.
        ///</summary>
        [Description("OSS ASN.1 Error: Out of memory.")] public const int OSS_OUT_MEMORY = unchecked((int) 0x80093008);

        ///<summary>
        ///OSS ASN.1 Error: Encode/Decode Error.
        ///</summary>
        [Description("OSS ASN.1 Error: Encode/Decode Error.")] public const int OSS_PDU_MISMATCH =
            unchecked((int) 0x80093009);

        ///<summary>
        ///OSS ASN.1 Error: Internal Error.
        ///</summary>
        [Description("OSS ASN.1 Error: Internal Error.")] public const int OSS_LIMITED = unchecked((int) 0x8009300A);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_BAD_PTR = unchecked((int) 0x8009300B);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_BAD_TIME = unchecked((int) 0x8009300C);

        ///<summary>
        ///OSS ASN.1 Error: Unsupported BER indefinite-length encoding.
        ///</summary>
        [Description("OSS ASN.1 Error: Unsupported BER indefinite-length encoding.")] public const int
            OSS_INDEFINITE_NOT_SUPPORTED = unchecked((int) 0x8009300D);

        ///<summary>
        ///OSS ASN.1 Error: Access violation.
        ///</summary>
        [Description("OSS ASN.1 Error: Access violation.")] public const int OSS_MEM_ERROR = unchecked((int) 0x8009300E);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_BAD_TABLE = unchecked((int) 0x8009300F);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_TOO_LONG = unchecked((int) 0x80093010);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_CONSTRAINT_VIOLATED =
            unchecked((int) 0x80093011);

        ///<summary>
        ///OSS ASN.1 Error: Internal Error.
        ///</summary>
        [Description("OSS ASN.1 Error: Internal Error.")] public const int OSS_FATAL_ERROR = unchecked((int) 0x80093012);

        ///<summary>
        ///OSS ASN.1 Error: Multi-threading conflict.
        ///</summary>
        [Description("OSS ASN.1 Error: Multi-threading conflict.")] public const int OSS_ACCESS_SERIALIZATION_ERROR =
            unchecked((int) 0x80093013);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_NULL_TBL = unchecked((int) 0x80093014);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_NULL_FCN = unchecked((int) 0x80093015);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_BAD_ENCRULES = unchecked((int) 0x80093016);

        ///<summary>
        ///OSS ASN.1 Error: Encode/Decode function not implemented.
        ///</summary>
        [Description("OSS ASN.1 Error: Encode/Decode function not implemented.")] public const int OSS_UNAVAIL_ENCRULES
            = unchecked((int) 0x80093017);

        ///<summary>
        ///OSS ASN.1 Error: Trace file error.
        ///</summary>
        [Description("OSS ASN.1 Error: Trace file error.")] public const int OSS_CANT_OPEN_TRACE_WINDOW =
            unchecked((int) 0x80093018);

        ///<summary>
        ///OSS ASN.1 Error: Function not implemented.
        ///</summary>
        [Description("OSS ASN.1 Error: Function not implemented.")] public const int OSS_UNIMPLEMENTED =
            unchecked((int) 0x80093019);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_OID_DLL_NOT_LINKED =
            unchecked((int) 0x8009301A);

        ///<summary>
        ///OSS ASN.1 Error: Trace file error.
        ///</summary>
        [Description("OSS ASN.1 Error: Trace file error.")] public const int OSS_CANT_OPEN_TRACE_FILE =
            unchecked((int) 0x8009301B);

        ///<summary>
        ///OSS ASN.1 Error: Trace file error.
        ///</summary>
        [Description("OSS ASN.1 Error: Trace file error.")] public const int OSS_TRACE_FILE_ALREADY_OPEN =
            unchecked((int) 0x8009301C);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_TABLE_MISMATCH =
            unchecked((int) 0x8009301D);

        ///<summary>
        ///OSS ASN.1 Error: Invalid data.
        ///</summary>
        [Description("OSS ASN.1 Error: Invalid data.")] public const int OSS_TYPE_NOT_SUPPORTED =
            unchecked((int) 0x8009301E);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_REAL_DLL_NOT_LINKED =
            unchecked((int) 0x8009301F);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_REAL_CODE_NOT_LINKED =
            unchecked((int) 0x80093020);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_OUT_OF_RANGE =
            unchecked((int) 0x80093021);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_COPIER_DLL_NOT_LINKED =
            unchecked((int) 0x80093022);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_CONSTRAINT_DLL_NOT_LINKED =
            unchecked((int) 0x80093023);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_COMPARATOR_DLL_NOT_LINKED =
            unchecked((int) 0x80093024);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_COMPARATOR_CODE_NOT_LINKED =
            unchecked((int) 0x80093025);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_MEM_MGR_DLL_NOT_LINKED =
            unchecked((int) 0x80093026);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_PDV_DLL_NOT_LINKED =
            unchecked((int) 0x80093027);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_PDV_CODE_NOT_LINKED =
            unchecked((int) 0x80093028);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_API_DLL_NOT_LINKED =
            unchecked((int) 0x80093029);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_BERDER_DLL_NOT_LINKED =
            unchecked((int) 0x8009302A);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_PER_DLL_NOT_LINKED =
            unchecked((int) 0x8009302B);

        ///<summary>
        ///OSS ASN.1 Error: Program link error.
        ///</summary>
        [Description("OSS ASN.1 Error: Program link error.")] public const int OSS_OPEN_TYPE_ERROR =
            unchecked((int) 0x8009302C);

        ///<summary>
        ///OSS ASN.1 Error: System resource error.
        ///</summary>
        [Description("OSS ASN.1 Error: System resource error.")] public const int OSS_MUTEX_NOT_CREATED =
            unchecked((int) 0x8009302D);

        ///<summary>
        ///OSS ASN.1 Error: Trace file error.
        ///</summary>
        [Description("OSS ASN.1 Error: Trace file error.")] public const int OSS_CANT_CLOSE_TRACE_FILE =
            unchecked((int) 0x8009302E);

        //  The ASN1 error values are offset by CRYPT_E_ASN1_ERROR.
        ///<summary>
        ///ASN1 Certificate encode/decode error code base.
        ///</summary>
        [Description("ASN1 Certificate encode/decode error code base.")] public const int CRYPT_E_ASN1_ERROR =
            unchecked((int) 0x80093100);

        ///<summary>
        ///ASN1 internal encode or decode error.
        ///</summary>
        [Description("ASN1 internal encode or decode error.")] public const int CRYPT_E_ASN1_INTERNAL =
            unchecked((int) 0x80093101);

        ///<summary>
        ///ASN1 unexpected end of data.
        ///</summary>
        [Description("ASN1 unexpected end of data.")] public const int CRYPT_E_ASN1_EOD = unchecked((int) 0x80093102);

        ///<summary>
        ///ASN1 corrupted data.
        ///</summary>
        [Description("ASN1 corrupted data.")] public const int CRYPT_E_ASN1_CORRUPT = unchecked((int) 0x80093103);

        ///<summary>
        ///ASN1 value too large.
        ///</summary>
        [Description("ASN1 value too large.")] public const int CRYPT_E_ASN1_LARGE = unchecked((int) 0x80093104);

        ///<summary>
        ///ASN1 constraint violated.
        ///</summary>
        [Description("ASN1 constraint violated.")] public const int CRYPT_E_ASN1_CONSTRAINT =
            unchecked((int) 0x80093105);

        ///<summary>
        ///ASN1 out of memory.
        ///</summary>
        [Description("ASN1 out of memory.")] public const int CRYPT_E_ASN1_MEMORY = unchecked((int) 0x80093106);

        ///<summary>
        ///ASN1 buffer overflow.
        ///</summary>
        [Description("ASN1 buffer overflow.")] public const int CRYPT_E_ASN1_OVERFLOW = unchecked((int) 0x80093107);

        ///<summary>
        ///ASN1 function not supported for this PDU.
        ///</summary>
        [Description("ASN1 function not supported for this PDU.")] public const int CRYPT_E_ASN1_BADPDU =
            unchecked((int) 0x80093108);

        ///<summary>
        ///ASN1 bad arguments to function call.
        ///</summary>
        [Description("ASN1 bad arguments to function call.")] public const int CRYPT_E_ASN1_BADARGS =
            unchecked((int) 0x80093109);

        ///<summary>
        ///ASN1 bad real value.
        ///</summary>
        [Description("ASN1 bad real value.")] public const int CRYPT_E_ASN1_BADREAL = unchecked((int) 0x8009310A);

        ///<summary>
        ///ASN1 bad tag value met.
        ///</summary>
        [Description("ASN1 bad tag value met.")] public const int CRYPT_E_ASN1_BADTAG = unchecked((int) 0x8009310B);

        ///<summary>
        ///ASN1 bad choice value.
        ///</summary>
        [Description("ASN1 bad choice value.")] public const int CRYPT_E_ASN1_CHOICE = unchecked((int) 0x8009310C);

        ///<summary>
        ///ASN1 bad encoding rule.
        ///</summary>
        [Description("ASN1 bad encoding rule.")] public const int CRYPT_E_ASN1_RULE = unchecked((int) 0x8009310D);

        ///<summary>
        ///ASN1 bad unicode (UTF8).
        ///</summary>
        [Description("ASN1 bad unicode (UTF8).")] public const int CRYPT_E_ASN1_UTF8 = unchecked((int) 0x8009310E);

        ///<summary>
        ///ASN1 bad PDU type.
        ///</summary>
        [Description("ASN1 bad PDU type.")] public const int CRYPT_E_ASN1_PDU_TYPE = unchecked((int) 0x80093133);

        ///<summary>
        ///ASN1 not yet implemented.
        ///</summary>
        [Description("ASN1 not yet implemented.")] public const int CRYPT_E_ASN1_NYI = unchecked((int) 0x80093134);

        ///<summary>
        ///ASN1 skipped unknown extension(s).
        ///</summary>
        [Description("ASN1 skipped unknown extension(s).")] public const int CRYPT_E_ASN1_EXTENDED =
            unchecked((int) 0x80093201);

        ///<summary>
        ///ASN1 end of data expected
        ///</summary>
        [Description("ASN1 end of data expected")] public const int CRYPT_E_ASN1_NOEOD = unchecked((int) 0x80093202);

        ///<summary>
        ///The request subject name is invalid or too long.
        ///</summary>
        [Description("The request subject name is invalid or too long.")] public const int CERTSRV_E_BAD_REQUESTSUBJECT
            = unchecked((int) 0x80094001);

        ///<summary>
        ///The request does not exist.
        ///</summary>
        [Description("The request does not exist.")] public const int CERTSRV_E_NO_REQUEST = unchecked((int) 0x80094002);

        ///<summary>
        ///The request's current status does not allow this operation.
        ///</summary>
        [Description("The request's current status does not allow this operation.")] public const int
            CERTSRV_E_BAD_REQUESTSTATUS = unchecked((int) 0x80094003);

        ///<summary>
        ///The requested property value is empty.
        ///</summary>
        [Description("The requested property value is empty.")] public const int CERTSRV_E_PROPERTY_EMPTY =
            unchecked((int) 0x80094004);

        ///<summary>
        ///The certification authority's certificate contains invalid data.
        ///</summary>
        [Description("The certification authority's certificate contains invalid data.")] public const int
            CERTSRV_E_INVALID_CA_CERTIFICATE = unchecked((int) 0x80094005);

        ///<summary>
        ///Certificate service has been suspended for a database restore operation.
        ///</summary>
        [Description("Certificate service has been suspended for a database restore operation.")] public const int
            CERTSRV_E_SERVER_SUSPENDED = unchecked((int) 0x80094006);

        ///<summary>
        ///The certificate contains an encoded length that is potentially incompatible with older enrollment software.
        ///</summary>
        [Description(
            "The certificate contains an encoded length that is potentially incompatible with older enrollment software."
            )] public const int CERTSRV_E_ENCODING_LENGTH = unchecked((int) 0x80094007);

        ///<summary>
        ///The operation is denied. The user has multiple roles assigned and the certification authority is configured to enforce role separation.
        ///</summary>
        [Description(
            "The operation is denied. The user has multiple roles assigned and the certification authority is configured to enforce role separation."
            )] public const int CERTSRV_E_ROLECONFLICT = unchecked((int) 0x80094008);

        ///<summary>
        ///The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester.
        ///</summary>
        [Description(
            "The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester."
            )] public const int CERTSRV_E_RESTRICTEDOFFICER = unchecked((int) 0x80094009);

        ///<summary>
        ///Cannot archive private key.  The certification authority is not configured for key archival.
        ///</summary>
        [Description("Cannot archive private key.  The certification authority is not configured for key archival.")] public const int CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED = unchecked((int) 0x8009400A);

        ///<summary>
        ///Cannot archive private key.  The certification authority could not verify one or more key recovery certificates.
        ///</summary>
        [Description(
            "Cannot archive private key.  The certification authority could not verify one or more key recovery certificates."
            )] public const int CERTSRV_E_NO_VALID_KRA = unchecked((int) 0x8009400B);

        ///<summary>
        ///The request is incorrectly formatted.  The encrypted private key must be in an unauthenticated attribute in an outermost signature.
        ///</summary>
        [Description(
            "The request is incorrectly formatted.  The encrypted private key must be in an unauthenticated attribute in an outermost signature."
            )] public const int CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL = unchecked((int) 0x8009400C);

        ///<summary>
        ///At least one security principal must have the permission to manage this CA.
        ///</summary>
        [Description("At least one security principal must have the permission to manage this CA.")] public const int
            CERTSRV_E_NO_CAADMIN_DEFINED = unchecked((int) 0x8009400D);

        ///<summary>
        ///The request contains an invalid renewal certificate attribute.
        ///</summary>
        [Description("The request contains an invalid renewal certificate attribute.")] public const int
            CERTSRV_E_BAD_RENEWAL_CERT_ATTRIBUTE = unchecked((int) 0x8009400E);

        ///<summary>
        ///An attempt was made to open a Certification Authority database session, but there are already too many active sessions.  The server may need to be configured to allow additional sessions.
        ///</summary>
        [Description(
            "An attempt was made to open a Certification Authority database session, but there are already too many active sessions.  The server may need to be configured to allow additional sessions."
            )] public const int CERTSRV_E_NO_DB_SESSIONS = unchecked((int) 0x8009400F);

        ///<summary>
        ///A memory reference caused a data alignment fault.
        ///</summary>
        [Description("A memory reference caused a data alignment fault.")] public const int CERTSRV_E_ALIGNMENT_FAULT =
            unchecked((int) 0x80094010);

        ///<summary>
        ///The permissions on this certification authority do not allow the current user to enroll for certificates.
        ///</summary>
        [Description(
            "The permissions on this certification authority do not allow the current user to enroll for certificates.")
        ] public const int CERTSRV_E_ENROLL_DENIED = unchecked((int) 0x80094011);

        ///<summary>
        ///The permissions on the certificate template do not allow the current user to enroll for this type of certificate.
        ///</summary>
        [Description(
            "The permissions on the certificate template do not allow the current user to enroll for this type of certificate."
            )] public const int CERTSRV_E_TEMPLATE_DENIED = unchecked((int) 0x80094012);

        ///<summary>
        ///The contacted domain controller cannot support signed LDAP traffic.  Update the domain controller or configure Certificate Services to use SSL for Active Directory access.
        ///</summary>
        [Description(
            "The contacted domain controller cannot support signed LDAP traffic.  Update the domain controller or configure Certificate Services to use SSL for Active Directory access."
            )] public const int CERTSRV_E_DOWNLEVEL_DC_SSL_OR_UPGRADE = unchecked((int) 0x80094013);

        ///<summary>
        ///The requested certificate template is not supported by this CA.
        ///</summary>
        [Description("The requested certificate template is not supported by this CA.")] public const int
            CERTSRV_E_UNSUPPORTED_CERT_TYPE = unchecked((int) 0x80094800);

        ///<summary>
        ///The request contains no certificate template information.
        ///</summary>
        [Description("The request contains no certificate template information.")] public const int
            CERTSRV_E_NO_CERT_TYPE = unchecked((int) 0x80094801);

        ///<summary>
        ///The request contains conflicting template information.
        ///</summary>
        [Description("The request contains conflicting template information.")] public const int
            CERTSRV_E_TEMPLATE_CONFLICT = unchecked((int) 0x80094802);

        ///<summary>
        ///The request is missing a required Subject Alternate name extension.
        ///</summary>
        [Description("The request is missing a required Subject Alternate name extension.")] public const int
            CERTSRV_E_SUBJECT_ALT_NAME_REQUIRED = unchecked((int) 0x80094803);

        ///<summary>
        ///The request is missing a required private key for archival by the server.
        ///</summary>
        [Description("The request is missing a required private key for archival by the server.")] public const int
            CERTSRV_E_ARCHIVED_KEY_REQUIRED = unchecked((int) 0x80094804);

        ///<summary>
        ///The request is missing a required SMIME capabilities extension.
        ///</summary>
        [Description("The request is missing a required SMIME capabilities extension.")] public const int
            CERTSRV_E_SMIME_REQUIRED = unchecked((int) 0x80094805);

        ///<summary>
        ///The request was made on behalf of a subject other than the caller.  The certificate template must be configured to require at least one signature to authorize the request.
        ///</summary>
        [Description(
            "The request was made on behalf of a subject other than the caller.  The certificate template must be configured to require at least one signature to authorize the request."
            )] public const int CERTSRV_E_BAD_RENEWAL_SUBJECT = unchecked((int) 0x80094806);

        ///<summary>
        ///The request template version is newer than the supported template version.
        ///</summary>
        [Description("The request template version is newer than the supported template version.")] public const int
            CERTSRV_E_BAD_TEMPLATE_VERSION = unchecked((int) 0x80094807);

        ///<summary>
        ///The template is missing a required signature policy attribute.
        ///</summary>
        [Description("The template is missing a required signature policy attribute.")] public const int
            CERTSRV_E_TEMPLATE_POLICY_REQUIRED = unchecked((int) 0x80094808);

        ///<summary>
        ///The request is missing required signature policy information.
        ///</summary>
        [Description("The request is missing required signature policy information.")] public const int
            CERTSRV_E_SIGNATURE_POLICY_REQUIRED = unchecked((int) 0x80094809);

        ///<summary>
        ///The request is missing one or more required signatures.
        ///</summary>
        [Description("The request is missing one or more required signatures.")] public const int
            CERTSRV_E_SIGNATURE_COUNT = unchecked((int) 0x8009480A);

        ///<summary>
        ///One or more signatures did not include the required application or issuance policies.  The request is missing one or more required valid signatures.
        ///</summary>
        [Description(
            "One or more signatures did not include the required application or issuance policies.  The request is missing one or more required valid signatures."
            )] public const int CERTSRV_E_SIGNATURE_REJECTED = unchecked((int) 0x8009480B);

        ///<summary>
        ///The request is missing one or more required signature issuance policies.
        ///</summary>
        [Description("The request is missing one or more required signature issuance policies.")] public const int
            CERTSRV_E_ISSUANCE_POLICY_REQUIRED = unchecked((int) 0x8009480C);

        ///<summary>
        ///The UPN is unavailable and cannot be added to the Subject Alternate name.
        ///</summary>
        [Description("The UPN is unavailable and cannot be added to the Subject Alternate name.")] public const int
            CERTSRV_E_SUBJECT_UPN_REQUIRED = unchecked((int) 0x8009480D);

        ///<summary>
        ///The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.
        ///</summary>
        [Description("The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.")] public const int CERTSRV_E_SUBJECT_DIRECTORY_GUID_REQUIRED = unchecked((int) 0x8009480E);

        ///<summary>
        ///The DNS name is unavailable and cannot be added to the Subject Alternate name.
        ///</summary>
        [Description("The DNS name is unavailable and cannot be added to the Subject Alternate name.")] public const int
            CERTSRV_E_SUBJECT_DNS_REQUIRED = unchecked((int) 0x8009480F);

        ///<summary>
        ///The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template.
        ///</summary>
        [Description(
            "The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template."
            )] public const int CERTSRV_E_ARCHIVED_KEY_UNEXPECTED = unchecked((int) 0x80094810);

        ///<summary>
        ///The public key does not meet the minimum size required by the specified certificate template.
        ///</summary>
        [Description("The public key does not meet the minimum size required by the specified certificate template.")] public const int CERTSRV_E_KEY_LENGTH = unchecked((int) 0x80094811);

        ///<summary>
        ///The EMail name is unavailable and cannot be added to the Subject or Subject Alternate name.
        ///</summary>
        [Description("The EMail name is unavailable and cannot be added to the Subject or Subject Alternate name.")] public const int CERTSRV_E_SUBJECT_EMAIL_REQUIRED = unchecked((int) 0x80094812);

        ///<summary>
        ///One or more certificate templates to be enabled on this certification authority could not be found.
        ///</summary>
        [Description(
            "One or more certificate templates to be enabled on this certification authority could not be found.")] public const int CERTSRV_E_UNKNOWN_CERT_TYPE = unchecked((int) 0x80094813);

        ///<summary>
        ///The certificate template renewal period is longer than the certificate validity period.  The template should be reconfigured or the CA certificate renewed.
        ///</summary>
        [Description(
            "The certificate template renewal period is longer than the certificate validity period.  The template should be reconfigured or the CA certificate renewed."
            )] public const int CERTSRV_E_CERT_TYPE_OVERLAP = unchecked((int) 0x80094814);

        //
        // The range 0x5000-0x51ff is reserved for XENROLL errors.
        //
        ///<summary>
        ///The key is not exportable.
        ///</summary>
        [Description("The key is not exportable.")] public const int XENROLL_E_KEY_NOT_EXPORTABLE =
            unchecked((int) 0x80095000);

        ///<summary>
        ///You cannot add the root CA certificate into your local store.
        ///</summary>
        [Description("You cannot add the root CA certificate into your local store.")] public const int
            XENROLL_E_CANNOT_ADD_ROOT_CERT = unchecked((int) 0x80095001);

        ///<summary>
        ///The key archival hash attribute was not found in the response.
        ///</summary>
        [Description("The key archival hash attribute was not found in the response.")] public const int
            XENROLL_E_RESPONSE_KA_HASH_NOT_FOUND = unchecked((int) 0x80095002);

        ///<summary>
        ///An unexpected key archival hash attribute was found in the response.
        ///</summary>
        [Description("An unexpected key archival hash attribute was found in the response.")] public const int
            XENROLL_E_RESPONSE_UNEXPECTED_KA_HASH = unchecked((int) 0x80095003);

        ///<summary>
        ///There is a key archival hash mismatch between the request and the response.
        ///</summary>
        [Description("There is a key archival hash mismatch between the request and the response.")] public const int
            XENROLL_E_RESPONSE_KA_HASH_MISMATCH = unchecked((int) 0x80095004);

        ///<summary>
        ///Signing certificate cannot include SMIME extension.
        ///</summary>
        [Description("Signing certificate cannot include SMIME extension.")] public const int
            XENROLL_E_KEYSPEC_SMIME_MISMATCH = unchecked((int) 0x80095005);

        ///<summary>
        ///A system-level error occurred while verifying trust.
        ///</summary>
        [Description("A system-level error occurred while verifying trust.")] public const int TRUST_E_SYSTEM_ERROR =
            unchecked((int) 0x80096001);

        ///<summary>
        ///The certificate for the signer of the message is invalid or not found.
        ///</summary>
        [Description("The certificate for the signer of the message is invalid or not found.")] public const int
            TRUST_E_NO_SIGNER_CERT = unchecked((int) 0x80096002);

        ///<summary>
        ///One of the counter signatures was invalid.
        ///</summary>
        [Description("One of the counter signatures was invalid.")] public const int TRUST_E_COUNTER_SIGNER =
            unchecked((int) 0x80096003);

        ///<summary>
        ///The signature of the certificate can not be verified.
        ///</summary>
        [Description("The signature of the certificate can not be verified.")] public const int TRUST_E_CERT_SIGNATURE =
            unchecked((int) 0x80096004);

        ///<summary>
        ///The timestamp signature and/or certificate could not be verified or is malformed.
        ///</summary>
        [Description("The timestamp signature and/or certificate could not be verified or is malformed.")] public const
            int TRUST_E_TIME_STAMP = unchecked((int) 0x80096005);

        ///<summary>
        ///The digital signature of the object did not verify.
        ///</summary>
        [Description("The digital signature of the object did not verify.")] public const int TRUST_E_BAD_DIGEST =
            unchecked((int) 0x80096010);

        ///<summary>
        ///A certificate's basic constraint extension has not been observed.
        ///</summary>
        [Description("A certificate's basic constraint extension has not been observed.")] public const int
            TRUST_E_BASIC_CONSTRAINTS = unchecked((int) 0x80096019);

        ///<summary>
        ///The certificate does not meet or contain the Authenticode financial extensions.
        ///</summary>
        [Description("The certificate does not meet or contain the Authenticode financial extensions.")] public const
            int TRUST_E_FINANCIAL_CRITERIA = unchecked((int) 0x8009601E);

        //
        //  Error codes for mssipotf.dll
        //  Most of the error codes can only occur when an error occurs
        //    during font file signing
        //
        //
        ///<summary>
        ///Tried to reference a part of the file outside the proper range.
        ///</summary>
        [Description("Tried to reference a part of the file outside the proper range.")] public const int
            MSSIPOTF_E_OUTOFMEMRANGE = unchecked((int) 0x80097001);

        ///<summary>
        ///Could not retrieve an object from the file.
        ///</summary>
        [Description("Could not retrieve an object from the file.")] public const int MSSIPOTF_E_CANTGETOBJECT =
            unchecked((int) 0x80097002);

        ///<summary>
        ///Could not find the head table in the file.
        ///</summary>
        [Description("Could not find the head table in the file.")] public const int MSSIPOTF_E_NOHEADTABLE =
            unchecked((int) 0x80097003);

        ///<summary>
        ///The magic number in the head table is incorrect.
        ///</summary>
        [Description("The magic number in the head table is incorrect.")] public const int MSSIPOTF_E_BAD_MAGICNUMBER =
            unchecked((int) 0x80097004);

        ///<summary>
        ///The offset table has incorrect values.
        ///</summary>
        [Description("The offset table has incorrect values.")] public const int MSSIPOTF_E_BAD_OFFSET_TABLE =
            unchecked((int) 0x80097005);

        ///<summary>
        ///Duplicate table tags or tags out of alphabetical order.
        ///</summary>
        [Description("Duplicate table tags or tags out of alphabetical order.")] public const int
            MSSIPOTF_E_TABLE_TAGORDER = unchecked((int) 0x80097006);

        ///<summary>
        ///A table does not start on a long word boundary.
        ///</summary>
        [Description("A table does not start on a long word boundary.")] public const int MSSIPOTF_E_TABLE_LONGWORD =
            unchecked((int) 0x80097007);

        ///<summary>
        ///First table does not appear after header information.
        ///</summary>
        [Description("First table does not appear after header information.")] public const int
            MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT = unchecked((int) 0x80097008);

        ///<summary>
        ///Two or more tables overlap.
        ///</summary>
        [Description("Two or more tables overlap.")] public const int MSSIPOTF_E_TABLES_OVERLAP =
            unchecked((int) 0x80097009);

        ///<summary>
        ///Too many pad bytes between tables or pad bytes are not 0.
        ///</summary>
        [Description("Too many pad bytes between tables or pad bytes are not 0.")] public const int
            MSSIPOTF_E_TABLE_PADBYTES = unchecked((int) 0x8009700A);

        ///<summary>
        ///File is too small to contain the last table.
        ///</summary>
        [Description("File is too small to contain the last table.")] public const int MSSIPOTF_E_FILETOOSMALL =
            unchecked((int) 0x8009700B);

        ///<summary>
        ///A table checksum is incorrect.
        ///</summary>
        [Description("A table checksum is incorrect.")] public const int MSSIPOTF_E_TABLE_CHECKSUM =
            unchecked((int) 0x8009700C);

        ///<summary>
        ///The file checksum is incorrect.
        ///</summary>
        [Description("The file checksum is incorrect.")] public const int MSSIPOTF_E_FILE_CHECKSUM =
            unchecked((int) 0x8009700D);

        ///<summary>
        ///The signature does not have the correct attributes for the policy.
        ///</summary>
        [Description("The signature does not have the correct attributes for the policy.")] public const int
            MSSIPOTF_E_FAILED_POLICY = unchecked((int) 0x80097010);

        ///<summary>
        ///The file did not pass the hints check.
        ///</summary>
        [Description("The file did not pass the hints check.")] public const int MSSIPOTF_E_FAILED_HINTS_CHECK =
            unchecked((int) 0x80097011);

        ///<summary>
        ///The file is not an OpenType file.
        ///</summary>
        [Description("The file is not an OpenType file.")] public const int MSSIPOTF_E_NOT_OPENTYPE =
            unchecked((int) 0x80097012);

        ///<summary>
        ///Failed on a file operation (open, map, read, write).
        ///</summary>
        [Description("Failed on a file operation (open, map, read, write).")] public const int MSSIPOTF_E_FILE =
            unchecked((int) 0x80097013);

        ///<summary>
        ///A call to a CryptoAPI function failed.
        ///</summary>
        [Description("A call to a CryptoAPI function failed.")] public const int MSSIPOTF_E_CRYPT =
            unchecked((int) 0x80097014);

        ///<summary>
        ///There is a bad version number in the file.
        ///</summary>
        [Description("There is a bad version number in the file.")] public const int MSSIPOTF_E_BADVERSION =
            unchecked((int) 0x80097015);

        ///<summary>
        ///The structure of the DSIG table is incorrect.
        ///</summary>
        [Description("The structure of the DSIG table is incorrect.")] public const int MSSIPOTF_E_DSIG_STRUCTURE =
            unchecked((int) 0x80097016);

        ///<summary>
        ///A check failed in a partially constant table.
        ///</summary>
        [Description("A check failed in a partially constant table.")] public const int MSSIPOTF_E_PCONST_CHECK =
            unchecked((int) 0x80097017);

        ///<summary>
        ///Some kind of structural error.
        ///</summary>
        [Description("Some kind of structural error.")] public const int MSSIPOTF_E_STRUCTURE =
            unchecked((int) 0x80097018);

        //
        // Note that additional FACILITY_SSPI errors are in issperr.h
        //

        #endregion

        // ******************
        //  FACILITY_CONTROL
        // ******************

        #region (0x0A0000 - 0x0AFFFF) FACILITY_CONTROL errors

        #endregion

        // ******************
        // FACILITY_CERT
        // ******************

        #region (0x0B0000 - 0x0BFFFF) FACILITY_CERT errors

        ///<summary>
        ///Unknown trust provider.
        ///</summary>
        [Description("Unknown trust provider.")] public const int TRUST_E_PROVIDER_UNKNOWN = unchecked((int) 0x800B0001);

        ///<summary>
        ///The trust verification action specified is not supported by the specified trust provider.
        ///</summary>
        [Description("The trust verification action specified is not supported by the specified trust provider.")] public const int TRUST_E_ACTION_UNKNOWN = unchecked((int) 0x800B0002);

        ///<summary>
        ///The form specified for the subject is not one supported or known by the specified trust provider.
        ///</summary>
        [Description("The form specified for the subject is not one supported or known by the specified trust provider."
            )] public const int TRUST_E_SUBJECT_FORM_UNKNOWN = unchecked((int) 0x800B0003);

        ///<summary>
        ///The subject is not trusted for the specified action.
        ///</summary>
        [Description("The subject is not trusted for the specified action.")] public const int
            TRUST_E_SUBJECT_NOT_TRUSTED = unchecked((int) 0x800B0004);

        ///<summary>
        ///Error due to problem in ASN.1 encoding process.
        ///</summary>
        [Description("Error due to problem in ASN.1 encoding process.")] public const int DIGSIG_E_ENCODE =
            unchecked((int) 0x800B0005);

        ///<summary>
        ///Error due to problem in ASN.1 decoding process.
        ///</summary>
        [Description("Error due to problem in ASN.1 decoding process.")] public const int DIGSIG_E_DECODE =
            unchecked((int) 0x800B0006);

        ///<summary>
        ///Reading / writing Extensions where Attributes are appropriate, and visa versa.
        ///</summary>
        [Description("Reading / writing Extensions where Attributes are appropriate, and visa versa.")] public const int
            DIGSIG_E_EXTENSIBILITY = unchecked((int) 0x800B0007);

        ///<summary>
        ///Unspecified cryptographic failure.
        ///</summary>
        [Description("Unspecified cryptographic failure.")] public const int DIGSIG_E_CRYPTO =
            unchecked((int) 0x800B0008);

        ///<summary>
        ///The size of the data could not be determined.
        ///</summary>
        [Description("The size of the data could not be determined.")] public const int PERSIST_E_SIZEDEFINITE =
            unchecked((int) 0x800B0009);

        ///<summary>
        ///The size of the indefinite-sized data could not be determined.
        ///</summary>
        [Description("The size of the indefinite-sized data could not be determined.")] public const int
            PERSIST_E_SIZEINDEFINITE = unchecked((int) 0x800B000A);

        ///<summary>
        ///This object does not read and write self-sizing data.
        ///</summary>
        [Description("This object does not read and write self-sizing data.")] public const int PERSIST_E_NOTSELFSIZING
            = unchecked((int) 0x800B000B);

        ///<summary>
        ///No signature was present in the subject.
        ///</summary>
        [Description("No signature was present in the subject.")] public const int TRUST_E_NOSIGNATURE =
            unchecked((int) 0x800B0100);

        ///<summary>
        ///A required certificate is not within its validity period when verifying against the current system clock or the timestamp in the signed file.
        ///</summary>
        [Description(
            "A required certificate is not within its validity period when verifying against the current system clock or the timestamp in the signed file."
            )] public const int CERT_E_EXPIRED = unchecked((int) 0x800B0101);

        ///<summary>
        ///The validity periods of the certification chain do not nest correctly.
        ///</summary>
        [Description("The validity periods of the certification chain do not nest correctly.")] public const int
            CERT_E_VALIDITYPERIODNESTING = unchecked((int) 0x800B0102);

        ///<summary>
        ///A certificate that can only be used as an end-entity is being used as a CA or visa versa.
        ///</summary>
        [Description("A certificate that can only be used as an end-entity is being used as a CA or visa versa.")] public const int CERT_E_ROLE = unchecked((int) 0x800B0103);

        ///<summary>
        ///A path length constraint in the certification chain has been violated.
        ///</summary>
        [Description("A path length constraint in the certification chain has been violated.")] public const int
            CERT_E_PATHLENCONST = unchecked((int) 0x800B0104);

        ///<summary>
        ///A certificate contains an unknown extension that is marked 'critical'.
        ///</summary>
        [Description("A certificate contains an unknown extension that is marked 'critical'.")] public const int
            CERT_E_CRITICAL = unchecked((int) 0x800B0105);

        ///<summary>
        ///A certificate being used for a purpose other than the ones specified by its CA.
        ///</summary>
        [Description("A certificate being used for a purpose other than the ones specified by its CA.")] public const
            int CERT_E_PURPOSE = unchecked((int) 0x800B0106);

        ///<summary>
        ///A parent of a given certificate in fact did not issue that child certificate.
        ///</summary>
        [Description("A parent of a given certificate in fact did not issue that child certificate.")] public const int
            CERT_E_ISSUERCHAINING = unchecked((int) 0x800B0107);

        ///<summary>
        ///A certificate is missing or has an empty value for an important field, such as a subject or issuer name.
        ///</summary>
        [Description(
            "A certificate is missing or has an empty value for an important field, such as a subject or issuer name.")] public const int CERT_E_MALFORMED = unchecked((int) 0x800B0108);

        ///<summary>
        ///A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.
        ///</summary>
        [Description(
            "A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider."
            )] public const int CERT_E_UNTRUSTEDROOT = unchecked((int) 0x800B0109);

        ///<summary>
        ///A certificate chain could not be built to a trusted root authority.
        ///</summary>
        [Description("A certificate chain could not be built to a trusted root authority.")] public const int
            CERT_E_CHAINING = unchecked((int) 0x800B010A);

        ///<summary>
        ///Generic trust failure.
        ///</summary>
        [Description("Generic trust failure.")] public const int TRUST_E_FAIL = unchecked((int) 0x800B010B);

        ///<summary>
        ///A certificate was explicitly revoked by its issuer.
        ///</summary>
        [Description("A certificate was explicitly revoked by its issuer.")] public const int CERT_E_REVOKED =
            unchecked((int) 0x800B010C);

        ///<summary>
        ///The certification path terminates with the test root which is not trusted with the current policy settings.
        ///</summary>
        [Description(
            "The certification path terminates with the test root which is not trusted with the current policy settings."
            )] public const int CERT_E_UNTRUSTEDTESTROOT = unchecked((int) 0x800B010D);

        ///<summary>
        ///The revocation process could not continue - the certificate(s) could not be checked.
        ///</summary>
        [Description("The revocation process could not continue - the certificate(s) could not be checked.")] public
            const int CERT_E_REVOCATION_FAILURE = unchecked((int) 0x800B010E);

        ///<summary>
        ///The certificate's CN name does not match the passed value.
        ///</summary>
        [Description("The certificate's CN name does not match the passed value.")] public const int CERT_E_CN_NO_MATCH
            = unchecked((int) 0x800B010F);

        ///<summary>
        ///The certificate is not valid for the requested usage.
        ///</summary>
        [Description("The certificate is not valid for the requested usage.")] public const int CERT_E_WRONG_USAGE =
            unchecked((int) 0x800B0110);

        ///<summary>
        ///The certificate was explicitly marked as untrusted by the user.
        ///</summary>
        [Description("The certificate was explicitly marked as untrusted by the user.")] public const int
            TRUST_E_EXPLICIT_DISTRUST = unchecked((int) 0x800B0111);

        ///<summary>
        ///A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider.
        ///</summary>
        [Description(
            "A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider."
            )] public const int CERT_E_UNTRUSTEDCA = unchecked((int) 0x800B0112);

        ///<summary>
        ///The certificate has invalid policy.
        ///</summary>
        [Description("The certificate has invalid policy.")] public const int CERT_E_INVALID_POLICY =
            unchecked((int) 0x800B0113);

        ///<summary>
        ///The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded.
        ///</summary>
        [Description(
            "The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded."
            )] public const int CERT_E_INVALID_NAME = unchecked((int) 0x800B0114);

        #endregion

        // ******************
        //  FACILITY_INTERNET
        // ******************

        #region (0x0C0000 - 0x0CFFFF) FACILITY_INTERNET errors

        #endregion

        // ******************
        //  FACILITY_MEDIASERVER
        // ******************

        #region (0x0D0000 - 0x0DFFFF) FACILITY_MEDIASERVER errors

        #endregion

        // ******************
        //  FACILITY_MSMQ
        // ******************

        #region (0x0E0000 - 0x0EFFFF) FACILITY_MSMQ errors

        #endregion

        // *****************
        // FACILITY_SETUPAPI
        // *****************

        #region (0x0F0000 - 0x0FFFFF) FACILITY_SETUPAPI errors

        ///<summary>
        ///A non-empty line was encountered in the INF before the start of a section.
        ///</summary>
        [Description("A non-empty line was encountered in the INF before the start of a section.")] public const int
            SPAPI_E_EXPECTED_SECTION_NAME = unchecked((int) 0x800F0000);

        ///<summary>
        ///A section name marker in the INF is not complete, or does not exist on a line by itself.
        ///</summary>
        [Description("A section name marker in the INF is not complete, or does not exist on a line by itself.")] public
            const int SPAPI_E_BAD_SECTION_NAME_LINE = unchecked((int) 0x800F0001);

        ///<summary>
        ///An INF section was encountered whose name exceeds the maximum section name length.
        ///</summary>
        [Description("An INF section was encountered whose name exceeds the maximum section name length.")] public const
            int SPAPI_E_SECTION_NAME_TOO_LONG = unchecked((int) 0x800F0002);

        ///<summary>
        ///The syntax of the INF is invalid.
        ///</summary>
        [Description("The syntax of the INF is invalid.")] public const int SPAPI_E_GENERAL_SYNTAX =
            unchecked((int) 0x800F0003);

        ///<summary>
        ///The style of the INF is different than what was requested.
        ///</summary>
        [Description("The style of the INF is different than what was requested.")] public const int
            SPAPI_E_WRONG_INF_STYLE = unchecked((int) 0x800F0100);

        ///<summary>
        ///The required section was not found in the INF.
        ///</summary>
        [Description("The required section was not found in the INF.")] public const int SPAPI_E_SECTION_NOT_FOUND =
            unchecked((int) 0x800F0101);

        ///<summary>
        ///The required line was not found in the INF.
        ///</summary>
        [Description("The required line was not found in the INF.")] public const int SPAPI_E_LINE_NOT_FOUND =
            unchecked((int) 0x800F0102);

        ///<summary>
        ///The files affected by the installation of this file queue have not been backed up for uninstall.
        ///</summary>
        [Description("The files affected by the installation of this file queue have not been backed up for uninstall.")
        ] public const int SPAPI_E_NO_BACKUP = unchecked((int) 0x800F0103);

        ///<summary>
        ///The INF or the device information set or element does not have an associated install class.
        ///</summary>
        [Description("The INF or the device information set or element does not have an associated install class.")] public const int SPAPI_E_NO_ASSOCIATED_CLASS = unchecked((int) 0x800F0200);

        ///<summary>
        ///The INF or the device information set or element does not match the specified install class.
        ///</summary>
        [Description("The INF or the device information set or element does not match the specified install class.")] public const int SPAPI_E_CLASS_MISMATCH = unchecked((int) 0x800F0201);

        ///<summary>
        ///An existing device was found that is a duplicate of the device being manually installed.
        ///</summary>
        [Description("An existing device was found that is a duplicate of the device being manually installed.")] public
            const int SPAPI_E_DUPLICATE_FOUND = unchecked((int) 0x800F0202);

        ///<summary>
        ///There is no driver selected for the device information set or element.
        ///</summary>
        [Description("There is no driver selected for the device information set or element.")] public const int
            SPAPI_E_NO_DRIVER_SELECTED = unchecked((int) 0x800F0203);

        ///<summary>
        ///The requested device registry key does not exist.
        ///</summary>
        [Description("The requested device registry key does not exist.")] public const int SPAPI_E_KEY_DOES_NOT_EXIST =
            unchecked((int) 0x800F0204);

        ///<summary>
        ///The device instance name is invalid.
        ///</summary>
        [Description("The device instance name is invalid.")] public const int SPAPI_E_INVALID_DEVINST_NAME =
            unchecked((int) 0x800F0205);

        ///<summary>
        ///The install class is not present or is invalid.
        ///</summary>
        [Description("The install class is not present or is invalid.")] public const int SPAPI_E_INVALID_CLASS =
            unchecked((int) 0x800F0206);

        ///<summary>
        ///The device instance cannot be created because it already exists.
        ///</summary>
        [Description("The device instance cannot be created because it already exists.")] public const int
            SPAPI_E_DEVINST_ALREADY_EXISTS = unchecked((int) 0x800F0207);

        ///<summary>
        ///The operation cannot be performed on a device information element that has not been registered.
        ///</summary>
        [Description("The operation cannot be performed on a device information element that has not been registered.")] public const int SPAPI_E_DEVINFO_NOT_REGISTERED = unchecked((int) 0x800F0208);

        ///<summary>
        ///The device property code is invalid.
        ///</summary>
        [Description("The device property code is invalid.")] public const int SPAPI_E_INVALID_REG_PROPERTY =
            unchecked((int) 0x800F0209);

        ///<summary>
        ///The INF from which a driver list is to be built does not exist.
        ///</summary>
        [Description("The INF from which a driver list is to be built does not exist.")] public const int SPAPI_E_NO_INF
            = unchecked((int) 0x800F020A);

        ///<summary>
        ///The device instance does not exist in the hardware tree.
        ///</summary>
        [Description("The device instance does not exist in the hardware tree.")] public const int
            SPAPI_E_NO_SUCH_DEVINST = unchecked((int) 0x800F020B);

        ///<summary>
        ///The icon representing this install class cannot be loaded.
        ///</summary>
        [Description("The icon representing this install class cannot be loaded.")] public const int
            SPAPI_E_CANT_LOAD_CLASS_ICON = unchecked((int) 0x800F020C);

        ///<summary>
        ///The class installer registry entry is invalid.
        ///</summary>
        [Description("The class installer registry entry is invalid.")] public const int SPAPI_E_INVALID_CLASS_INSTALLER
            = unchecked((int) 0x800F020D);

        ///<summary>
        ///The class installer has indicated that the default action should be performed for this installation request.
        ///</summary>
        [Description(
            "The class installer has indicated that the default action should be performed for this installation request."
            )] public const int SPAPI_E_DI_DO_DEFAULT = unchecked((int) 0x800F020E);

        ///<summary>
        ///The operation does not require any files to be copied.
        ///</summary>
        [Description("The operation does not require any files to be copied.")] public const int SPAPI_E_DI_NOFILECOPY =
            unchecked((int) 0x800F020F);

        ///<summary>
        ///The specified hardware profile does not exist.
        ///</summary>
        [Description("The specified hardware profile does not exist.")] public const int SPAPI_E_INVALID_HWPROFILE =
            unchecked((int) 0x800F0210);

        ///<summary>
        ///There is no device information element currently selected for this device information set.
        ///</summary>
        [Description("There is no device information element currently selected for this device information set.")] public const int SPAPI_E_NO_DEVICE_SELECTED = unchecked((int) 0x800F0211);

        ///<summary>
        ///The operation cannot be performed because the device information set is locked.
        ///</summary>
        [Description("The operation cannot be performed because the device information set is locked.")] public const
            int SPAPI_E_DEVINFO_LIST_LOCKED = unchecked((int) 0x800F0212);

        ///<summary>
        ///The operation cannot be performed because the device information element is locked.
        ///</summary>
        [Description("The operation cannot be performed because the device information element is locked.")] public
            const int SPAPI_E_DEVINFO_DATA_LOCKED = unchecked((int) 0x800F0213);

        ///<summary>
        ///The specified path does not contain any applicable device INFs.
        ///</summary>
        [Description("The specified path does not contain any applicable device INFs.")] public const int
            SPAPI_E_DI_BAD_PATH = unchecked((int) 0x800F0214);

        ///<summary>
        ///No class installer parameters have been set for the device information set or element.
        ///</summary>
        [Description("No class installer parameters have been set for the device information set or element.")] public
            const int SPAPI_E_NO_CLASSINSTALL_PARAMS = unchecked((int) 0x800F0215);

        ///<summary>
        ///The operation cannot be performed because the file queue is locked.
        ///</summary>
        [Description("The operation cannot be performed because the file queue is locked.")] public const int
            SPAPI_E_FILEQUEUE_LOCKED = unchecked((int) 0x800F0216);

        ///<summary>
        ///A service installation section in this INF is invalid.
        ///</summary>
        [Description("A service installation section in this INF is invalid.")] public const int
            SPAPI_E_BAD_SERVICE_INSTALLSECT = unchecked((int) 0x800F0217);

        ///<summary>
        ///There is no class driver list for the device information element.
        ///</summary>
        [Description("There is no class driver list for the device information element.")] public const int
            SPAPI_E_NO_CLASS_DRIVER_LIST = unchecked((int) 0x800F0218);

        ///<summary>
        ///The installation failed because a function driver was not specified for this device instance.
        ///</summary>
        [Description("The installation failed because a function driver was not specified for this device instance.")] public const int SPAPI_E_NO_ASSOCIATED_SERVICE = unchecked((int) 0x800F0219);

        ///<summary>
        ///There is presently no default device interface designated for this interface class.
        ///</summary>
        [Description("There is presently no default device interface designated for this interface class.")] public
            const int SPAPI_E_NO_DEFAULT_DEVICE_INTERFACE = unchecked((int) 0x800F021A);

        ///<summary>
        ///The operation cannot be performed because the device interface is currently active.
        ///</summary>
        [Description("The operation cannot be performed because the device interface is currently active.")] public
            const int SPAPI_E_DEVICE_INTERFACE_ACTIVE = unchecked((int) 0x800F021B);

        ///<summary>
        ///The operation cannot be performed because the device interface has been removed from the system.
        ///</summary>
        [Description("The operation cannot be performed because the device interface has been removed from the system.")
        ] public const int SPAPI_E_DEVICE_INTERFACE_REMOVED = unchecked((int) 0x800F021C);

        ///<summary>
        ///An interface installation section in this INF is invalid.
        ///</summary>
        [Description("An interface installation section in this INF is invalid.")] public const int
            SPAPI_E_BAD_INTERFACE_INSTALLSECT = unchecked((int) 0x800F021D);

        ///<summary>
        ///This interface class does not exist in the system.
        ///</summary>
        [Description("This interface class does not exist in the system.")] public const int
            SPAPI_E_NO_SUCH_INTERFACE_CLASS = unchecked((int) 0x800F021E);

        ///<summary>
        ///The reference string supplied for this interface device is invalid.
        ///</summary>
        [Description("The reference string supplied for this interface device is invalid.")] public const int
            SPAPI_E_INVALID_REFERENCE_STRING = unchecked((int) 0x800F021F);

        ///<summary>
        ///The specified machine name does not conform to UNC naming conventions.
        ///</summary>
        [Description("The specified machine name does not conform to UNC naming conventions.")] public const int
            SPAPI_E_INVALID_MACHINENAME = unchecked((int) 0x800F0220);

        ///<summary>
        ///A general remote communication error occurred.
        ///</summary>
        [Description("A general remote communication error occurred.")] public const int SPAPI_E_REMOTE_COMM_FAILURE =
            unchecked((int) 0x800F0221);

        ///<summary>
        ///The machine selected for remote communication is not available at this time.
        ///</summary>
        [Description("The machine selected for remote communication is not available at this time.")] public const int
            SPAPI_E_MACHINE_UNAVAILABLE = unchecked((int) 0x800F0222);

        ///<summary>
        ///The Plug and Play service is not available on the remote machine.
        ///</summary>
        [Description("The Plug and Play service is not available on the remote machine.")] public const int
            SPAPI_E_NO_CONFIGMGR_SERVICES = unchecked((int) 0x800F0223);

        ///<summary>
        ///The property page provider registry entry is invalid.
        ///</summary>
        [Description("The property page provider registry entry is invalid.")] public const int
            SPAPI_E_INVALID_PROPPAGE_PROVIDER = unchecked((int) 0x800F0224);

        ///<summary>
        ///The requested device interface is not present in the system.
        ///</summary>
        [Description("The requested device interface is not present in the system.")] public const int
            SPAPI_E_NO_SUCH_DEVICE_INTERFACE = unchecked((int) 0x800F0225);

        ///<summary>
        ///The device's co-installer has additional work to perform after installation is complete.
        ///</summary>
        [Description("The device's co-installer has additional work to perform after installation is complete.")] public
            const int SPAPI_E_DI_POSTPROCESSING_REQUIRED = unchecked((int) 0x800F0226);

        ///<summary>
        ///The device's co-installer is invalid.
        ///</summary>
        [Description("The device's co-installer is invalid.")] public const int SPAPI_E_INVALID_COINSTALLER =
            unchecked((int) 0x800F0227);

        ///<summary>
        ///There are no compatible drivers for this device.
        ///</summary>
        [Description("There are no compatible drivers for this device.")] public const int SPAPI_E_NO_COMPAT_DRIVERS =
            unchecked((int) 0x800F0228);

        ///<summary>
        ///There is no icon that represents this device or device type.
        ///</summary>
        [Description("There is no icon that represents this device or device type.")] public const int
            SPAPI_E_NO_DEVICE_ICON = unchecked((int) 0x800F0229);

        ///<summary>
        ///A logical configuration specified in this INF is invalid.
        ///</summary>
        [Description("A logical configuration specified in this INF is invalid.")] public const int
            SPAPI_E_INVALID_INF_LOGCONFIG = unchecked((int) 0x800F022A);

        ///<summary>
        ///The class installer has denied the request to install or upgrade this device.
        ///</summary>
        [Description("The class installer has denied the request to install or upgrade this device.")] public const int
            SPAPI_E_DI_DONT_INSTALL = unchecked((int) 0x800F022B);

        ///<summary>
        ///One of the filter drivers installed for this device is invalid.
        ///</summary>
        [Description("One of the filter drivers installed for this device is invalid.")] public const int
            SPAPI_E_INVALID_FILTER_DRIVER = unchecked((int) 0x800F022C);

        ///<summary>
        ///The driver selected for this device does not support Windows XP.
        ///</summary>
        [Description("The driver selected for this device does not support Windows XP.")] public const int
            SPAPI_E_NON_WINDOWS_NT_DRIVER = unchecked((int) 0x800F022D);

        ///<summary>
        ///The driver selected for this device does not support Windows.
        ///</summary>
        [Description("The driver selected for this device does not support Windows.")] public const int
            SPAPI_E_NON_WINDOWS_DRIVER = unchecked((int) 0x800F022E);

        ///<summary>
        ///The third-party INF does not contain digital signature information.
        ///</summary>
        [Description("The third-party INF does not contain digital signature information.")] public const int
            SPAPI_E_NO_CATALOG_FOR_OEM_INF = unchecked((int) 0x800F022F);

        ///<summary>
        ///An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms.
        ///</summary>
        [Description(
            "An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms."
            )] public const int SPAPI_E_DEVINSTALL_QUEUE_NONNATIVE = unchecked((int) 0x800F0230);

        ///<summary>
        ///The device cannot be disabled.
        ///</summary>
        [Description("The device cannot be disabled.")] public const int SPAPI_E_NOT_DISABLEABLE =
            unchecked((int) 0x800F0231);

        ///<summary>
        ///The device could not be dynamically removed.
        ///</summary>
        [Description("The device could not be dynamically removed.")] public const int SPAPI_E_CANT_REMOVE_DEVINST =
            unchecked((int) 0x800F0232);

        ///<summary>
        ///Cannot copy to specified target.
        ///</summary>
        [Description("Cannot copy to specified target.")] public const int SPAPI_E_INVALID_TARGET =
            unchecked((int) 0x800F0233);

        ///<summary>
        ///Driver is not intended for this platform.
        ///</summary>
        [Description("Driver is not intended for this platform.")] public const int SPAPI_E_DRIVER_NONNATIVE =
            unchecked((int) 0x800F0234);

        ///<summary>
        ///Operation not allowed in WOW64.
        ///</summary>
        [Description("Operation not allowed in WOW64.")] public const int SPAPI_E_IN_WOW64 = unchecked((int) 0x800F0235);

        ///<summary>
        ///The operation involving unsigned file copying was rolled back, so that a system restore point could be set.
        ///</summary>
        [Description(
            "The operation involving unsigned file copying was rolled back, so that a system restore point could be set."
            )] public const int SPAPI_E_SET_SYSTEM_RESTORE_POINT = unchecked((int) 0x800F0236);

        ///<summary>
        ///An INF was copied into the Windows INF directory in an improper manner.
        ///</summary>
        [Description("An INF was copied into the Windows INF directory in an improper manner.")] public const int
            SPAPI_E_INCORRECTLY_COPIED_INF = unchecked((int) 0x800F0237);

        ///<summary>
        ///The Security Configuration Editor (SCE) APIs have been disabled on this Embedded product.
        ///</summary>
        [Description("The Security Configuration Editor (SCE) APIs have been disabled on this Embedded product.")] public const int SPAPI_E_SCE_DISABLED = unchecked((int) 0x800F0238);

        ///<summary>
        ///No installed components were detected.
        ///</summary>
        [Description("No installed components were detected.")] public const int SPAPI_E_ERROR_NOT_INSTALLED =
            unchecked((int) 0x800F1000);

        #endregion

        // *****************
        // FACILITY_SCARD
        // *****************

        #region (0x100000 - 0x10FFFF) FACILITY_SCARD errors

        // =============================
        // Facility SCARD Error Messages
        // =============================
        //
        public const int SCARD_S_SUCCESS = NO_ERROR;

        ///<summary>
        ///An internal consistency check failed.
        ///</summary>
        [Description("An internal consistency check failed.")] public const int SCARD_F_INTERNAL_ERROR =
            unchecked((int) 0x80100001);

        ///<summary>
        ///The action was cancelled by an SCardCancel request.
        ///</summary>
        [Description("The action was cancelled by an SCardCancel request.")] public const int SCARD_E_CANCELLED =
            unchecked((int) 0x80100002);

        ///<summary>
        ///The supplied handle was invalid.
        ///</summary>
        [Description("The supplied handle was invalid.")] public const int SCARD_E_INVALID_HANDLE =
            unchecked((int) 0x80100003);

        ///<summary>
        ///One or more of the supplied parameters could not be properly interpreted.
        ///</summary>
        [Description("One or more of the supplied parameters could not be properly interpreted.")] public const int
            SCARD_E_INVALID_PARAMETER = unchecked((int) 0x80100004);

        ///<summary>
        ///Registry startup information is missing or invalid.
        ///</summary>
        [Description("Registry startup information is missing or invalid.")] public const int SCARD_E_INVALID_TARGET =
            unchecked((int) 0x80100005);

        ///<summary>
        ///Not enough memory available to complete this command.
        ///</summary>
        [Description("Not enough memory available to complete this command.")] public const int SCARD_E_NO_MEMORY =
            unchecked((int) 0x80100006);

        ///<summary>
        ///An internal consistency timer has expired.
        ///</summary>
        [Description("An internal consistency timer has expired.")] public const int SCARD_F_WAITED_TOO_LONG =
            unchecked((int) 0x80100007);

        ///<summary>
        ///The data buffer to receive returned data is too small for the returned data.
        ///</summary>
        [Description("The data buffer to receive returned data is too small for the returned data.")] public const int
            SCARD_E_INSUFFICIENT_BUFFER = unchecked((int) 0x80100008);

        ///<summary>
        ///The specified reader name is not recognized.
        ///</summary>
        [Description("The specified reader name is not recognized.")] public const int SCARD_E_UNKNOWN_READER =
            unchecked((int) 0x80100009);

        ///<summary>
        ///The user-specified timeout value has expired.
        ///</summary>
        [Description("The user-specified timeout value has expired.")] public const int SCARD_E_TIMEOUT =
            unchecked((int) 0x8010000A);

        ///<summary>
        ///The smart card cannot be accessed because of other connections outstanding.
        ///</summary>
        [Description("The smart card cannot be accessed because of other connections outstanding.")] public const int
            SCARD_E_SHARING_VIOLATION = unchecked((int) 0x8010000B);

        ///<summary>
        ///The operation requires a Smart Card, but no Smart Card is currently in the device.
        ///</summary>
        [Description("The operation requires a Smart Card, but no Smart Card is currently in the device.")] public const
            int SCARD_E_NO_SMARTCARD = unchecked((int) 0x8010000C);

        ///<summary>
        ///The specified smart card name is not recognized.
        ///</summary>
        [Description("The specified smart card name is not recognized.")] public const int SCARD_E_UNKNOWN_CARD =
            unchecked((int) 0x8010000D);

        ///<summary>
        ///The system could not dispose of the media in the requested manner.
        ///</summary>
        [Description("The system could not dispose of the media in the requested manner.")] public const int
            SCARD_E_CANT_DISPOSE = unchecked((int) 0x8010000E);

        ///<summary>
        ///The requested protocols are incompatible with the protocol currently in use with the smart card.
        ///</summary>
        [Description("The requested protocols are incompatible with the protocol currently in use with the smart card.")
        ] public const int SCARD_E_PROTO_MISMATCH = unchecked((int) 0x8010000F);

        ///<summary>
        ///The reader or smart card is not ready to accept commands.
        ///</summary>
        [Description("The reader or smart card is not ready to accept commands.")] public const int SCARD_E_NOT_READY =
            unchecked((int) 0x80100010);

        ///<summary>
        ///One or more of the supplied parameters values could not be properly interpreted.
        ///</summary>
        [Description("One or more of the supplied parameters values could not be properly interpreted.")] public const
            int SCARD_E_INVALID_VALUE = unchecked((int) 0x80100011);

        ///<summary>
        ///The action was cancelled by the system, presumably to log off or shut down.
        ///</summary>
        [Description("The action was cancelled by the system, presumably to log off or shut down.")] public const int
            SCARD_E_SYSTEM_CANCELLED = unchecked((int) 0x80100012);

        ///<summary>
        ///An internal communications error has been detected.
        ///</summary>
        [Description("An internal communications error has been detected.")] public const int SCARD_F_COMM_ERROR =
            unchecked((int) 0x80100013);

        ///<summary>
        ///An internal error has been detected, but the source is unknown.
        ///</summary>
        [Description("An internal error has been detected, but the source is unknown.")] public const int
            SCARD_F_UNKNOWN_ERROR = unchecked((int) 0x80100014);

        ///<summary>
        ///An ATR obtained from the registry is not a valid ATR string.
        ///</summary>
        [Description("An ATR obtained from the registry is not a valid ATR string.")] public const int
            SCARD_E_INVALID_ATR = unchecked((int) 0x80100015);

        ///<summary>
        ///An attempt was made to end a non-existent transaction.
        ///</summary>
        [Description("An attempt was made to end a non-existent transaction.")] public const int SCARD_E_NOT_TRANSACTED
            = unchecked((int) 0x80100016);

        ///<summary>
        ///The specified reader is not currently available for use.
        ///</summary>
        [Description("The specified reader is not currently available for use.")] public const int
            SCARD_E_READER_UNAVAILABLE = unchecked((int) 0x80100017);

        ///<summary>
        ///The operation has been aborted to allow the server application to exit.
        ///</summary>
        [Description("The operation has been aborted to allow the server application to exit.")] public const int
            SCARD_P_SHUTDOWN = unchecked((int) 0x80100018);

        ///<summary>
        ///The PCI Receive buffer was too small.
        ///</summary>
        [Description("The PCI Receive buffer was too small.")] public const int SCARD_E_PCI_TOO_SMALL =
            unchecked((int) 0x80100019);

        ///<summary>
        ///The reader driver does not meet minimal requirements for support.
        ///</summary>
        [Description("The reader driver does not meet minimal requirements for support.")] public const int
            SCARD_E_READER_UNSUPPORTED = unchecked((int) 0x8010001A);

        ///<summary>
        ///The reader driver did not produce a unique reader name.
        ///</summary>
        [Description("The reader driver did not produce a unique reader name.")] public const int
            SCARD_E_DUPLICATE_READER = unchecked((int) 0x8010001B);

        ///<summary>
        ///The smart card does not meet minimal requirements for support.
        ///</summary>
        [Description("The smart card does not meet minimal requirements for support.")] public const int
            SCARD_E_CARD_UNSUPPORTED = unchecked((int) 0x8010001C);

        ///<summary>
        ///The Smart card resource manager is not running.
        ///</summary>
        [Description("The Smart card resource manager is not running.")] public const int SCARD_E_NO_SERVICE =
            unchecked((int) 0x8010001D);

        ///<summary>
        ///The Smart card resource manager has shut down.
        ///</summary>
        [Description("The Smart card resource manager has shut down.")] public const int SCARD_E_SERVICE_STOPPED =
            unchecked((int) 0x8010001E);

        ///<summary>
        ///An unexpected card error has occurred.
        ///</summary>
        [Description("An unexpected card error has occurred.")] public const int SCARD_E_UNEXPECTED =
            unchecked((int) 0x8010001F);

        ///<summary>
        ///No Primary Provider can be found for the smart card.
        ///</summary>
        [Description("No Primary Provider can be found for the smart card.")] public const int SCARD_E_ICC_INSTALLATION
            = unchecked((int) 0x80100020);

        ///<summary>
        ///The requested order of object creation is not supported.
        ///</summary>
        [Description("The requested order of object creation is not supported.")] public const int
            SCARD_E_ICC_CREATEORDER = unchecked((int) 0x80100021);

        ///<summary>
        ///This smart card does not support the requested feature.
        ///</summary>
        [Description("This smart card does not support the requested feature.")] public const int
            SCARD_E_UNSUPPORTED_FEATURE = unchecked((int) 0x80100022);

        ///<summary>
        ///The identified directory does not exist in the smart card.
        ///</summary>
        [Description("The identified directory does not exist in the smart card.")] public const int
            SCARD_E_DIR_NOT_FOUND = unchecked((int) 0x80100023);

        ///<summary>
        ///The identified file does not exist in the smart card.
        ///</summary>
        [Description("The identified file does not exist in the smart card.")] public const int SCARD_E_FILE_NOT_FOUND =
            unchecked((int) 0x80100024);

        ///<summary>
        ///The supplied path does not represent a smart card directory.
        ///</summary>
        [Description("The supplied path does not represent a smart card directory.")] public const int SCARD_E_NO_DIR =
            unchecked((int) 0x80100025);

        ///<summary>
        ///The supplied path does not represent a smart card file.
        ///</summary>
        [Description("The supplied path does not represent a smart card file.")] public const int SCARD_E_NO_FILE =
            unchecked((int) 0x80100026);

        ///<summary>
        ///Access is denied to this file.
        ///</summary>
        [Description("Access is denied to this file.")] public const int SCARD_E_NO_ACCESS = unchecked((int) 0x80100027);

        ///<summary>
        ///The smartcard does not have enough memory to store the information.
        ///</summary>
        [Description("The smartcard does not have enough memory to store the information.")] public const int
            SCARD_E_WRITE_TOO_MANY = unchecked((int) 0x80100028);

        ///<summary>
        ///There was an error trying to set the smart card file object pointer.
        ///</summary>
        [Description("There was an error trying to set the smart card file object pointer.")] public const int
            SCARD_E_BAD_SEEK = unchecked((int) 0x80100029);

        ///<summary>
        ///The supplied PIN is incorrect.
        ///</summary>
        [Description("The supplied PIN is incorrect.")] public const int SCARD_E_INVALID_CHV =
            unchecked((int) 0x8010002A);

        ///<summary>
        ///An unrecognized error code was returned from a layered component.
        ///</summary>
        [Description("An unrecognized error code was returned from a layered component.")] public const int
            SCARD_E_UNKNOWN_RES_MNG = unchecked((int) 0x8010002B);

        ///<summary>
        ///The requested certificate does not exist.
        ///</summary>
        [Description("The requested certificate does not exist.")] public const int SCARD_E_NO_SUCH_CERTIFICATE =
            unchecked((int) 0x8010002C);

        ///<summary>
        ///The requested certificate could not be obtained.
        ///</summary>
        [Description("The requested certificate could not be obtained.")] public const int
            SCARD_E_CERTIFICATE_UNAVAILABLE = unchecked((int) 0x8010002D);

        ///<summary>
        ///Cannot find a smart card reader.
        ///</summary>
        [Description("Cannot find a smart card reader.")] public const int SCARD_E_NO_READERS_AVAILABLE =
            unchecked((int) 0x8010002E);

        ///<summary>
        ///A communications error with the smart card has been detected.  Retry the operation.
        ///</summary>
        [Description("A communications error with the smart card has been detected.  Retry the operation.")] public
            const int SCARD_E_COMM_DATA_LOST = unchecked((int) 0x8010002F);

        ///<summary>
        ///The requested key container does not exist on the smart card.
        ///</summary>
        [Description("The requested key container does not exist on the smart card.")] public const int
            SCARD_E_NO_KEY_CONTAINER = unchecked((int) 0x80100030);

        ///<summary>
        ///The Smart card resource manager is too busy to complete this operation.
        ///</summary>
        [Description("The Smart card resource manager is too busy to complete this operation.")] public const int
            SCARD_E_SERVER_TOO_BUSY = unchecked((int) 0x80100031);

        //
        // These are warning codes.
        //
        ///<summary>
        ///The reader cannot communicate with the smart card, due to ATR configuration conflicts.
        ///</summary>
        [Description("The reader cannot communicate with the smart card, due to ATR configuration conflicts.")] public
            const int SCARD_W_UNSUPPORTED_CARD = unchecked((int) 0x80100065);

        ///<summary>
        ///The smart card is not responding to a reset.
        ///</summary>
        [Description("The smart card is not responding to a reset.")] public const int SCARD_W_UNRESPONSIVE_CARD =
            unchecked((int) 0x80100066);

        ///<summary>
        ///Power has been removed from the smart card, so that further communication is not possible.
        ///</summary>
        [Description("Power has been removed from the smart card, so that further communication is not possible.")] public const int SCARD_W_UNPOWERED_CARD = unchecked((int) 0x80100067);

        ///<summary>
        ///The smart card has been reset, so any shared state information is invalid.
        ///</summary>
        [Description("The smart card has been reset, so any shared state information is invalid.")] public const int
            SCARD_W_RESET_CARD = unchecked((int) 0x80100068);

        ///<summary>
        ///The smart card has been removed, so that further communication is not possible.
        ///</summary>
        [Description("The smart card has been removed, so that further communication is not possible.")] public const
            int SCARD_W_REMOVED_CARD = unchecked((int) 0x80100069);

        ///<summary>
        ///Access was denied because of a security violation.
        ///</summary>
        [Description("Access was denied because of a security violation.")] public const int SCARD_W_SECURITY_VIOLATION
            = unchecked((int) 0x8010006A);

        ///<summary>
        ///The card cannot be accessed because the wrong PIN was presented.
        ///</summary>
        [Description("The card cannot be accessed because the wrong PIN was presented.")] public const int
            SCARD_W_WRONG_CHV = unchecked((int) 0x8010006B);

        ///<summary>
        ///The card cannot be accessed because the maximum number of PIN entry attempts has been reached.
        ///</summary>
        [Description("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")] public const int SCARD_W_CHV_BLOCKED = unchecked((int) 0x8010006C);

        ///<summary>
        ///The end of the smart card file has been reached.
        ///</summary>
        [Description("The end of the smart card file has been reached.")] public const int SCARD_W_EOF =
            unchecked((int) 0x8010006D);

        ///<summary>
        ///The action was cancelled by the user.
        ///</summary>
        [Description("The action was cancelled by the user.")] public const int SCARD_W_CANCELLED_BY_USER =
            unchecked((int) 0x8010006E);

        ///<summary>
        ///No PIN was presented to the smart card.
        ///</summary>
        [Description("No PIN was presented to the smart card.")] public const int SCARD_W_CARD_NOT_AUTHENTICATED =
            unchecked((int) 0x8010006F);

        #endregion

        // *****************
        // FACILITY_COMPLUS
        // *****************

        #region (0x110000 - 0x11FFFF) FACILITY_COMPLUS errors

        // ===============================
        // Facility COMPLUS Error Messages
        // ===============================
        //
        //
        // The following are the subranges  within the COMPLUS facility
        // 0x400 - 0x4ff           COMADMIN_E_CAT
        // 0x600 - 0x6ff           COMQC errors
        // 0x700 - 0x7ff           MSDTC errors
        // 0x800 - 0x8ff           Other COMADMIN errors
        //
        // COMPLUS Admin errors
        //
        ///<summary>
        ///Errors occurred accessing one or more objects - the ErrorInfo collection may have more detail
        ///</summary>
        [Description("Errors occurred accessing one or more objects - the ErrorInfo collection may have more detail")] public const int COMADMIN_E_OBJECTERRORS = unchecked((int) 0x80110401);

        ///<summary>
        ///One or more of the object's properties are missing or invalid
        ///</summary>
        [Description("One or more of the object's properties are missing or invalid")] public const int
            COMADMIN_E_OBJECTINVALID = unchecked((int) 0x80110402);

        ///<summary>
        ///The object was not found in the catalog
        ///</summary>
        [Description("The object was not found in the catalog")] public const int COMADMIN_E_KEYMISSING =
            unchecked((int) 0x80110403);

        ///<summary>
        ///The object is already registered
        ///</summary>
        [Description("The object is already registered")] public const int COMADMIN_E_ALREADYINSTALLED =
            unchecked((int) 0x80110404);

        ///<summary>
        ///Error occurred writing to the application file
        ///</summary>
        [Description("Error occurred writing to the application file")] public const int COMADMIN_E_APP_FILE_WRITEFAIL =
            unchecked((int) 0x80110407);

        ///<summary>
        ///Error occurred reading the application file
        ///</summary>
        [Description("Error occurred reading the application file")] public const int COMADMIN_E_APP_FILE_READFAIL =
            unchecked((int) 0x80110408);

        ///<summary>
        ///Invalid version number in application file
        ///</summary>
        [Description("Invalid version number in application file")] public const int COMADMIN_E_APP_FILE_VERSION =
            unchecked((int) 0x80110409);

        ///<summary>
        ///The file path is invalid
        ///</summary>
        [Description("The file path is invalid")] public const int COMADMIN_E_BADPATH = unchecked((int) 0x8011040A);

        ///<summary>
        ///The application is already installed
        ///</summary>
        [Description("The application is already installed")] public const int COMADMIN_E_APPLICATIONEXISTS =
            unchecked((int) 0x8011040B);

        ///<summary>
        ///The role already exists
        ///</summary>
        [Description("The role already exists")] public const int COMADMIN_E_ROLEEXISTS = unchecked((int) 0x8011040C);

        ///<summary>
        ///An error occurred copying the file
        ///</summary>
        [Description("An error occurred copying the file")] public const int COMADMIN_E_CANTCOPYFILE =
            unchecked((int) 0x8011040D);

        ///<summary>
        ///One or more users are not valid
        ///</summary>
        [Description("One or more users are not valid")] public const int COMADMIN_E_NOUSER =
            unchecked((int) 0x8011040F);

        ///<summary>
        ///One or more users in the application file are not valid
        ///</summary>
        [Description("One or more users in the application file are not valid")] public const int
            COMADMIN_E_INVALIDUSERIDS = unchecked((int) 0x80110410);

        ///<summary>
        ///The component's CLSID is missing or corrupt
        ///</summary>
        [Description("The component's CLSID is missing or corrupt")] public const int COMADMIN_E_NOREGISTRYCLSID =
            unchecked((int) 0x80110411);

        ///<summary>
        ///The component's progID is missing or corrupt
        ///</summary>
        [Description("The component's progID is missing or corrupt")] public const int COMADMIN_E_BADREGISTRYPROGID =
            unchecked((int) 0x80110412);

        ///<summary>
        ///Unable to set required authentication level for update request
        ///</summary>
        [Description("Unable to set required authentication level for update request")] public const int
            COMADMIN_E_AUTHENTICATIONLEVEL = unchecked((int) 0x80110413);

        ///<summary>
        ///The identity or password set on the application is not valid
        ///</summary>
        [Description("The identity or password set on the application is not valid")] public const int
            COMADMIN_E_USERPASSWDNOTVALID = unchecked((int) 0x80110414);

        ///<summary>
        ///Application file CLSIDs or IIDs do not match corresponding DLLs
        ///</summary>
        [Description("Application file CLSIDs or IIDs do not match corresponding DLLs")] public const int
            COMADMIN_E_CLSIDORIIDMISMATCH = unchecked((int) 0x80110418);

        ///<summary>
        ///Interface information is either missing or changed
        ///</summary>
        [Description("Interface information is either missing or changed")] public const int COMADMIN_E_REMOTEINTERFACE
            = unchecked((int) 0x80110419);

        ///<summary>
        ///DllRegisterServer failed on component install
        ///</summary>
        [Description("DllRegisterServer failed on component install")] public const int COMADMIN_E_DLLREGISTERSERVER =
            unchecked((int) 0x8011041A);

        ///<summary>
        ///No server file share available
        ///</summary>
        [Description("No server file share available")] public const int COMADMIN_E_NOSERVERSHARE =
            unchecked((int) 0x8011041B);

        ///<summary>
        ///DLL could not be loaded
        ///</summary>
        [Description("DLL could not be loaded")] public const int COMADMIN_E_DLLLOADFAILED = unchecked((int) 0x8011041D);

        ///<summary>
        ///The registered TypeLib ID is not valid
        ///</summary>
        [Description("The registered TypeLib ID is not valid")] public const int COMADMIN_E_BADREGISTRYLIBID =
            unchecked((int) 0x8011041E);

        ///<summary>
        ///Application install directory not found
        ///</summary>
        [Description("Application install directory not found")] public const int COMADMIN_E_APPDIRNOTFOUND =
            unchecked((int) 0x8011041F);

        ///<summary>
        ///Errors occurred while in the component registrar
        ///</summary>
        [Description("Errors occurred while in the component registrar")] public const int COMADMIN_E_REGISTRARFAILED =
            unchecked((int) 0x80110423);

        ///<summary>
        ///The file does not exist
        ///</summary>
        [Description("The file does not exist")] public const int COMADMIN_E_COMPFILE_DOESNOTEXIST =
            unchecked((int) 0x80110424);

        ///<summary>
        ///The DLL could not be loaded
        ///</summary>
        [Description("The DLL could not be loaded")] public const int COMADMIN_E_COMPFILE_LOADDLLFAIL =
            unchecked((int) 0x80110425);

        ///<summary>
        ///GetClassObject failed in the DLL
        ///</summary>
        [Description("GetClassObject failed in the DLL")] public const int COMADMIN_E_COMPFILE_GETCLASSOBJ =
            unchecked((int) 0x80110426);

        ///<summary>
        ///The DLL does not support the components listed in the TypeLib
        ///</summary>
        [Description("The DLL does not support the components listed in the TypeLib")] public const int
            COMADMIN_E_COMPFILE_CLASSNOTAVAIL = unchecked((int) 0x80110427);

        ///<summary>
        ///The TypeLib could not be loaded
        ///</summary>
        [Description("The TypeLib could not be loaded")] public const int COMADMIN_E_COMPFILE_BADTLB =
            unchecked((int) 0x80110428);

        ///<summary>
        ///The file does not contain components or component information
        ///</summary>
        [Description("The file does not contain components or component information")] public const int
            COMADMIN_E_COMPFILE_NOTINSTALLABLE = unchecked((int) 0x80110429);

        ///<summary>
        ///Changes to this object and its sub-objects have been disabled
        ///</summary>
        [Description("Changes to this object and its sub-objects have been disabled")] public const int
            COMADMIN_E_NOTCHANGEABLE = unchecked((int) 0x8011042A);

        ///<summary>
        ///The delete function has been disabled for this object
        ///</summary>
        [Description("The delete function has been disabled for this object")] public const int COMADMIN_E_NOTDELETEABLE
            = unchecked((int) 0x8011042B);

        ///<summary>
        ///The server catalog version is not supported
        ///</summary>
        [Description("The server catalog version is not supported")] public const int COMADMIN_E_SESSION =
            unchecked((int) 0x8011042C);

        ///<summary>
        ///The component move was disallowed, because the source or destination application is either a system application or currently locked against changes
        ///</summary>
        [Description(
            "The component move was disallowed, because the source or destination application is either a system application or currently locked against changes"
            )] public const int COMADMIN_E_COMP_MOVE_LOCKED = unchecked((int) 0x8011042D);

        ///<summary>
        ///The component move failed because the destination application no longer exists
        ///</summary>
        [Description("The component move failed because the destination application no longer exists")] public const int
            COMADMIN_E_COMP_MOVE_BAD_DEST = unchecked((int) 0x8011042E);

        ///<summary>
        ///The system was unable to register the TypeLib
        ///</summary>
        [Description("The system was unable to register the TypeLib")] public const int COMADMIN_E_REGISTERTLB =
            unchecked((int) 0x80110430);

        ///<summary>
        ///This operation can not be performed on the system application
        ///</summary>
        [Description("This operation can not be performed on the system application")] public const int
            COMADMIN_E_SYSTEMAPP = unchecked((int) 0x80110433);

        ///<summary>
        ///The component registrar referenced in this file is not available
        ///</summary>
        [Description("The component registrar referenced in this file is not available")] public const int
            COMADMIN_E_COMPFILE_NOREGISTRAR = unchecked((int) 0x80110434);

        ///<summary>
        ///A component in the same DLL is already installed
        ///</summary>
        [Description("A component in the same DLL is already installed")] public const int COMADMIN_E_COREQCOMPINSTALLED
            = unchecked((int) 0x80110435);

        ///<summary>
        ///The service is not installed
        ///</summary>
        [Description("The service is not installed")] public const int COMADMIN_E_SERVICENOTINSTALLED =
            unchecked((int) 0x80110436);

        ///<summary>
        ///One or more property settings are either invalid or in conflict with each other
        ///</summary>
        [Description("One or more property settings are either invalid or in conflict with each other")] public const
            int COMADMIN_E_PROPERTYSAVEFAILED = unchecked((int) 0x80110437);

        ///<summary>
        ///The object you are attempting to add or rename already exists
        ///</summary>
        [Description("The object you are attempting to add or rename already exists")] public const int
            COMADMIN_E_OBJECTEXISTS = unchecked((int) 0x80110438);

        ///<summary>
        ///The component already exists
        ///</summary>
        [Description("The component already exists")] public const int COMADMIN_E_COMPONENTEXISTS =
            unchecked((int) 0x80110439);

        ///<summary>
        ///The registration file is corrupt
        ///</summary>
        [Description("The registration file is corrupt")] public const int COMADMIN_E_REGFILE_CORRUPT =
            unchecked((int) 0x8011043B);

        ///<summary>
        ///The property value is too large
        ///</summary>
        [Description("The property value is too large")] public const int COMADMIN_E_PROPERTY_OVERFLOW =
            unchecked((int) 0x8011043C);

        ///<summary>
        ///Object was not found in registry
        ///</summary>
        [Description("Object was not found in registry")] public const int COMADMIN_E_NOTINREGISTRY =
            unchecked((int) 0x8011043E);

        ///<summary>
        ///This object is not poolable
        ///</summary>
        [Description("This object is not poolable")] public const int COMADMIN_E_OBJECTNOTPOOLABLE =
            unchecked((int) 0x8011043F);

        ///<summary>
        ///A CLSID with the same GUID as the new application ID is already installed on this machine
        ///</summary>
        [Description("A CLSID with the same GUID as the new application ID is already installed on this machine")] public const int COMADMIN_E_APPLID_MATCHES_CLSID = unchecked((int) 0x80110446);

        ///<summary>
        ///A role assigned to a component, interface, or method did not exist in the application
        ///</summary>
        [Description("A role assigned to a component, interface, or method did not exist in the application")] public
            const int COMADMIN_E_ROLE_DOES_NOT_EXIST = unchecked((int) 0x80110447);

        ///<summary>
        ///You must have components in an application in order to start the application
        ///</summary>
        [Description("You must have components in an application in order to start the application")] public const int
            COMADMIN_E_START_APP_NEEDS_COMPONENTS = unchecked((int) 0x80110448);

        ///<summary>
        ///This operation is not enabled on this platform
        ///</summary>
        [Description("This operation is not enabled on this platform")] public const int
            COMADMIN_E_REQUIRES_DIFFERENT_PLATFORM = unchecked((int) 0x80110449);

        ///<summary>
        ///Application Proxy is not exportable
        ///</summary>
        [Description("Application Proxy is not exportable")] public const int COMADMIN_E_CAN_NOT_EXPORT_APP_PROXY =
            unchecked((int) 0x8011044A);

        ///<summary>
        ///Failed to start application because it is either a library application or an application proxy
        ///</summary>
        [Description("Failed to start application because it is either a library application or an application proxy")] public const int COMADMIN_E_CAN_NOT_START_APP = unchecked((int) 0x8011044B);

        ///<summary>
        ///System application is not exportable
        ///</summary>
        [Description("System application is not exportable")] public const int COMADMIN_E_CAN_NOT_EXPORT_SYS_APP =
            unchecked((int) 0x8011044C);

        ///<summary>
        ///Can not subscribe to this component (the component may have been imported)
        ///</summary>
        [Description("Can not subscribe to this component (the component may have been imported)")] public const int
            COMADMIN_E_CANT_SUBSCRIBE_TO_COMPONENT = unchecked((int) 0x8011044D);

        ///<summary>
        ///An event class cannot also be a subscriber component
        ///</summary>
        [Description("An event class cannot also be a subscriber component")] public const int
            COMADMIN_E_EVENTCLASS_CANT_BE_SUBSCRIBER = unchecked((int) 0x8011044E);

        ///<summary>
        ///Library applications and application proxies are incompatible
        ///</summary>
        [Description("Library applications and application proxies are incompatible")] public const int
            COMADMIN_E_LIB_APP_PROXY_INCOMPATIBLE = unchecked((int) 0x8011044F);

        ///<summary>
        ///This function is valid for the base partition only
        ///</summary>
        [Description("This function is valid for the base partition only")] public const int
            COMADMIN_E_BASE_PARTITION_ONLY = unchecked((int) 0x80110450);

        ///<summary>
        ///You cannot start an application that has been disabled
        ///</summary>
        [Description("You cannot start an application that has been disabled")] public const int
            COMADMIN_E_START_APP_DISABLED = unchecked((int) 0x80110451);

        ///<summary>
        ///The specified partition name is already in use on this computer
        ///</summary>
        [Description("The specified partition name is already in use on this computer")] public const int
            COMADMIN_E_CAT_DUPLICATE_PARTITION_NAME = unchecked((int) 0x80110457);

        ///<summary>
        ///The specified partition name is invalid. Check that the name contains at least one visible character
        ///</summary>
        [Description(
            "The specified partition name is invalid. Check that the name contains at least one visible character")] public const int COMADMIN_E_CAT_INVALID_PARTITION_NAME = unchecked((int) 0x80110458);

        ///<summary>
        ///The partition cannot be deleted because it is the default partition for one or more users
        ///</summary>
        [Description("The partition cannot be deleted because it is the default partition for one or more users")] public const int COMADMIN_E_CAT_PARTITION_IN_USE = unchecked((int) 0x80110459);

        ///<summary>
        ///The partition cannot be exported, because one or more components in the partition have the same file name
        ///</summary>
        [Description(
            "The partition cannot be exported, because one or more components in the partition have the same file name")
        ] public const int COMADMIN_E_FILE_PARTITION_DUPLICATE_FILES = unchecked((int) 0x8011045A);

        ///<summary>
        ///Applications that contain one or more imported components cannot be installed into a non-base partition
        ///</summary>
        [Description(
            "Applications that contain one or more imported components cannot be installed into a non-base partition")] public const int COMADMIN_E_CAT_IMPORTED_COMPONENTS_NOT_ALLOWED = unchecked((int) 0x8011045B);

        ///<summary>
        ///The application name is not unique and cannot be resolved to an application id
        ///</summary>
        [Description("The application name is not unique and cannot be resolved to an application id")] public const int
            COMADMIN_E_AMBIGUOUS_APPLICATION_NAME = unchecked((int) 0x8011045C);

        ///<summary>
        ///The partition name is not unique and cannot be resolved to a partition id
        ///</summary>
        [Description("The partition name is not unique and cannot be resolved to a partition id")] public const int
            COMADMIN_E_AMBIGUOUS_PARTITION_NAME = unchecked((int) 0x8011045D);

        ///<summary>
        ///The COM+ registry database has not been initialized
        ///</summary>
        [Description("The COM+ registry database has not been initialized")] public const int
            COMADMIN_E_REGDB_NOTINITIALIZED = unchecked((int) 0x80110472);

        ///<summary>
        ///The COM+ registry database is not open
        ///</summary>
        [Description("The COM+ registry database is not open")] public const int COMADMIN_E_REGDB_NOTOPEN =
            unchecked((int) 0x80110473);

        ///<summary>
        ///The COM+ registry database detected a system error
        ///</summary>
        [Description("The COM+ registry database detected a system error")] public const int COMADMIN_E_REGDB_SYSTEMERR
            = unchecked((int) 0x80110474);

        ///<summary>
        ///The COM+ registry database is already running
        ///</summary>
        [Description("The COM+ registry database is already running")] public const int COMADMIN_E_REGDB_ALREADYRUNNING
            = unchecked((int) 0x80110475);

        ///<summary>
        ///This version of the COM+ registry database cannot be migrated
        ///</summary>
        [Description("This version of the COM+ registry database cannot be migrated")] public const int
            COMADMIN_E_MIG_VERSIONNOTSUPPORTED = unchecked((int) 0x80110480);

        ///<summary>
        ///The schema version to be migrated could not be found in the COM+ registry database
        ///</summary>
        [Description("The schema version to be migrated could not be found in the COM+ registry database")] public const
            int COMADMIN_E_MIG_SCHEMANOTFOUND = unchecked((int) 0x80110481);

        ///<summary>
        ///There was a type mismatch between binaries
        ///</summary>
        [Description("There was a type mismatch between binaries")] public const int COMADMIN_E_CAT_BITNESSMISMATCH =
            unchecked((int) 0x80110482);

        ///<summary>
        ///A binary of unknown or invalid type was provided
        ///</summary>
        [Description("A binary of unknown or invalid type was provided")] public const int
            COMADMIN_E_CAT_UNACCEPTABLEBITNESS = unchecked((int) 0x80110483);

        ///<summary>
        ///There was a type mismatch between a binary and an application
        ///</summary>
        [Description("There was a type mismatch between a binary and an application")] public const int
            COMADMIN_E_CAT_WRONGAPPBITNESS = unchecked((int) 0x80110484);

        ///<summary>
        ///The application cannot be paused or resumed
        ///</summary>
        [Description("The application cannot be paused or resumed")] public const int
            COMADMIN_E_CAT_PAUSE_RESUME_NOT_SUPPORTED = unchecked((int) 0x80110485);

        ///<summary>
        ///The COM+ Catalog Server threw an exception during execution
        ///</summary>
        [Description("The COM+ Catalog Server threw an exception during execution")] public const int
            COMADMIN_E_CAT_SERVERFAULT = unchecked((int) 0x80110486);

        //
        // COMPLUS Queued component errors
        //
        ///<summary>
        ///Only COM+ Applications marked "queued" can be invoked using the "queue" moniker
        ///</summary>
        [Description("Only COM+ Applications marked \"queued\" can be invoked using the \"queue\" moniker")] public
            const int COMQC_E_APPLICATION_NOT_QUEUED = unchecked((int) 0x80110600);

        ///<summary>
        ///At least one interface must be marked "queued" in order to create a queued component instance with the "queue" moniker
        ///</summary>
        [Description(
            "At least one interface must be marked \"queued\" in order to create a queued component instance with the \"queue\" moniker"
            )] public const int COMQC_E_NO_QUEUEABLE_INTERFACES = unchecked((int) 0x80110601);

        ///<summary>
        ///MSMQ is required for the requested operation and is not installed
        ///</summary>
        [Description("MSMQ is required for the requested operation and is not installed")] public const int
            COMQC_E_QUEUING_SERVICE_NOT_AVAILABLE = unchecked((int) 0x80110602);

        ///<summary>
        ///Unable to marshal an interface that does not support IPersistStream
        ///</summary>
        [Description("Unable to marshal an interface that does not support IPersistStream")] public const int
            COMQC_E_NO_IPERSISTSTREAM = unchecked((int) 0x80110603);

        ///<summary>
        ///The message is improperly formatted or was damaged in transit
        ///</summary>
        [Description("The message is improperly formatted or was damaged in transit")] public const int
            COMQC_E_BAD_MESSAGE = unchecked((int) 0x80110604);

        ///<summary>
        ///An unauthenticated message was received by an application that accepts only authenticated messages
        ///</summary>
        [Description(
            "An unauthenticated message was received by an application that accepts only authenticated messages")] public const int COMQC_E_UNAUTHENTICATED = unchecked((int) 0x80110605);

        ///<summary>
        ///The message was requeued or moved by a user not in the "QC Trusted User" role
        ///</summary>
        [Description("The message was requeued or moved by a user not in the \"QC Trusted User\" role")] public const
            int COMQC_E_UNTRUSTED_ENQUEUER = unchecked((int) 0x80110606);

        //
        // The range 0x700-0x7ff is reserved for MSDTC errors.
        //
        ///<summary>
        ///Cannot create a duplicate resource of type Distributed Transaction Coordinator
        ///</summary>
        [Description("Cannot create a duplicate resource of type Distributed Transaction Coordinator")] public const int
            MSDTC_E_DUPLICATE_RESOURCE = unchecked((int) 0x80110701);

        //
        // More COMADMIN errors from 0x8**
        //
        ///<summary>
        ///One of the objects being inserted or updated does not belong to a valid parent collection
        ///</summary>
        [Description("One of the objects being inserted or updated does not belong to a valid parent collection")] public const int COMADMIN_E_OBJECT_PARENT_MISSING = unchecked((int) 0x80110808);

        ///<summary>
        ///One of the specified objects cannot be found
        ///</summary>
        [Description("One of the specified objects cannot be found")] public const int COMADMIN_E_OBJECT_DOES_NOT_EXIST
            = unchecked((int) 0x80110809);

        ///<summary>
        ///The specified application is not currently running
        ///</summary>
        [Description("The specified application is not currently running")] public const int COMADMIN_E_APP_NOT_RUNNING
            = unchecked((int) 0x8011080A);

        ///<summary>
        ///The partition(s) specified are not valid.
        ///</summary>
        [Description("The partition(s) specified are not valid.")] public const int COMADMIN_E_INVALID_PARTITION =
            unchecked((int) 0x8011080B);

        ///<summary>
        ///COM+ applications that run as NT service may not be pooled or recycled
        ///</summary>
        [Description("COM+ applications that run as NT service may not be pooled or recycled")] public const int
            COMADMIN_E_SVCAPP_NOT_POOLABLE_OR_RECYCLABLE = unchecked((int) 0x8011080D);

        ///<summary>
        ///One or more users are already assigned to a local partition set.
        ///</summary>
        [Description("One or more users are already assigned to a local partition set.")] public const int
            COMADMIN_E_USER_IN_SET = unchecked((int) 0x8011080E);

        ///<summary>
        ///Library applications may not be recycled.
        ///</summary>
        [Description("Library applications may not be recycled.")] public const int COMADMIN_E_CANTRECYCLELIBRARYAPPS =
            unchecked((int) 0x8011080F);

        ///<summary>
        ///Applications running as NT services may not be recycled.
        ///</summary>
        [Description("Applications running as NT services may not be recycled.")] public const int
            COMADMIN_E_CANTRECYCLESERVICEAPPS = unchecked((int) 0x80110811);

        ///<summary>
        ///The process has already been recycled.
        ///</summary>
        [Description("The process has already been recycled.")] public const int COMADMIN_E_PROCESSALREADYRECYCLED =
            unchecked((int) 0x80110812);

        ///<summary>
        ///A paused process may not be recycled.
        ///</summary>
        [Description("A paused process may not be recycled.")] public const int COMADMIN_E_PAUSEDPROCESSMAYNOTBERECYCLED
            = unchecked((int) 0x80110813);

        ///<summary>
        ///Library applications may not be NT services.
        ///</summary>
        [Description("Library applications may not be NT services.")] public const int COMADMIN_E_CANTMAKEINPROCSERVICE
            = unchecked((int) 0x80110814);

        ///<summary>
        ///The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.
        ///</summary>
        [Description(
            "The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.")] public const int COMADMIN_E_PROGIDINUSEBYCLSID = unchecked((int) 0x80110815);

        ///<summary>
        ///The partition specified as default is not a member of the partition set.
        ///</summary>
        [Description("The partition specified as default is not a member of the partition set.")] public const int
            COMADMIN_E_DEFAULT_PARTITION_NOT_IN_SET = unchecked((int) 0x80110816);

        ///<summary>
        ///A recycled process may not be paused.
        ///</summary>
        [Description("A recycled process may not be paused.")] public const int COMADMIN_E_RECYCLEDPROCESSMAYNOTBEPAUSED
            = unchecked((int) 0x80110817);

        ///<summary>
        ///Access to the specified partition is denied.
        ///</summary>
        [Description("Access to the specified partition is denied.")] public const int COMADMIN_E_PARTITION_ACCESSDENIED
            = unchecked((int) 0x80110818);

        ///<summary>
        ///Only Application Files (*.MSI files) can be installed into partitions.
        ///</summary>
        [Description("Only Application Files (*.MSI files) can be installed into partitions.")] public const int
            COMADMIN_E_PARTITION_MSI_ONLY = unchecked((int) 0x80110819);

        ///<summary>
        ///Applications containing one or more legacy components may not be exported to 1.0 format.
        ///</summary>
        [Description("Applications containing one or more legacy components may not be exported to 1.0 format.")] public
            const int COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_1_0_FORMAT = unchecked((int) 0x8011081A);

        ///<summary>
        ///Legacy components may not exist in non-base partitions.
        ///</summary>
        [Description("Legacy components may not exist in non-base partitions.")] public const int
            COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_NONBASE_PARTITIONS = unchecked((int) 0x8011081B);

        ///<summary>
        ///A component cannot be moved (or copied) from the System Application, an application proxy or a non-changeable application
        ///</summary>
        [Description(
            "A component cannot be moved (or copied) from the System Application, an application proxy or a non-changeable application"
            )] public const int COMADMIN_E_COMP_MOVE_SOURCE = unchecked((int) 0x8011081C);

        ///<summary>
        ///A component cannot be moved (or copied) to the System Application, an application proxy or a non-changeable application
        ///</summary>
        [Description(
            "A component cannot be moved (or copied) to the System Application, an application proxy or a non-changeable application"
            )] public const int COMADMIN_E_COMP_MOVE_DEST = unchecked((int) 0x8011081D);

        ///<summary>
        ///A private component cannot be moved (or copied) to a library application or to the base partition
        ///</summary>
        [Description("A private component cannot be moved (or copied) to a library application or to the base partition"
            )] public const int COMADMIN_E_COMP_MOVE_PRIVATE = unchecked((int) 0x8011081E);

        ///<summary>
        ///The Base Application Partition exists in all partition sets and cannot be removed.
        ///</summary>
        [Description("The Base Application Partition exists in all partition sets and cannot be removed.")] public const
            int COMADMIN_E_BASEPARTITION_REQUIRED_IN_SET = unchecked((int) 0x8011081F);

        ///<summary>
        ///Alas, Event Class components cannot be aliased.
        ///</summary>
        [Description("Alas, Event Class components cannot be aliased.")] public const int
            COMADMIN_E_CANNOT_ALIAS_EVENTCLASS = unchecked((int) 0x80110820);

        ///<summary>
        ///Access is denied because the component is private.
        ///</summary>
        [Description("Access is denied because the component is private.")] public const int
            COMADMIN_E_PRIVATE_ACCESSDENIED = unchecked((int) 0x80110821);

        ///<summary>
        ///The specified SAFER level is invalid.
        ///</summary>
        [Description("The specified SAFER level is invalid.")] public const int COMADMIN_E_SAFERINVALID =
            unchecked((int) 0x80110822);

        ///<summary>
        ///The specified user cannot write to the system registry
        ///</summary>
        [Description("The specified user cannot write to the system registry")] public const int
            COMADMIN_E_REGISTRY_ACCESSDENIED = unchecked((int) 0x80110823);

        ///<summary>
        ///COM+ partitions are currently disabled.
        ///</summary>
        [Description("COM+ partitions are currently disabled.")] public const int COMADMIN_E_PARTITIONS_DISABLED =
            unchecked((int) 0x80110824);

        #endregion

        // ******************
        //  FACILITY_AAF
        // ******************

        #region (0x120000 - 0x12FFFF) FACILITY_AAF=18 errors

        #endregion

        // ******************
        //  FACILITY_URT
        // ******************

        #region (0x130000 - 0x13FFFF) FACILITY_URT=19 errors

        #endregion

        // ******************
        //  FACILITY_ACS
        // ******************

        #region (0x140000 - 0x14FFFF) FACILITY_ACS=20 errors

        #endregion

        // ******************
        //  FACILITY_DPLAY
        // ******************

        #region (0x150000 - 0x15FFFF) FACILITY_DPLAY=21 errors

        #endregion

        // ******************
        //  FACILITY_UMI
        // ******************

        #region (0x160000 - 0x16FFFF) FACILITY_UMI=22 errors

        #endregion

        // ******************
        //  FACILITY_SXS
        // ******************

        #region (0x170000 - 0x17FFFF) FACILITY_SXS=23 errors

        #endregion

        // ******************
        //  FACILITY_WINDOWS_CE
        // ******************

        #region (0x180000 - 0x18FFFF) FACILITY_WINDOWS_CE=24 errors

        #endregion

        // ******************
        //  FACILITY_HTTP
        // ******************

        #region (0x190000 - 0x19FFFF) FACILITY_HTTP=25 errors

        #endregion

        // ******************
        //  FACILITY_26
        // ******************

        #region (0x1A0000 - 0x1AFFFF) FACILITY_26=26 errors

        #endregion

        // ******************
        //  FACILITY_27
        // ******************

        #region (0x1B0000 - 0x1BFFFF) FACILITY_27=27 errors

        #endregion

        // ******************
        //  FACILITY_28
        // ******************

        #region (0x1C0000 - 0x1CFFFF) FACILITY_28=28 errors

        #endregion

        // ******************
        //  FACILITY_29
        // ******************

        #region (0x1D0000 - 0x1DFFFF) FACILITY_29=29 errors

        #endregion

        // ******************
        //  FACILITY_30
        // ******************

        #region (0x1E0000 - 0x1EFFFF) FACILITY_30=30 errors

        #endregion

        // ******************
        //  FACILITY_31
        // ******************

        #region (0x1F0000 - 0x1FFFFF) FACILITY_31=31 errors

        #endregion

        // ******************
        //  FACILITY_BACKGROUNDCOPY
        // ******************

        #region (0x200000 - 0x20FFFF) FACILITY_BACKGROUNDCOPY=32 errors

        #endregion

        // ******************
        //  FACILITY_CONFIGURATION
        // ******************

        #region (0x210000 - 0x21FFFF) FACILITY_CONFIGURATION=33 errors

        #endregion

        // ******************
        //  FACILITY_STATE_MANAGEMENT
        // ******************

        #region (0x220000 - 0x22FFFF) FACILITY_STATE_MANAGEMENT=34 errors

        #endregion

        // ******************
        //  FACILITY_METADIRECTORY
        // ******************

        #region (0x230000 - 0x23FFFF) FACILITY_METADIRECTORY=35 errors

        #endregion

        #endregion Error Codes

        #region OPC Error codes

        [Description("The value of the handle is invalid.")]
        public const int OPC_E_INVALIDHANDLE =
            unchecked((int)0xC0040001);

        [Description("The server cannot convert the data between the specified format/ requested data type and the canonical data type."
            )]
        public const int OPC_E_BADTYPE = unchecked((int)0xC0040004);

        [Description("The requested operation cannot be done on a public group.")]
        public const int OPC_E_PUBLIC =
            unchecked((int)0xC0040005);

        [Description("The item's access rights do not allow the operation.")]
        public const int OPC_E_BADRIGHTS =
            unchecked((int)0xC0040006);

        [Description(
    "The item ID is not defined in the server address space (on add or validate) or no longer exists in the server address space (for read or write)."
    )]
        public const int OPC_E_UNKNOWNITEMID = unchecked((int)0xC0040007);


        [Description("The item ID doesn't conform to the server's syntax.")] public const int OPC_E_INVALIDITEMID =
            unchecked((int) 0xC0040008);

        [Description("The filter string was not valid.")]
        public const int OPC_E_INVALIDFILTER =
            unchecked((int)0xC0040009);

        [Description("The item's access path is not known to the server.")] public const int OPC_E_UNKNOWNPATH =
            unchecked((int) 0xC004000A);

        [Description("The value was out of range.")]
        public const int OPC_E_RANGE =
            unchecked((int)0xC004000B);

        [Description("Duplicate name not allowed.")]
        public const int OPC_E_DUPLICATENAME =
            unchecked((int)0xC004000C);

        [Description("The server does not support the requested data rate but will use the closest available rate.")]
        public const int OPC_S_UNSUPPORTEDRATE =
            unchecked((int)0x0004000D);

        [Description("A value passed to write was accepted but the output was clamped.")]
        public const int OPC_S_CLAMP =
            unchecked((int)0x0004000E);

        [Description("The operation cannot be performed because the object is being referenced.")]
        public const int OPC_S_INUSE =
            unchecked((int)0x0004000F);

        [Description("The server's configuration file is an invalid format.")]
        public const int OPC_E_INVALIDCONFIGFILE =
            unchecked((int)0xC0040010);

        [Description("The requested object (e.g. a public group) was not found.")]
        public const int OPC_E_NOTFOUND =
            unchecked((int)0xC0040011);

        [Description("The specified property ID is not valid for the item.")]
        public const int OPC_E_INVALID_PID =
            unchecked((int)0xC0040203);

        [Description("The item does not support deadband.")]
        public const int OPC_E_DEADBANDNOTSUPPORTED =
            unchecked((int)0xC0040401);

        [Description("The server does not support buffering of data items that are collected at a faster rate than the group update rate.")]
        public const int OPC_E_NOBUFFERING =
            unchecked((int)0xC0040402);

        [Description("The continuation point is not valid.")]
        public const int OPC_E_INVALIDCONTINUATIONPOINT =
            unchecked((int)0xC0040403);

        [Description("Data Queue Overflow - Some value transitions were lost.")]
        public const int OPC_S_DATAQUEUEOVERFLOW =
            unchecked((int)0x00040404);

        [Description("Server does not support requested rate.")]
        public const int OPC_E_RATENOTSET =
            unchecked((int)0xC0040405);

        [Description("The server does not support writing of quality and/or timestamp.")]
        public const int OPC_E_NOTSUPPORTED =
            unchecked((int)0xC0040406);
        #endregion

        private class ValueToDescription : Dictionary<int, string>
        {
            public ValueToDescription()
                : base(EqualityComparer<int>.Default)
            {
            }

            public ValueToDescription(int capacity)
                : base(capacity, EqualityComparer<int>.Default)
            {
            }
        }

        private static readonly ValueToDescription valueToDescription;

        /// <summary>
        /// Initializes the <see cref="HRESULT"/> struct.
        /// </summary>
        static HRESULT()
        {
            valueToDescription = new ValueToDescription(1284);
            FieldInfo[] fieldsInfo = typeof (HRESULT).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in fieldsInfo)
            {
                if (fi.GetValue(null) is int)
                {
                    int hr = (int) fi.GetValue(null);
                    if (!valueToDescription.ContainsKey(hr))
                        valueToDescription[hr] = GetDescription(fi);
                }
            }
        }

        private static string GetDescription(FieldInfo fi)
        {
            object[] o = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            DescriptionAttribute descr = null;
            if (o != null && o.Length > 0) descr = (DescriptionAttribute)o[0];
            string s = (descr != null) ? fi.Name + ": " + descr.description : fi.Name;
            return s;
        }
    }

    #endregion HRESULT
}