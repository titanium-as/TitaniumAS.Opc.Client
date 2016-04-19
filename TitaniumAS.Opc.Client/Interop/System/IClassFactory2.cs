using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [ComImport]
    [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IClassFactory2
    {
        void CreateInstance(
            [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] [Out] out object ppvObject);

        void LockServer(
            [MarshalAs(UnmanagedType.Bool)] bool fLock);

        void GetLicInfo(
            [In, Out] ref LICINFO pLicInfo);

        void RequestLicKey(
            int dwReserved,
            [MarshalAs(UnmanagedType.BStr)] string pbstrKey);

        void CreateInstanceLic(
            [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
            [MarshalAs(UnmanagedType.IUnknown)] object punkReserved,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.BStr)] string bstrKey,
            [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppvObject);
    }
}