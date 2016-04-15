using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Guid("9DD0B56C-AD9E-43EE-8305-487F3188BF7A")]
    [InterfaceType((short)1)]
    [ComImport]
    interface IOPCServerList2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumClassesOfCategories([In] int cImplemented, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] Guid[] rgcatidImpl, [In] int cRequired, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.Struct), In] Guid[] rgcatidReq, [MarshalAs(UnmanagedType.Interface)] out IOPCEnumGUID ppenumClsid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetClassDetails([In] ref Guid clsid, [MarshalAs(UnmanagedType.LPWStr)] out string ppszProgID, [MarshalAs(UnmanagedType.LPWStr)] out string ppszUserType, [MarshalAs(UnmanagedType.LPWStr)] out string ppszVerIndProgID);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr), In] string szProgId, out Guid clsid);
    }
}
