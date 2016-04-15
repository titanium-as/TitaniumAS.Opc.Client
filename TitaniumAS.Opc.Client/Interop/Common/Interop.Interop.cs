using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    /// <summary>
    ///     Exposes WIN32 and COM API functions.
    /// </summary>
    public class Interop
    {
        public enum EoAuthnCap
        {
            None = 0x00,
            MutualAuth = 0x01,
            StaticCloaking = 0x20,
            DynamicCloaking = 0x40,
            AnyAuthority = 0x80,
            MakeFullSIC = 0x100,
            Default = 0x800,
            SecureRefs = 0x02,
            AccessControl = 0x04,
            AppID = 0x08,
            Dynamic = 0x10,
            RequireFullSIC = 0x200,
            AutoImpersonate = 0x400,
            NoCustomMarshal = 0x2000,
            DisableAAA = 0x1000
        }

        private const uint LEVEL_SERVER_INFO_100 = 100;
        private const uint LEVEL_SERVER_INFO_101 = 101;
        private const int MAX_PREFERRED_LENGTH = -1;
        private const uint SV_TYPE_WORKSTATION = 0x00000001;
        private const uint SV_TYPE_SERVER = 0x00000002;
        private const int MAX_COMPUTERNAME_LENGTH = 31;

        [DllImport("Netapi32.dll")]
        private static extern int NetServerEnum(
            IntPtr servername,
            uint level,
            out IntPtr bufptr,
            int prefmaxlen,
            out int entriesread,
            out int totalentries,
            uint servertype,
            IntPtr domain,
            IntPtr resume_handle);

        [DllImport("Netapi32.dll")]
        private static extern int NetApiBufferFree(IntPtr buffer);

        /// <summary>
        ///     Enumerates computers on the local network.
        /// </summary>
        public static string[] GetNetworkComputers()
        {
            IntPtr pInfo;

            var entriesRead = 0;
            var totalEntries = 0;

            var result = NetServerEnum(
                IntPtr.Zero,
                LEVEL_SERVER_INFO_100,
                out pInfo,
                MAX_PREFERRED_LENGTH,
                out entriesRead,
                out totalEntries,
                SV_TYPE_WORKSTATION | SV_TYPE_SERVER,
                IntPtr.Zero,
                IntPtr.Zero);

            if (result != 0)
            {
                throw new ApplicationException("NetApi Error = " + string.Format("0x{0,0:X}", result));
            }

            var computers = new string[entriesRead];

            var pos = pInfo;

            for (var ii = 0; ii < entriesRead; ii++)
            {
                var info = (SERVER_INFO_100) Marshal.PtrToStructure(pos, typeof (SERVER_INFO_100));

                computers[ii] = info.sv100_name;

                pos = (IntPtr) (pos.ToInt32() + Marshal.SizeOf(typeof (SERVER_INFO_100)));
            }

            NetApiBufferFree(pInfo);

            return computers;
        }

        [DllImport("Kernel32.dll")]
        private static extern int GetComputerNameW(IntPtr lpBuffer, ref int lpnSize);

        /// <summary>
        ///     Retrieves the name of the local computer.
        /// </summary>
        public static string GetLocalComputer()
        {
            string name = null;
            var size = MAX_COMPUTERNAME_LENGTH + 1;

            var pName = Marshal.AllocCoTaskMem(size*2);

            if (GetComputerNameW(pName, ref size) != 0)
            {
                name = Marshal.PtrToStringUni(pName, size);
            }

            Marshal.FreeCoTaskMem(pName);

            return name;
        }

        [DllImport("ole32.dll")]
        private static extern int CoInitializeSecurity(IntPtr pSecDesc, int cAuthSvc,
            SOLE_AUTHENTICATION_SERVICE[] asAuthSvc, IntPtr pReserved1, uint dwAuthnLevel, uint dwImpLevel,
            IntPtr pAuthList, uint dwCapabilities, IntPtr pReserved3);

        public static void InitializeSecurity()
        {
            var errorCode = CoInitializeSecurity(IntPtr.Zero, -1, null, IntPtr.Zero, 1, 2, IntPtr.Zero, 0, IntPtr.Zero);
            if (errorCode != 0)
            {
                throw new ExternalException(string.Format("CoInitializeSecurity: {0}", errorCode), errorCode);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SERVER_INFO_100
        {
            public readonly uint sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)] public readonly string sv100_name;
        }

        private struct SOLE_AUTHENTICATION_SERVICE
        {
            public int dwAuthnSvc;
            public int dwAuthzSvc;
            public int hr;
            [MarshalAs(UnmanagedType.BStr)] public string pPrincipalName;
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern string ProgIDFromCLSID([In()] ref Guid clsid);

        [DllImport("ole32.dll")]
        internal static extern int CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid pclsid);
    }
}