using System;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Serializable]
    public enum RpcAuthnLevel
    {
        Default = 0,
        None = 1,
        Connect = 2,
        Call = 3,
        Pkt = 4,
        PktIntegrity = 5,
        PktPrivacy = 6
    }
}
