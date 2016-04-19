using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [ComImport]
    [Guid("0000013D-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IClientSecurity
    {
        void QueryBlanket(
            [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
            ref uint pAuthnSvc,
            ref uint pAuthzSvc,
            [MarshalAs(UnmanagedType.LPWStr)] ref string pServerPrincName,
            ref uint pAuthnLevel,
            ref uint pImpLevel,
            ref IntPtr pAuthInfo,
            ref uint pCapabilities);

        void SetBlanket(
            [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
            uint pAuthnSvc,
            uint pAuthzSvc,
            [MarshalAs(UnmanagedType.LPWStr)] string pServerPrincName,
            uint pAuthnLevel,
            uint pImpLevel,
            IntPtr pAuthInfo,
            uint pCapabilities);

        void CopyProxy(
            [MarshalAs(UnmanagedType.IUnknown)] object pProxy,
            [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppCopy);
    }
}