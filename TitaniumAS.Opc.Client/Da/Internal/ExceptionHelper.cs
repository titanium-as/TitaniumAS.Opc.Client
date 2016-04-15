using System;

namespace TitaniumAS.Opc.Client.Da.Internal
{
    internal static class ExceptionHelper
    {
        public static NotSupportedException NotSupportedDa2x()
        {
            return new NotSupportedException("OPC DA 2.x not supported by the server.");
        }

        public static NotSupportedException NotSupportedDa3x()
        {
            return new NotSupportedException("OPC DA 3.x not supported by the server.");
        }
        
        public static NotSupportedException NotSupportedAddressSpaceBrowser()
        {
            return new NotSupportedException("IOPCBrowseServerAddressSpace not supported by the server.");
        }

        public static NotSupportedException NotSupportedItemProperties()
        {
            return new NotSupportedException("IOPCItemProperties not supported by the server.");
        }

        public static NotSupportedException NotSupportedBrowser()
        {
            return new NotSupportedException("IOPCBrowse not supported by the server.");
        }

        public static InvalidOperationException NotConnected()
        {
            return new InvalidOperationException("Not connected to opc da server.");
        }
    }
}