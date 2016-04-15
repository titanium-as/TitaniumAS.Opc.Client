using System;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop
{
    public static class Com
    {
        private const uint CLSCTX_INPROC_SERVER = 0x1;
        private const uint CLSCTX_INPROC_HANDLER = 0x2;
        private const uint CLSCTX_LOCAL_SERVER = 0x4;
        private const uint CLSCTX_REMOTE_SERVER = 0x10;
        private const uint SEC_WINNT_AUTH_IDENTITY_ANSI = 0x1;
        private const uint SEC_WINNT_AUTH_IDENTITY_UNICODE = 0x2;
        private const uint RPC_C_AUTHN_NONE = 0;
        private const uint RPC_C_AUTHN_DCE_PRIVATE = 1;
        private const uint RPC_C_AUTHN_DCE_PUBLIC = 2;
        private const uint RPC_C_AUTHN_DEC_PUBLIC = 4;
        private const uint RPC_C_AUTHN_GSS_NEGOTIATE = 9;
        private const uint RPC_C_AUTHN_WINNT = 10;
        private const uint RPC_C_AUTHN_GSS_SCHANNEL = 14;
        private const uint RPC_C_AUTHN_GSS_KERBEROS = 16;
        private const uint RPC_C_AUTHN_DPA = 17;
        private const uint RPC_C_AUTHN_MSN = 18;
        private const uint RPC_C_AUTHN_DIGEST = 21;
        private const uint RPC_C_AUTHN_MQ = 100;
        private const uint RPC_C_AUTHN_DEFAULT = 0xFFFFFFFF;
        private const uint RPC_C_AUTHZ_NONE = 0;
        private const uint RPC_C_AUTHZ_NAME = 1;
        private const uint RPC_C_AUTHZ_DCE = 2;
        private const uint RPC_C_AUTHZ_DEFAULT = 0xffffffff;
        private const uint RPC_C_AUTHN_LEVEL_DEFAULT = 0;
        private const uint RPC_C_AUTHN_LEVEL_NONE = 1;
        private const uint RPC_C_AUTHN_LEVEL_CONNECT = 2;
        private const uint RPC_C_AUTHN_LEVEL_CALL = 3;
        private const uint RPC_C_AUTHN_LEVEL_PKT = 4;
        private const uint RPC_C_AUTHN_LEVEL_PKT_INTEGRITY = 5;
        private const uint RPC_C_AUTHN_LEVEL_PKT_PRIVACY = 6;
        private const uint RPC_C_IMP_LEVEL_ANONYMOUS = 1;
        private const uint RPC_C_IMP_LEVEL_IDENTIFY = 2;
        private const uint RPC_C_IMP_LEVEL_IMPERSONATE = 3;
        private const uint RPC_C_IMP_LEVEL_DELEGATE = 4;
        private const uint EOAC_NONE = 0x00;
        private const uint EOAC_MUTUAL_AUTH = 0x01;
        private const uint EOAC_CLOAKING = 0x10;
        private const uint EOAC_SECURE_REFS = 0x02;
        private const uint EOAC_ACCESS_CONTROL = 0x04;
        private const uint EOAC_APPID = 0x08;
        private const uint EOAC_DEFAULT = 0x80;

        /// <summary>
        ///     The size, in bytes, of a VARIANT structure.
        /// </summary>
        private const int VARIANT_SIZE = 0x10;

        private const int MAX_MESSAGE_LENGTH = 1024;
        private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        public static readonly Guid IUnknownIID = new Guid("00000000-0000-0000-C000-000000000046");
        private static bool m_preserveUTC;
        private static readonly DateTime FILETIME_BaseTime = new DateTime(1601, 1, 1);

        /// <summary>
        ///     This flag supresses the conversion to local time done during marhsalling.
        /// </summary>
        public static bool PreserveUTC
        {
            get
            {
                lock (typeof (Com))
                {
                    return m_preserveUTC;
                }
            }
            set
            {
                lock (typeof (Com))
                {
                    m_preserveUTC = value;
                }
            }
        }

        [DllImport("ole32.dll")]
        private static extern int CoInitializeSecurity(
            IntPtr pSecDesc,
            int cAuthSvc,
            SOLE_AUTHENTICATION_SERVICE[] asAuthSvc,
            IntPtr pReserved1,
            uint dwAuthnLevel,
            uint dwImpLevel,
            IntPtr pAuthList,
            uint dwCapabilities,
            IntPtr pReserved3);

        [DllImport("ole32.dll")]
        private static extern void CoCreateInstanceEx(
            ref Guid clsid,
            [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
            uint dwClsCtx,
            [In] ref COSERVERINFO pServerInfo,
            uint dwCount,
            [In, Out] MULTI_QI[] pResults);

        [DllImport("ole32.dll")]
        private static extern void CoGetClassObject(
            [MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
            uint dwClsContext,
            [In] ref COSERVERINFO pServerInfo,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppv);

        [DllImport("ole32.dll")]
        private static extern int CoSetProxyBlanket(
            [MarshalAs(UnmanagedType.IUnknown)] object pProxy, uint dwAuthnSvc, uint dwAuthzSvc,
            [MarshalAs(UnmanagedType.LPWStr)] string pServerPrincName, uint dwAuthnLevel,
            uint dwImpLevel, IntPtr pAuthInfo, uint dwCapabilities);

        /// <summary>
        ///     Initializes COM security.
        /// </summary>
        public static void InitializeSecurity()
        {
            int error = CoInitializeSecurity(
                IntPtr.Zero,
                -1,
                null,
                IntPtr.Zero,
                RPC_C_AUTHN_LEVEL_NONE,
                RPC_C_IMP_LEVEL_IDENTIFY,
                IntPtr.Zero,
                EOAC_NONE,
                IntPtr.Zero);

            if (error != 0)
            {
                throw new ExternalException("CoInitializeSecurity: " + GetSystemMessage(error), error);
            }
        }

        public static void SetProxyBlanket(object comObject, ComProxyBlanket comProxyBlanket = null)
        {
            if (comProxyBlanket != null)
            {
                var result =
                    new HRESULT(CoSetProxyBlanket(comObject, (uint) comProxyBlanket.RpcAuthService, (uint) comProxyBlanket.RpcAuthType,
                        null, (uint) comProxyBlanket.RpcAuthnLevel, (uint) comProxyBlanket.RpcImpLevel, IntPtr.Zero,
                        (uint) comProxyBlanket.DwCapabilities));
                if (result.Failed)
                {
                    throw HRESULT.GetExceptionForHR(result);
                }
            }
        }

        /// <summary>
        ///     Creates an instance of a COM server.
        /// </summary>
        public static object CreateInstance(Guid clsid, string host, NetworkCredential credential)
        {
//            var serverType = Type.GetTypeFromCLSID(clsid, host);
//            var comObject = Activator.CreateInstance(serverType);
//            return comObject;

            var serverInfo = new ServerInfo();
            COSERVERINFO coserverInfo = serverInfo.Allocate(host, credential);

            GCHandle hIID = GCHandle.Alloc(IUnknownIID, GCHandleType.Pinned);

            var results = new MULTI_QI[1];

            results[0].iid = hIID.AddrOfPinnedObject();
            results[0].pItf = null;
            results[0].hr = 0;

            try
            {
                // check whether connecting locally or remotely.
                uint clsctx = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER;

                if (host != null && host.Length > 0 && host != "localhost")
                {
                    clsctx = CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER;
                }

                // create an instance.
                CoCreateInstanceEx(
                    ref clsid,
                    null,
                    clsctx,
                    ref coserverInfo,
                    1,
                    results);
            }
            finally
            {
                if (hIID.IsAllocated) hIID.Free();
                serverInfo.Deallocate();
            }

            if (results[0].hr != 0)
            {
                throw new ExternalException("CoCreateInstanceEx: " + GetSystemMessage((int) results[0].hr));
            }

            return results[0].pItf;
        }

        /// <summary>
        ///     Creates an instance of a COM server and call CoSetProxyBlanket.
        /// </summary>
        public static object CreateInstanceWithBlanket(Guid clsid, string host, NetworkCredential credential,
            ComProxyBlanket comProxyBlanket = null)
        {
            object obj = CreateInstance(clsid, host, credential);
            SetProxyBlanket(obj, comProxyBlanket);
            return obj;
        }

        /// <summary>
        ///     Creates an instance of a COM server using the specified license key and call CoSetProxyBlanket.
        /// </summary>
        public static object CreateInstanceWithLicenseKeyAndBlanket(Guid clsid, string host,
            NetworkCredential credential, string licenseKey, ComProxyBlanket comProxyBlanket = null)
        {
            object obj = CreateInstanceWithLicenseKey(clsid, host, credential, licenseKey);
            SetProxyBlanket(obj, comProxyBlanket);
            return obj;
        }

        /// <summary>
        ///     Creates an instance of a COM server using the specified license key.
        /// </summary>
        public static object CreateInstanceWithLicenseKey(Guid clsid, string hostName, NetworkCredential credential,
            string licenseKey)
        {
            var serverInfo = new ServerInfo();
            COSERVERINFO coserverInfo = serverInfo.Allocate(hostName, credential);
            object instance = null;
            IClassFactory2 factory = null;

            try
            {
                // check whether connecting locally or remotely.
                uint clsctx = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER;

                if (hostName != null && hostName.Length > 0)
                {
                    clsctx = CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER;
                }

                // get the class factory.
                object unknown = null;

                CoGetClassObject(
                    clsid,
                    clsctx,
                    ref coserverInfo,
                    typeof (IClassFactory2).GUID,
                    out unknown);

                factory = (IClassFactory2) unknown;

                // set the proper connect authentication level
                var security = (IClientSecurity) factory;

                uint pAuthnSvc = 0;
                uint pAuthzSvc = 0;
                string pServerPrincName = "";
                uint pAuthnLevel = 0;
                uint pImpLevel = 0;
                IntPtr pAuthInfo = IntPtr.Zero;
                uint pCapabilities = 0;

                // get existing security settings.
                security.QueryBlanket(
                    factory,
                    ref pAuthnSvc,
                    ref pAuthzSvc,
                    ref pServerPrincName,
                    ref pAuthnLevel,
                    ref pImpLevel,
                    ref pAuthInfo,
                    ref pCapabilities);

                pAuthnSvc = RPC_C_AUTHN_DEFAULT;
                pAuthnLevel = RPC_C_AUTHN_LEVEL_CONNECT;

                // update security settings.
                security.SetBlanket(
                    factory,
                    pAuthnSvc,
                    pAuthzSvc,
                    pServerPrincName,
                    pAuthnLevel,
                    pImpLevel,
                    pAuthInfo,
                    pCapabilities);

                // create instance.
                factory.CreateInstanceLic(
                    null,
                    null,
                    IUnknownIID,
                    licenseKey,
                    out instance);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                serverInfo.Deallocate();
            }

            return instance;
        }

        /// <summary>
        ///     Unmarshals and frees an array of 32 bit integers.
        /// </summary>
        public static int[] GetInt32s(ref IntPtr pArray, int size, bool deallocate)
        {
            if (pArray == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var array = new int[size];
            Marshal.Copy(pArray, array, 0, size);

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pArray);
                pArray = IntPtr.Zero;
            }

            return array;
        }

        /// <summary>
        ///     Allocates and marshals an array of 32 bit integers.
        /// </summary>
        public static IntPtr GetInt32s(int[] input)
        {
            IntPtr output = IntPtr.Zero;

            if (input != null)
            {
                output = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (int))*input.Length);
                Marshal.Copy(input, 0, output, input.Length);
            }

            return output;
        }

        /// <summary>
        ///     Unmarshals and frees a array of 16 bit integers.
        /// </summary>
        public static short[] GetInt16s(ref IntPtr pArray, int size, bool deallocate)
        {
            if (pArray == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var array = new short[size];
            Marshal.Copy(pArray, array, 0, size);

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pArray);
                pArray = IntPtr.Zero;
            }

            return array;
        }

        /// <summary>
        ///     Allocates and marshals an array of 16 bit integers.
        /// </summary>
        public static IntPtr GetInt16s(short[] input)
        {
            IntPtr output = IntPtr.Zero;

            if (input != null)
            {
                output = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (short))*input.Length);
                Marshal.Copy(input, 0, output, input.Length);
            }

            return output;
        }

        /// <summary>
        ///     Marshals an array of strings into a unmanaged memory buffer
        /// </summary>
        /// <param name="values">The array of strings to marshal</param>
        /// <returns>The pointer to the unmanaged memory buffer</returns>
        public static IntPtr GetUnicodeStrings(string[] values)
        {
            int size = (values != null) ? values.Length : 0;

            if (size <= 0)
            {
                return IntPtr.Zero;
            }

            IntPtr pValues = IntPtr.Zero;

            var pointers = new int[size];

            for (int ii = 0; ii < size; ii++)
            {
                pointers[ii] = (int) Marshal.StringToCoTaskMemUni(values[ii]);
            }

            pValues = Marshal.AllocCoTaskMem(values.Length*Marshal.SizeOf(typeof (IntPtr)));
            Marshal.Copy(pointers, 0, pValues, size);

            return pValues;
        }

        /// <summary>
        ///     Unmarshals and frees a array of unicode strings.
        /// </summary>
        public static string[] GetUnicodeStrings(ref IntPtr pArray, int size, bool deallocate)
        {
            if (pArray == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var pointers = new int[size];
            Marshal.Copy(pArray, pointers, 0, size);

            var strings = new string[size];

            for (int ii = 0; ii < size; ii++)
            {
                var pString = (IntPtr) pointers[ii];
                strings[ii] = Marshal.PtrToStringUni(pString);
                if (deallocate) Marshal.FreeCoTaskMem(pString);
            }

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pArray);
                pArray = IntPtr.Zero;
            }

            return strings;
        }

        /// <summary>
        ///     Marshals a DateTime as a WIN32 FILETIME.
        /// </summary>
        /// <param name="datetime">The DateTime object to marshal</param>
        /// <returns>The WIN32 FILETIME</returns>
        public static FILETIME GetFILETIME(DateTime datetime)
        {
            FILETIME filetime;

            if (datetime <= FILETIME_BaseTime)
            {
                filetime.dwHighDateTime = 0;
                filetime.dwLowDateTime = 0;
                return filetime;
            }

            // adjust for WIN32 FILETIME base.
            long ticks = 0;

            if (m_preserveUTC)
            {
                ticks = datetime.Subtract(new TimeSpan(FILETIME_BaseTime.Ticks)).Ticks;
            }
            else
            {
                ticks = (datetime.ToUniversalTime().Subtract(new TimeSpan(FILETIME_BaseTime.Ticks))).Ticks;
            }

            filetime.dwHighDateTime = (int) ((ticks >> 32) & 0xFFFFFFFF);
            filetime.dwLowDateTime = (int) (ticks & 0xFFFFFFFF);

            return filetime;
        }

        /// <summary>
        ///     Unmarshals a WIN32 FILETIME from a pointer.
        /// </summary>
        /// <param name="pFiletime">A pointer to a FILETIME structure.</param>
        /// <returns>A DateTime object.</returns>
        public static DateTime GetFILETIME(IntPtr pFiletime)
        {
            if (pFiletime == IntPtr.Zero)
            {
                return DateTime.MinValue;
            }

            return GetFILETIME((FILETIME) Marshal.PtrToStructure(pFiletime, typeof (FILETIME)));
        }

        /// <summary>
        ///     Unmarshals a WIN32 FILETIME.
        /// </summary>
        public static DateTime GetFILETIME(FILETIME filetime)
        {
            // convert FILETIME structure to a 64 bit integer.
            long buffer = filetime.dwHighDateTime;

            if (buffer < 0)
            {
                buffer += ((long) uint.MaxValue + 1);
            }

            long ticks = (buffer << 32);

            buffer = filetime.dwLowDateTime;

            if (buffer < 0)
            {
                buffer += ((long) uint.MaxValue + 1);
            }

            ticks += buffer;

            // check for invalid value.
            if (ticks == 0)
            {
                return DateTime.MinValue;
            }

            // adjust for WIN32 FILETIME base.			
            if (m_preserveUTC)
            {
                return FILETIME_BaseTime.Add(new TimeSpan(ticks));
            }
            return FILETIME_BaseTime.Add(new TimeSpan(ticks)).ToLocalTime();
        }

        /// <summary>
        ///     Marshals an array of DateTimes into an unmanaged array of FILETIMEs
        /// </summary>
        /// <param name="datetimes">The array of DateTimes to marshal</param>
        /// <returns>The IntPtr array of FILETIMEs</returns>
        public static IntPtr GetFILETIMEs(DateTime[] datetimes)
        {
            int count = (datetimes != null) ? datetimes.Length : 0;

            if (count <= 0)
            {
                return IntPtr.Zero;
            }

            IntPtr pFiletimes = Marshal.AllocCoTaskMem(count*Marshal.SizeOf(typeof (FILETIME)));

            IntPtr pos = pFiletimes;

            for (int ii = 0; ii < count; ii++)
            {
                Marshal.StructureToPtr(GetFILETIME(datetimes[ii]), pos, false);
                pos = (IntPtr) (pos.ToInt32() + Marshal.SizeOf(typeof (FILETIME)));
            }

            return pFiletimes;
        }

        /// <summary>
        ///     Unmarshals an array of WIN32 FILETIMEs as DateTimes.
        /// </summary>
        public static DateTime[] GetFILETIMEs(ref IntPtr pArray, int size, bool deallocate)
        {
            if (pArray == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var datetimes = new DateTime[size];

            IntPtr pos = pArray;

            for (int ii = 0; ii < size; ii++)
            {
                datetimes[ii] = GetFILETIME(pos);
                pos = (IntPtr) (pos.ToInt32() + Marshal.SizeOf(typeof (FILETIME)));
            }

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pArray);
                pArray = IntPtr.Zero;
            }

            return datetimes;
        }

        /// <summary>
        ///     Unmarshals an array of WIN32 GUIDs as Guid.
        /// </summary>
        public static Guid[] GetGUIDs(ref IntPtr pInput, int size, bool deallocate)
        {
            if (pInput == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var guids = new Guid[size];

            IntPtr pos = pInput;

            for (int ii = 0; ii < size; ii++)
            {
                var input = (GUID) Marshal.PtrToStructure(pInput, typeof (GUID));

                guids[ii] = new Guid(input.Data1, input.Data2, input.Data3, input.Data4);

                pos = (IntPtr) (pos.ToInt32() + Marshal.SizeOf(typeof (GUID)));
            }

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pInput);
                pInput = IntPtr.Zero;
            }

            return guids;
        }

        /// <summary>
        ///     Frees all memory referenced by a VARIANT stored in unmanaged memory.
        /// </summary>
        [DllImport("oleaut32.dll")]
        public static extern void VariantClear(IntPtr pVariant);

        /// <summary>
        ///     Converts an object into a value that can be marshalled to a VARIANT.
        /// </summary>
        /// <param name="source">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static object GetVARIANT(object source)
        {
            // check for invalid args.
            if (source == null || source.GetType() == null)
            {
                return null;
            }

            // convert a decimal array to an object array since decimal arrays can't be converted to a variant.
            if (source.GetType() == typeof (decimal[]))
            {
                var srcArray = (decimal[]) source;
                var dstArray = new object[srcArray.Length];

                for (int ii = 0; ii < srcArray.Length; ii++)
                {
                    try
                    {
                        dstArray[ii] = srcArray[ii];
                    }
                    catch (Exception)
                    {
                        dstArray[ii] = double.NaN;
                    }
                }

                return dstArray;
            }

            // no conversion required.
            return source;
        }

        /// <summary>
        ///     Marshals an array objects into an unmanaged array of VARIANTs.
        /// </summary>
        /// <param name="values">An array of the objects to be marshalled</param>
        /// <param name="preprocess">Whether the objects should have troublesome types removed before marhalling.</param>
        /// <returns>An pointer to the array in unmanaged memory</returns>
        public static IntPtr GetVARIANTs(object[] values, bool preprocess)
        {
            int count = (values != null) ? values.Length : 0;

            if (count <= 0)
            {
                return IntPtr.Zero;
            }

            IntPtr pValues = Marshal.AllocCoTaskMem(count*VARIANT_SIZE);

            IntPtr pos = pValues;

            for (int ii = 0; ii < count; ii++)
            {
                if (preprocess)
                {
                    Marshal.GetNativeVariantForObject(GetVARIANT(values[ii]), pos);
                }
                else
                {
                    Marshal.GetNativeVariantForObject(values[ii], pos);
                }

                pos = (IntPtr) (pos.ToInt32() + VARIANT_SIZE);
            }

            return pValues;
        }

        /// <summary>
        ///     Unmarshals an array of VARIANTs as objects.
        /// </summary>
        public static object[] GetVARIANTs(ref IntPtr pArray, int size, bool deallocate)
        {
            // this method unmarshals VARIANTs one at a time because a single bad value throws 
            // an exception with GetObjectsForNativeVariants(). This approach simply sets the 
            // offending value to null.

            if (pArray == IntPtr.Zero || size <= 0)
            {
                return null;
            }

            var values = new object[size];

            IntPtr pos = pArray;

            for (int ii = 0; ii < size; ii++)
            {
                try
                {
                    values[ii] = Marshal.GetObjectForNativeVariant(pos);
                    if (deallocate) VariantClear(pos);
                }
                catch (Exception)
                {
                    values[ii] = null;
                }

                pos = (IntPtr) (pos.ToInt32() + VARIANT_SIZE);
            }

            if (deallocate)
            {
                Marshal.FreeCoTaskMem(pArray);
                pArray = IntPtr.Zero;
            }

            return values;
        }

//        /// <summary>
//        /// Converts the VARTYPE to a system type.
//        /// </summary>
//        internal static System.Type GetType(VarEnum input)
//        {
//            switch (input)
//            {
//                case VarEnum.VT_EMPTY: return null;
//                case VarEnum.VT_I1: return typeof(sbyte);
//                case VarEnum.VT_UI1: return typeof(byte);
//                case VarEnum.VT_I2: return typeof(short);
//                case VarEnum.VT_UI2: return typeof(ushort);
//                case VarEnum.VT_I4: return typeof(int);
//                case VarEnum.VT_UI4: return typeof(uint);
//                case VarEnum.VT_I8: return typeof(long);
//                case VarEnum.VT_UI8: return typeof(ulong);
//                case VarEnum.VT_R4: return typeof(float);
//                case VarEnum.VT_R8: return typeof(double);
//                case VarEnum.VT_CY: return typeof(decimal);
//                case VarEnum.VT_BOOL: return typeof(bool);
//                case VarEnum.VT_DATE: return typeof(DateTime);
//                case VarEnum.VT_BSTR: return typeof(string);
//                case VarEnum.VT_ARRAY | VarEnum.VT_I1: return typeof(sbyte[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_UI1: return typeof(byte[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_I2: return typeof(short[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_UI2: return typeof(ushort[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_I4: return typeof(int[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_UI4: return typeof(uint[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_I8: return typeof(long[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_UI8: return typeof(ulong[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_R4: return typeof(float[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_R8: return typeof(double[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_CY: return typeof(decimal[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_BOOL: return typeof(bool[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_DATE: return typeof(DateTime[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_BSTR: return typeof(string[]);
//                case VarEnum.VT_ARRAY | VarEnum.VT_VARIANT: return typeof(object[]);
//                default: return Opc.Type.ILLEGAL_TYPE;
//            }
//        }

//        /// <summary>
//        /// Converts the system type to a VARTYPE.
//        /// </summary>
//        internal static VarEnum GetType(System.Type input)
//        {
//            if (input == null) return VarEnum.VT_EMPTY;
//            if (input == typeof(sbyte)) return VarEnum.VT_I1;
//            if (input == typeof(byte)) return VarEnum.VT_UI1;
//            if (input == typeof(short)) return VarEnum.VT_I2;
//            if (input == typeof(ushort)) return VarEnum.VT_UI2;
//            if (input == typeof(int)) return VarEnum.VT_I4;
//            if (input == typeof(uint)) return VarEnum.VT_UI4;
//            if (input == typeof(long)) return VarEnum.VT_I8;
//            if (input == typeof(ulong)) return VarEnum.VT_UI8;
//            if (input == typeof(float)) return VarEnum.VT_R4;
//            if (input == typeof(double)) return VarEnum.VT_R8;
//            if (input == typeof(decimal)) return VarEnum.VT_CY;
//            if (input == typeof(bool)) return VarEnum.VT_BOOL;
//            if (input == typeof(DateTime)) return VarEnum.VT_DATE;
//            if (input == typeof(string)) return VarEnum.VT_BSTR;
//            if (input == typeof(object)) return VarEnum.VT_EMPTY;
//            if (input == typeof(sbyte[])) return VarEnum.VT_ARRAY | VarEnum.VT_I1;
//            if (input == typeof(byte[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI1;
//            if (input == typeof(short[])) return VarEnum.VT_ARRAY | VarEnum.VT_I2;
//            if (input == typeof(ushort[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI2;
//            if (input == typeof(int[])) return VarEnum.VT_ARRAY | VarEnum.VT_I4;
//            if (input == typeof(uint[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
//            if (input == typeof(long[])) return VarEnum.VT_ARRAY | VarEnum.VT_I8;
//            if (input == typeof(ulong[])) return VarEnum.VT_ARRAY | VarEnum.VT_UI8;
//            if (input == typeof(float[])) return VarEnum.VT_ARRAY | VarEnum.VT_R4;
//            if (input == typeof(double[])) return VarEnum.VT_ARRAY | VarEnum.VT_R8;
//            if (input == typeof(decimal[])) return VarEnum.VT_ARRAY | VarEnum.VT_CY;
//            if (input == typeof(bool[])) return VarEnum.VT_ARRAY | VarEnum.VT_BOOL;
//            if (input == typeof(DateTime[])) return VarEnum.VT_ARRAY | VarEnum.VT_DATE;
//            if (input == typeof(string[])) return VarEnum.VT_ARRAY | VarEnum.VT_BSTR;
//            if (input == typeof(object[])) return VarEnum.VT_ARRAY | VarEnum.VT_VARIANT;
//
//            // check for special types.
//            if (input == Opc.Type.ILLEGAL_TYPE) return (VarEnum)Enum.ToObject(typeof(VarEnum), 0x7FFF);
//            if (input == typeof(System.Type)) return VarEnum.VT_I2;
//            if (input == typeof(Opc.Da.Quality)) return VarEnum.VT_I2;
//            if (input == typeof(Opc.Da.accessRights)) return VarEnum.VT_I4;
//            if (input == typeof(Opc.Da.euType)) return VarEnum.VT_I4;
//
//            return VarEnum.VT_EMPTY;
//        }


        /// <summary>
        ///     Releases the server if it is a true COM server.
        /// </summary>
        public static void ReleaseComServer(this object server)
        {
            if (server != null && server.GetType().IsCOMObject)
            {
                Marshal.ReleaseComObject(server);
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern int FormatMessageW(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            IntPtr lpBuffer,
            int nSize,
            IntPtr Arguments);

        /// <summary>
        ///     Retrieves the system message text for the specified error.
        /// </summary>
        public static string GetSystemMessage(int error)
        {
            IntPtr buffer = Marshal.AllocCoTaskMem(MAX_MESSAGE_LENGTH);

            int result = FormatMessageW(
                (int) (FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_FROM_SYSTEM),
                IntPtr.Zero,
                error,
                0,
                buffer,
                MAX_MESSAGE_LENGTH - 1,
                IntPtr.Zero);

            string msg = Marshal.PtrToStringUni(buffer);
            Marshal.FreeCoTaskMem(buffer);

            if (msg != null && msg.Length > 0)
            {
                return msg;
            }

            return string.Format("0x{0,0:X}", error);
        }

        public static T QueryInterface<T>(this object comServer) where T : class
        {
            return (T) comServer;
        }

        public static T TryQueryInterface<T>(this object comServer) where T : class
        {
            return comServer as T;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct COAUTHIDENTITY
        {
            public IntPtr User;
            public uint UserLength;
            public IntPtr Domain;
            public uint DomainLength;
            public IntPtr Password;
            public uint PasswordLength;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct COAUTHINFO
        {
            public uint dwAuthnSvc;
            public uint dwAuthzSvc;
            public IntPtr pwszServerPrincName;
            public uint dwAuthnLevel;
            public uint dwImpersonationLevel;
            public IntPtr pAuthIdentityData;
            public uint dwCapabilities;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct COSERVERINFO
        {
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.LPWStr)] public string pwszName;
            public IntPtr pAuthInfo;
            public uint dwReserved2;
        };

        /// <summary>
        ///     WIN32 GUID struct declaration.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct GUID
        {
            public readonly int Data1;
            public readonly short Data2;
            public readonly short Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public readonly byte[] Data4;
        }

        [ComImport]
        [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IClassFactory2
        {
            void CreateInstance(
                [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.Interface)] [Out] out object ppvObject);

            void LockServer(
                [MarshalAs(UnmanagedType.Bool)] bool fLock);

            void GetLicInfo(
                [In, Out] ref LICINFO pLicInfo);

            void RequestLicKey(
                int dwReserved,
                [MarshalAs(UnmanagedType.BStr)] string pbstrKey);

            void CreateInstanceLic(
                [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
                [MarshalAs(UnmanagedType.IUnknown)] object punkReserved,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.BStr)] string bstrKey,
                [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppvObject);
        }

        [ComImport]
        [Guid("0000013D-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IClientSecurity
        {
            void QueryBlanket(
                [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
                ref uint pAuthnSvc,
                ref uint pAuthzSvc,
                [MarshalAs(UnmanagedType.LPWStr)] ref string pServerPrincName,
                ref uint pAuthnLevel,
                ref uint pImpLevel,
                ref IntPtr pAuthInfo,
                ref uint pCapabilities);

            void SetBlanket(
                [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
                uint pAuthnSvc,
                uint pAuthzSvc,
                [MarshalAs(UnmanagedType.LPWStr)] string pServerPrincName,
                uint pAuthnLevel,
                uint pImpLevel,
                IntPtr pAuthInfo,
                uint pCapabilities);

            void CopyProxy(
                [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
                [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppCopy);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LICINFO
        {
            public readonly int cbLicInfo;
            [MarshalAs(UnmanagedType.Bool)] public readonly bool fRuntimeKeyAvail;
            [MarshalAs(UnmanagedType.Bool)] public readonly bool fLicVerified;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MULTI_QI
        {
            public IntPtr iid;
            [MarshalAs(UnmanagedType.IUnknown)] public object pItf;
            public uint hr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SOLE_AUTHENTICATION_SERVICE
        {
            public readonly uint dwAuthnSvc;
            public readonly uint dwAuthzSvc;
            [MarshalAs(UnmanagedType.LPWStr)] public readonly string pPrincipalName;
            public readonly int hr;
        }

        #region ServerInfo Class

        /// <summary>
        ///     A class used to allocate and deallocate the elements of a COSERVERINFO structure.
        /// </summary>
        private class ServerInfo
        {
            #region Public Interface

            /// <summary>
            ///     Allocates a COSERVERINFO structure.
            /// </summary>
            public COSERVERINFO Allocate(string hostName, NetworkCredential credential)
            {
                string userName = null;
                string password = null;
                string domain = null;

                if (credential != null)
                {
                    userName = credential.UserName;
                    password = credential.Password;
                    domain = credential.Domain;
                }

                m_hUserName = GCHandle.Alloc(userName, GCHandleType.Pinned);
                m_hPassword = GCHandle.Alloc(password, GCHandleType.Pinned);
                m_hDomain = GCHandle.Alloc(domain, GCHandleType.Pinned);

                m_hIdentity = new GCHandle();

                if (userName != null && userName != string.Empty)
                {
                    var identity = new COAUTHIDENTITY();

                    identity.User = m_hUserName.AddrOfPinnedObject();
                    identity.UserLength = (uint) ((userName != null) ? userName.Length : 0);
                    identity.Password = m_hPassword.AddrOfPinnedObject();
                    identity.PasswordLength = (uint) ((password != null) ? password.Length : 0);
                    identity.Domain = m_hDomain.AddrOfPinnedObject();
                    identity.DomainLength = (uint) ((domain != null) ? domain.Length : 0);
                    identity.Flags = SEC_WINNT_AUTH_IDENTITY_UNICODE;

                    m_hIdentity = GCHandle.Alloc(identity, GCHandleType.Pinned);
                }

                var authInfo = new COAUTHINFO();
                authInfo.dwAuthnSvc = RPC_C_AUTHN_WINNT;
                authInfo.dwAuthzSvc = RPC_C_AUTHZ_NONE;
                authInfo.pwszServerPrincName = IntPtr.Zero;
                authInfo.dwAuthnLevel = RPC_C_AUTHN_LEVEL_CONNECT;
                authInfo.dwImpersonationLevel = RPC_C_IMP_LEVEL_IMPERSONATE;
                authInfo.pAuthIdentityData = (m_hIdentity.IsAllocated) ? m_hIdentity.AddrOfPinnedObject() : IntPtr.Zero;
                authInfo.dwCapabilities = EOAC_NONE;

                m_hAuthInfo = GCHandle.Alloc(authInfo, GCHandleType.Pinned);

                var serverInfo = new COSERVERINFO();

                serverInfo.pwszName = hostName;
                serverInfo.pAuthInfo = (credential != null) ? m_hAuthInfo.AddrOfPinnedObject() : IntPtr.Zero;
                serverInfo.dwReserved1 = 0;
                serverInfo.dwReserved2 = 0;

                return serverInfo;
            }

            /// <summary>
            ///     Deallocated memory allocated when the COSERVERINFO structure was created.
            /// </summary>
            public void Deallocate()
            {
                if (m_hUserName.IsAllocated) m_hUserName.Free();
                if (m_hPassword.IsAllocated) m_hPassword.Free();
                if (m_hDomain.IsAllocated) m_hDomain.Free();
                if (m_hIdentity.IsAllocated) m_hIdentity.Free();
                if (m_hAuthInfo.IsAllocated) m_hAuthInfo.Free();
            }

            #endregion

            #region Private Members

            private GCHandle m_hAuthInfo;
            private GCHandle m_hDomain;
            private GCHandle m_hIdentity;
            private GCHandle m_hPassword;
            private GCHandle m_hUserName;

            #endregion
        }

        #endregion
    }
}