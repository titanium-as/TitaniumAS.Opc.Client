using System;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Flags]
    internal enum OPC_QUALITY_STATUS : short
    {
        BAD = 0,
        CONFIG_ERROR = (short) 4,
        NOT_CONNECTED = (short) 8,
        DEVICE_FAILURE = NOT_CONNECTED | CONFIG_ERROR,
        SENSOR_FAILURE = (short) 16,
        LAST_KNOWN = SENSOR_FAILURE | CONFIG_ERROR,
        COMM_FAILURE = SENSOR_FAILURE | NOT_CONNECTED,
        OUT_OF_SERVICE = COMM_FAILURE | CONFIG_ERROR,
        UNCERTAIN = (short) 64,
        LAST_USABLE = UNCERTAIN | CONFIG_ERROR,
        SENSOR_CAL = UNCERTAIN | SENSOR_FAILURE,
        EGU_EXCEEDED = SENSOR_CAL | CONFIG_ERROR,
        SUB_NORMAL = SENSOR_CAL | NOT_CONNECTED,
        OK = (short) 192,
        LOCAL_OVERRIDE = OK | COMM_FAILURE
    }
}