using System;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Flags]
    internal enum OPCACCESSRIGHTS
    {
        OPC_READABLE = 1,
        OPC_WRITEABLE = 2
    }
}