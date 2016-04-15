using System;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [Serializable]
    public enum RpcDwCapabilities
    {
        EOAC_NONE = 0,
        EOAC_MUTUAL_AUTH = 0x1,
        EOAC_STATIC_CLOAKING = 0x20,
        EOAC_DYNAMIC_CLOAKING = 0x40,
        EOAC_ANY_AUTHORITY = 0x80,
        EOAC_MAKE_FULLSIC = 0x100,
        EOAC_DEFAULT = 0x800,
        EOAC_SECURE_REFS = 0x2,
        EOAC_ACCESS_CONTROL = 0x4,
        EOAC_APPID = 0x8,
        EOAC_DYNAMIC = 0x10,
        EOAC_REQUIRE_FULLSIC = 0x200,
        EOAC_AUTO_IMPERSONATE = 0x400,
        EOAC_NO_CUSTOM_MARSHAL = 0x2000,
        EOAC_DISABLE_AAA = 0x1000
    }
}
