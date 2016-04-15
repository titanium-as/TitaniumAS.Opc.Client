using System;
using System.Threading;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Wrappers;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal class RefreshMaxAgeAsyncRequest : IAsyncRequest
    {
        private readonly OpcAsyncIO3 _asyncIO3;
        private readonly TaskCompletionSource<OpcDaItemValue[]> _tcs = new TaskCompletionSource<OpcDaItemValue[]>();
        private AsyncRequestManager _requestManager;

        public RefreshMaxAgeAsyncRequest(OpcAsyncIO3 asyncIO3)
        {
            _asyncIO3 = asyncIO3;
        }

        public int CancellationId { get; private set; }

        public Task<OpcDaItemValue[]> Task
        {
            get { return _tcs.Task; }
        }

        public void OnAdded(AsyncRequestManager requestManager, int transactionId)
        {
            _requestManager = requestManager;
            TransactionId = transactionId;
        }

        public void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            throw new NotSupportedException();
        }

        public void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors)
        {
            throw new NotSupportedException();
        }

        public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            _tcs.TrySetResult(values);
        }

        public void Cancel()
        {
            if (!Task.IsCompleted)
                _asyncIO3.Cancel2(CancellationId);
        }

        public int TransactionId { get; set; }

        public void OnCancel(int dwTransid, int hGroup)
        {
            _tcs.TrySetCanceled();
        }

        public Task<OpcDaItemValue[]> Start(TimeSpan maxAge, CancellationToken token)
        {
            try
            {
                int cancelId = _asyncIO3.RefreshMaxAge(maxAge, TransactionId);
                CancellationId = cancelId;
                RequestHelpers.SetCancellationHandler(token, Cancel);

                return Task;
            }
            catch (Exception ex)
            {
                _requestManager.CompleteRequest(TransactionId);
                _tcs.SetException(ex);
                return Task;
            }
        }
    }
}