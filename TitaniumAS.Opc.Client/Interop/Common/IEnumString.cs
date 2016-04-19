using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [InterfaceType(1)]
    [Guid("00000101-0000-0000-C000-000000000046")]
    [ComImport]
    internal interface IEnumString
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteNext([In] int celt,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr), In, Out] string[] rgelt,
            out int pceltFetched);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Skip([In] int celt);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Reset();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumString ppenum);
    }
}