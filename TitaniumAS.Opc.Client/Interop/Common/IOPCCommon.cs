using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Guid("F31DFDE2-07B6-11D2-B2D8-0060083BA1FB")]
    [InterfaceType(1)]
    [ComImport]
    internal interface IOPCCommon
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetLocaleID([In] int dwLcid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetLocaleID(out int pdwLcid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void QueryAvailableLocaleIDs(out int pdwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), Out] out int[] pdwLcid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetErrorString([In] int dwError, [MarshalAs(UnmanagedType.LPWStr)] out string ppString);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientName([MarshalAs(UnmanagedType.LPWStr), In] string szName);
    }
}