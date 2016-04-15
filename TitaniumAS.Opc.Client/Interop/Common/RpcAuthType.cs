using System;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Serializable]
    public enum RpcAuthType : uint
    {
        RPC_C_AUTHZ_NONE = 0,                   //no authorization
        RPC_C_AUTHZ_NAME = 1,                   //auth by principal name
        RPC_C_AUTHZ_DCE = 2,                    //auth by DCE 
        RPC_C_AUTHZ_DEFAULT = 0xffffffff        //auth by security blanket authentification
    }
}
