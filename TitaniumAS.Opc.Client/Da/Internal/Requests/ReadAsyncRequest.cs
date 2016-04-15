using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Wrappers;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal class ReadAsyncRequest : IAsyncRequest
    {
        private readonly OpcAsyncIO2 _asyncIO2;
        private readonly TaskCompletionSource<OpcDaItemValue[]> _tcs = new TaskCompletionSource<OpcDaItemValue[]>();
        private AsyncRequestManager _requestManager;

        public ReadAsyncRequest(OpcAsyncIO2 asyncIO2)
        {
            _asyncIO2 = asyncIO2;
        }

        public int CancellationId { get; private set; }

        public Task<OpcDaItemValue[]> Task
        {
            get { return _tcs.Task; }
        }

        public void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            _tcs.TrySetResult(values);
        }

        public void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors)
        {
            throw new NotSupportedException();
        }

        public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            throw new NotSupportedException();
        }

        public void Cancel()
        {
            if (!Task.IsCompleted)
                _asyncIO2.Cancel2(CancellationId);
        }

        public void OnAdded(AsyncRequestManager requestManager, int transactionId)
        {
            _requestManager = requestManager;
            TransactionId = transactionId;
        }

        public int TransactionId { get; private set; }

        public void OnCancel(int dwTransid, int hGroup)
        {
            _tcs.TrySetCanceled();
        }

        public Task<OpcDaItemValue[]> Start(IList<OpcDaItem> items, CancellationToken token)
        {
            try
            {
                var serverHandles = ArrayHelpers.GetServerHandles(items);
                
                HRESULT[] ppErrors;
                int cancelId = _asyncIO2.Read(serverHandles, TransactionId, out ppErrors);

                if (ppErrors.All(e => e.Failed)) // if all errors no callback will take place
                {
                    _requestManager.CompleteRequest(TransactionId);
                    var result = new OpcDaItemValue[ppErrors.Length];
                    for (var i = 0; i < result.Length; i++)
                    {
                        result[i] = new OpcDaItemValue {Error = ppErrors[i], Item = items[i]};
                    }
                    _tcs.SetResult(result);
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