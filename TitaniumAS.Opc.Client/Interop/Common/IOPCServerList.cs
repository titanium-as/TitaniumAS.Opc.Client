using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [InterfaceType(1)]
    [Guid("13486D50-4821-11D2-A494-3CB306C10000")]
    [ComImport]
    interface IOPCServerList
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumClassesOfCategories([In] int cImplemented,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] Guid[] rgcatidImpl,
            [In] int cRequired,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.Struct), In] Guid[]
                rgcatidReq, [MarshalAs(UnmanagedType.Interface)] out IEnumGUID ppenumClsid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetClassDetails([In] ref Guid clsid, [MarshalAs(UnmanagedType.LPWStr)] out string ppszProgID,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszUserType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr), In] string szProgId, out Guid clsid);
    }
}