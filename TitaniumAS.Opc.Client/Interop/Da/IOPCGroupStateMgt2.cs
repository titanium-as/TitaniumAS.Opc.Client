using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [Guid("8E368666-D72E-4F78-87ED-647611C61C9F")]
    [ComImport]
    internal interface IOPCGroupStateMgt2 : IOPCGroupStateMgt
    {
        void GetState(
            out int pUpdateRate,
            [MarshalAs(UnmanagedType.Bool)] out bool pActive,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppName,
            out int pTimeBias,
            out float pPercentDeadband,
            out int pLCID,
            out int phClientGroup,
            out int phServerGroup);

        void SetState([MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pRequestedUpdateRate,
            out uint pRevisedUpdateRate,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.Bool), In] bool[] pActive,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pTimeBias,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] float[] pPercentDeadband,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pLCID,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] phClientGroup);

        void SetName([MarshalAs(UnmanagedType.LPWStr), In] string szName);

        void CloneGroup([MarshalAs(UnmanagedType.LPWStr), In] string szName, [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetKeepAlive([In] int dwKeepAliveTime, out int pdwRevisedKeepAliveTime);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetKeepAlive(out int pdwKeepAliveTime);
    }
}