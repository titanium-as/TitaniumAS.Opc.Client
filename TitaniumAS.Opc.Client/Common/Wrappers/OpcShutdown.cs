using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OpcShutdown : IOPCShutdown
    {
        public void ShutdownRequest(string szReason)
        {
            OnShutdown(new OpcShutdownEventArgs(szReason, HRESULT.S_OK));
        }

        public event EventHandler<OpcShutdownEventArgs> Shutdown;

        protected virtual void OnShutdown(OpcShutdownEventArgs e)
        {
            var handler = Shutdown;
            if (handler != null) handler(this, e);
        }
    }
}