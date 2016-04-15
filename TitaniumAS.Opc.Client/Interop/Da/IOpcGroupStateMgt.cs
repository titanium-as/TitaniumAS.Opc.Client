using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("39c13a50-011e-11d0-9675-0020afd8adb3")]
    internal interface IOPCGroupStateMgt
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

        void SetState(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pRequestedUpdateRate,
            out int pRevisedUpdateRate,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.Bool), In] bool[] pActive,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pTimeBias,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] float[] pPercentDeadband,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] pLCID,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 1), In] int[] phClientGroup);

        void SetName([MarshalAs(UnmanagedType.LPWStr), In] string szName);

        void CloneGroup([MarshalAs(UnmanagedType.LPWStr), In] string szName, [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }
}