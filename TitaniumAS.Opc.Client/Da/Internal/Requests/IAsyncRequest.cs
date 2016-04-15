using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal interface IAsyncRequest
    {
        int TransactionId { get; }
        void OnCancel(int dwTransid, int clientGroupHandle);
        void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, OpcDaItemValue[] values);

        void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors);

        void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, OpcDaItemValue[] values);
        void Cancel();
        void OnAdded(AsyncRequestManager requestManager, int transactionId);
    }
}