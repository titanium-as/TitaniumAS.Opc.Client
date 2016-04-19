namespace TitaniumAS.Opc.Client.Common.Internal
{
    internal interface IOpcServerShutdownHandler
    {
        void HandleShutdown(string reason);
    }
}