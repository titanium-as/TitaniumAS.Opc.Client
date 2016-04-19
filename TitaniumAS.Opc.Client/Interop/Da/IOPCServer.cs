using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Guid("39c13a4d-011e-11d0-9675-0020afd8adb3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [ComImport]
    internal interface IOPCServer
    {
        void AddGroup([MarshalAs(UnmanagedType.LPWStr), In] string szName,
            [MarshalAs(UnmanagedType.Bool), In] bool bActive, [In] int dwRequestedUpdateRate, [In] int hClientGroup,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pTimeBias,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] float[] pPercentDeadband,
            [In] int dwLCID,
            out int phServerGroup,
            out int pRevisedUpdateRate,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        void GetErrorString([In] int dwError, [In] int dwLocale, [MarshalAs(UnmanagedType.LPWStr)] out string ppString);

        void GetGroupByName([MarshalAs(UnmanagedType.LPWStr), In] string szName, [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        //void GetStatus([MarshalAs(UnmanagedType.LPStruct), Out]out OPCSERVERSTATUS ppServerStatus);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStatus(out IntPtr ppServerStatus);

        void RemoveGroup([In] int hServerGroup, [MarshalAs(UnmanagedType.Bool), In] bool bForce);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int CreateGroupEnumerator([In] int dwScope, [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }
}