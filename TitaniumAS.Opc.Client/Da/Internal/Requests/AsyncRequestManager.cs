using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Common.Internal;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da.Internal.Requests
{
    internal class AsyncRequestManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<AsyncRequestManager>();
        private readonly ConnectionPoint<IOPCDataCallback> _connectionPoint;
        private readonly OpcDaGroup _opcDaGroup;
        private readonly Slots<IAsyncRequest> _slots;
        private bool _disposed;

        public AsyncRequestManager(OpcDaGroup opcDaGroup)
        {
            var opcDataCallback = new OpcDataCallback
            {
                CancelComplete = OnCancelComplete,
                DataChange = OnDataChange,
                ReadComplete = OnReadComplete,
                WriteComplete = OnWriteComplete
            };

            _connectionPoint = new ConnectionPoint<IOPCDataCallback>(opcDataCallback);

            _slots = new Slots<IAsyncRequest>(OpcConfiguration.MaxSimultaneousRequests);
            _opcDaGroup = opcDaGroup;

            TryConnect(opcDaGroup.ComObject);
        }

        public bool IsConnected
        {
            get { return _connectionPoint.IsConnected; }
        }

        public bool HasPendingRequests
        {
            get { return _slots.HasItems; }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnDataChange(int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror,
            int dwCount, int[] phClientItems,
            object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors)
        {
            try
            {
                Log.TraceFormat(
                    "On data change. Transaction id: {0}. Client group handle: {1}. Master quality: {2}. Error: {3}.",
                    dwTransid, hGroup, hrMasterquality, hrMastererror);
                if (hGroup != _opcDaGroup.ClientHandle)
                    throw new ArgumentException("Wrong group handle", "hGroup");

                OpcDaItemValue[] values = OpcDaItemValue.Create(_opcDaGroup, dwCount, phClientItems, pvValues,
                    pwQualities,
                    pftTimeStamps, pErrors);
                if (dwTransid == 0) // Data from subscription
                {
                    OnNewItemValues(values);
                    return;
                }

                IAsyncRequest request = CompleteRequest(dwTransid);
                if (request == null)
                    return;

                if (request.TransactionId != dwTransid)
                    throw new ArgumentException("Wrong transaction id.", "dwTransid");

                request.OnDataChange(dwTransid, hGroup, hrMasterquality, hrMastererror, values);
                OnNewItemValues(values);
            }
            catch (Exception ex)
            {
                Log.Error("Error on data change.", ex);
            }
        }

        private void OnReadComplete(int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror,
            int dwCount, int[] phClientItems,
            object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors)
        {
            try
            {
                Log.TraceFormat(
                    "On read complete. Transaction id: {0}. Client group handle: {1}. Master quality: {2}. Error: {3}.",
                    dwTransid, hGroup, hrMasterquality, hrMastererror);
                if (hGroup != _opcDaGroup.ClientHandle)
                    throw new ArgumentException("Wrong group handle", "hGroup");

                IAsyncRequest request = CompleteRequest(dwTransid);
                if (request == null)
                    return;

                if (request.TransactionId != dwTransid)
                    throw new ArgumentException("Wrong transaction id.", "dwTransid");

                OpcDaItemValue[] values = OpcDaItemValue.Create(_opcDaGroup, dwCount, phClientItems, pvValues,
                    pwQualities,
                    pftTimeStamps, pErrors);
                request.OnReadComplete(dwTransid, hGroup, hrMasterquality, hrMastererror, values);
                OnNewItemValues(values);
            }
            catch (Exception ex)
            {
                Log.Error("Error on read complete.", ex);
            }
        }

        private void OnWriteComplete(int dwTransid, int hGroup, HRESULT hrMastererr, int dwCount,
            int[] pClienthandles, HRESULT[] pErrors)
        {
            try
            {
                Log.TraceFormat("On write complete. Transaction id: {0}. Client group handle: {1}. Error: {2}.",
                    dwTransid, hGroup, hrMastererr);
                IAsyncRequest request = CompleteRequest(dwTransid);
                if (request == null)
                    return;

                if (request.TransactionId != dwTransid)
                    throw new ArgumentException("Wrong transaction id.", "dwTransid");

                request.OnWriteComplete(dwTransid, hGroup, hrMastererr, dwCount, pClienthandles, pErrors);
            }
            catch (Exception ex)
            {
                Log.Error("Error on write complete.", ex);
            }
        }

        private void OnCancelComplete(int dwTransid, int hGroup)
        {
            try
            {
                Log.TraceFormat("On cancel complete. Transaction id: {0}. Client group handle: {1}.", dwTransid, hGroup);
                IAsyncRequest request = CompleteRequest(dwTransid);
                if (request == null)
                    return;

                if (request.TransactionId != dwTransid)
                    throw new ArgumentException("Wrong transaction id.", "dwTransid");

                request.OnCancel(dwTransid, hGroup);
            }
            catch (Exception ex)
            {
                Log.Error("Error on cancel.", ex);
            }
        }

        public event Action<OpcDaItemValue[]> NewItemValues;

        private Task CancellAll(TimeSpan timeout)
        {
            Log.TraceFormat("Cancel all requested.");
            foreach (IAsyncRequest request in _slots.GetSnapshot())
            {
                if (request != null)
                    request.Cancel();
            }

            Task cancellAllTask = Task.Factory.StartNew(() =>
            {
                int attemps = 10;
                for (int i = 0; i < attemps; i++)
                {
                    Thread.Sleep((int) (timeout.TotalMilliseconds/attemps));
                    if (!HasPendingRequests)
                        return;
                }
                throw new TimeoutException(
                    "OPC DA async requests were not cancelled before the specified timeout period expires.");
            });
            cancellAllTask.ContinueWith(t => Log.Trace("All requests canceled."),
                TaskContinuationOptions.OnlyOnRanToCompletion);
            cancellAllTask.ContinueWith(t => Log.Trace("Failed to cancel requests.", t.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
            return cancellAllTask;
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                _slots.Dispose();
            }

            // Free any unmanaged objects here.
            //
            try
            {
                CancellAll(OpcConfiguration.RequestTimeout).Wait();
            }
            catch (Exception ex)
            {
                Log.Error("Cancellation failed.", ex);
            }

            Disconnect();
            _disposed = true;
        }

        ~AsyncRequestManager()
        {
            Dispose(false);
        }

        private void TryConnect(object comServer)
        {
            if (comServer == null)
                return;
            _connectionPoint.TryConnect(comServer);
        }

        private void Disconnect()
        {
            _connectionPoint.Disconnect();
        }

        public void AddRequest(IAsyncRequest request)
        {
            if (!IsConnected)
            {
                throw new Exception("Not connected");
            }
            int slot = _slots.TryAdd(request, OpcConfiguration.RequestTimeout);
            if (slot == -1) // all slots occupied.
            {
                throw new Exception("Exceeded limit of pending requests.");
            }

            request.OnAdded(this, slot + 1);
            // set transaction id to 1-based slot number. 0 is special case for exception based updates.
            Log.TraceFormat("Request added. Transaction id: {0}", request.TransactionId);
        }

        public IAsyncRequest CompleteRequest(int transactionId)
        {
            if (transactionId == 0) // Exception based callback. No pending requests.
                return null;
            IAsyncRequest request = _slots.Remove(transactionId - 1);
            Log.TraceFormat("Request removed. Transaction id: {0}", transactionId);
            return request;
        }

        protected virtual void OnNewItemValues(OpcDaItemValue[] values)
        {
            Action<OpcDaItemValue[]> handler = NewItemValues;
            if (handler != null) handler(values);
        }
    }
}