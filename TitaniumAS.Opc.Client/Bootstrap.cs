using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client
{
    /// <summary>
    /// Bootstrapping class of the OPC DA client library.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Initializes the OPC DA client library. It should run under MTA apartment state due to CoInitializeSecurity call.
        /// See http://www.pinvoke.net/default.aspx/ole32/CoInitializeSecurity.html for details.
        ///
        /// When you initialize the library in a UI application, an STAThreadAttribute should be removed from the program entry point.
        /// You can use workaround like the following:
        /// 1. Extract a content of a method Main() to RunApplication();
        /// 2. Call Bootstrap.Initialize() first;
        /// 3. Create new thread with STA apartment state to run the application:
        ///   var thread = new Thread(RunApplication);
        ///   thread.SetApartmentState(ApartmentState.STA);
        ///   thread.Start();
        /// </summary>
        public static void Initialize()
        {
            try
            {
                Com.InitializeSecurity();
            }
            catch (ExternalException ex)
            {
                throw new OpcClientException(
                    "Unable to initialize OPC DA client. It should run under MTA apartment state due to CoInitializeSecurity call. " +
                    "See http://www.pinvoke.net/default.aspx/ole32/CoInitializeSecurity.html for details.", ex);
            }
        }
    }
}