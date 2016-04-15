using System;
using System.Threading;
using Common.Logging;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal static class RequestHelpers
    {
        public static void SetCancellationHandler(CancellationToken token, Action callback)
        {
            token.Register(
                () =>
                {
                    try
                    {
                        callback();
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger<IAsyncRequest>().Error("Cancel failed.", ex);
                    }
                }
                );
        }
    }
}