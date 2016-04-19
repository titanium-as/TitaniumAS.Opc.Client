using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [InterfaceType(1)]
    [Guid("F31DFDE1-07B6-11D2-B2D8-0060083BA1FB")]
    [ComImport]
    interface IOPCShutdown
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ShutdownRequest([MarshalAs(UnmanagedType.LPWStr), In] string szReason);
    }
}