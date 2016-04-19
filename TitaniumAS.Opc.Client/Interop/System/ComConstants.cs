namespace TitaniumAS.Opc.Client.Interop.System
{
    internal class ComConstants
    {
        public const uint CLSCTX_INPROC_SERVER = 0x1;
        public const uint CLSCTX_INPROC_HANDLER = 0x2;
        public const uint CLSCTX_LOCAL_SERVER = 0x4;
        public const uint CLSCTX_REMOTE_SERVER = 0x10;
        public const uint SEC_WINNT_AUTH_IDENTITY_ANSI = 0x1;
        public const uint SEC_WINNT_AUTH_IDENTITY_UNICODE = 0x2;
        public const uint RPC_C_AUTHN_NONE = 0;
        public const uint RPC_C_AUTHN_DCE_PRIVATE = 1;
        public const uint RPC_C_AUTHN_DCE_PUBLIC = 2;
        public const uint RPC_C_AUTHN_DEC_PUBLIC = 4;
        public const uint RPC_C_AUTHN_GSS_NEGOTIATE = 9;
        public const uint RPC_C_AUTHN_WINNT = 10;
        public const uint RPC_C_AUTHN_GSS_SCHANNEL = 14;
        public const uint RPC_C_AUTHN_GSS_KERBEROS = 16;
        public const uint RPC_C_AUTHN_DPA = 17;
        public const uint RPC_C_AUTHN_MSN = 18;
        public const uint RPC_C_AUTHN_DIGEST = 21;
        public const uint RPC_C_AUTHN_MQ = 100;
        public const uint RPC_C_AUTHN_DEFAULT = 0xFFFFFFFF;
        public const uint RPC_C_AUTHZ_NONE = 0;
        public const uint RPC_C_AUTHZ_NAME = 1;
        public const uint RPC_C_AUTHZ_DCE = 2;
        public const uint RPC_C_AUTHZ_DEFAULT = 0xffffffff;
        public const uint RPC_C_AUTHN_LEVEL_DEFAULT = 0;
        public const uint RPC_C_AUTHN_LEVEL_NONE = 1;
        public const uint RPC_C_AUTHN_LEVEL_CONNECT = 2;
        public const uint RPC_C_AUTHN_LEVEL_CALL = 3;
        public const uint RPC_C_AUTHN_LEVEL_PKT = 4;
        public const uint RPC_C_AUTHN_LEVEL_PKT_INTEGRITY = 5;
        public const uint RPC_C_AUTHN_LEVEL_PKT_PRIVACY = 6;
        public const uint RPC_C_IMP_LEVEL_ANONYMOUS = 1;
        public const uint RPC_C_IMP_LEVEL_IDENTIFY = 2;
        public const uint RPC_C_IMP_LEVEL_IMPERSONATE = 3;
        public const uint RPC_C_IMP_LEVEL_DELEGATE = 4;
        public const uint EOAC_NONE = 0x00;
        public const uint EOAC_MUTUAL_AUTH = 0x01;
        public const uint EOAC_CLOAKING = 0x10;
        public const uint EOAC_SECURE_REFS = 0x02;
        public const uint EOAC_ACCESS_CONTROL = 0x04;
        public const uint EOAC_APPID = 0x08;
        public const uint EOAC_DEFAULT = 0x80;
    }
}
