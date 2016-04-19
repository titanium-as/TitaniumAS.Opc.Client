using System;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Flags]
    internal enum OPC_QUALITY_MASTER : short
    {
        QUALITY_BAD = 0,
        QUALITY_UNCERTAIN = (short) 64,
        ERROR_QUALITY_VALUE = (short) 128,
        QUALITY_GOOD = ERROR_QUALITY_VALUE | QUALITY_UNCERTAIN
    }
}