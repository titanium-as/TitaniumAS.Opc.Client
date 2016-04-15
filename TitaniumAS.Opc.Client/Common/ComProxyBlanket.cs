using TitaniumAS.Opc.Client.Interop.Common;

namespace TitaniumAS.Opc.Client.Common
{
    public class ComProxyBlanket
    {
        public RpcAuthnLevel RpcAuthnLevel { get; set; }
        public RpcImpLevel RpcImpLevel { get; set; }
        public RpcAuthService RpcAuthService { get; set; }
        public RpcAuthType RpcAuthType { get; set; }
        public RpcDwCapabilities DwCapabilities { get; set; }

        public static ComProxyBlanket Default { get; set; }

        public static ComProxyBlanket CreateDefaults()
        {
            return new ComProxyBlanket
            {
                RpcAuthnLevel = RpcAuthnLevel.Default,
                RpcImpLevel = RpcImpLevel.Identify,
                RpcAuthService = RpcAuthService.RPC_C_AUTHN_DEFAULT,
                RpcAuthType = RpcAuthType.RPC_C_AUTHZ_DEFAULT,
                DwCapabilities = RpcDwCapabilities.EOAC_NONE
            };
        }

        public static ComProxyBlanket CreateNoSecurity()
        {
            return new ComProxyBlanket
            {
                RpcAuthnLevel = RpcAuthnLevel.None,
                RpcImpLevel = RpcImpLevel.Identify,
                RpcAuthService = RpcAuthService.RPC_C_AUTHN_NONE,
                RpcAuthType = RpcAuthType.RPC_C_AUTHZ_NONE,
                DwCapabilities = RpcDwCapabilities.EOAC_NONE
            };
        }

        public static  ComProxyBlanket CreateRecommended()
        {
            return new ComProxyBlanket
            {
                RpcAuthnLevel = RpcAuthnLevel.Connect,
                RpcImpLevel = RpcImpLevel.Identify,
                RpcAuthService = RpcAuthService.RPC_C_AUTHN_DEFAULT,
                RpcAuthType = RpcAuthType.RPC_C_AUTHZ_DEFAULT,
                DwCapabilities = RpcDwCapabilities.EOAC_NONE
            };
        }
    }
}