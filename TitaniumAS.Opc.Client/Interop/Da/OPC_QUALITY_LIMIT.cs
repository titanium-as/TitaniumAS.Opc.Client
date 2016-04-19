using System;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Flags]
    internal enum OPC_QUALITY_LIMIT
    {
        LIMIT_OK = 0,
        LIMIT_LOW = 1,
        LIMIT_HIGH = 2,
        LIMIT_CONST = LIMIT_HIGH | LIMIT_LOW
    }
}