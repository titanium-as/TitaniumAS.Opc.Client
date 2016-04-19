using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal class WriteVQTAsyncRequest : IAsyncRequest
    {
        private readonly OpcAsyncIO3 _asyncIO3;
        private readonly TaskCompletionSource<HRESULT[]> _tcs = new TaskCompletionSource<HRESULT[]>();
        private AsyncRequestManager _requestManager;

        public WriteVQTAsyncRequest(OpcAsyncIO3 asyncIO3)
        {
            _asyncIO3 = asyncIO3;
        }

        public int CancellationId { get; private set; }

        public Task<HRESULT[]> Task
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
            _tcs.TrySetResult(pErrors);
        }

        public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            throw new NotSupportedException();
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

        public Task<HRESULT[]> Start(IList<OpcDaItem> items, OPCITEMVQT[] vqt, CancellationToken token)
        {
            try
            {
                var serverHandles = ArrayHelpers.GetServerHandles(items);
                HRESULT[] ppErrors;
                int cancelId = _asyncIO3.WriteVQT(serverHandles, vqt, TransactionId, out ppErrors);

                if (ppErrors.All(e => e.Failed)) // if all errors no callback will take place
                {
                    _requestManager.CompleteRequest(TransactionId);
                    _tcs.SetResult(ppErrors);
                }
                else
                {
                    CancellationId = cancelId;
                    RequestHelpers.SetCancellationHandler(token, Cancel);
                }

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