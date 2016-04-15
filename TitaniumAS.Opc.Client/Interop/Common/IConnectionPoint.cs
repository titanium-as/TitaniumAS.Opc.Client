using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(1)]
    [ComImport]
    internal interface IConnectionPoint
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetConnectionInterface(out Guid pIID);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetConnectionPointContainer([MarshalAs(UnmanagedType.Interface)] out IConnectionPointContainer ppCPC);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([MarshalAs(UnmanagedType.IUnknown), In] object pUnkSink, out int pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] int dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumConnections([MarshalAs(UnmanagedType.Interface)] out IEnumConnections ppenum);
    }
}