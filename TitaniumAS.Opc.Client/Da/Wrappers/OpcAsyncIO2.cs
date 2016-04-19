using System;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcAsyncIO2 : ComWrapper
    {
        public OpcAsyncIO2(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOpcAsyncIO2>",
                () => comObject.QueryInterface<IOPCAsyncIO2>());
        }

        private IOPCAsyncIO2 ComObject { get; set; }

        public bool Enable
        {
            get
            {
                return DoComCall(ComObject, "IOpcAsyncIO2::GetEnable", () =>
                {
                    bool enable;
                    ComObject.GetEnable(out enable);
                    return enable;
                });
            }
            set { DoComCall(ComObject, "IOpcAsyncIO2::SetEnable", () => ComObject.SetEnable(value), value); }
        }

        public int Read(int[] serverHandles, int transactionId, out HRESULT[] errors)
        {
            var _errors = new HRESULT[serverHandles.Length];
            int result = DoComCall(ComObject, "IOpcAsyncIO2::Read", () =>
            {
                int dwCancelId;
                ComObject.Read(serverHandles.Length, serverHandles, transactionId, out dwCancelId, out _errors);
                return dwCancelId;
            }, serverHandles, transactionId);
            errors = _errors;
            return result;
        }

        public int Write(int[] serverHandles, object[] values, int transactionId, out HRESULT[] errors)
        {
            var _errors = new HRESULT[serverHandles.Length];
            int result = DoComCall(ComObject, "IOpcAsyncIO2::Write", () =>
            {
                int dwCancelId;
                ComObject.Write(serverHandles.Length, serverHandles, values, transactionId, out dwCancelId, out _errors);
                return dwCancelId;
            }, serverHandles, values, transactionId);
            errors = _errors;
            return result;
        }

        public int Refresh2(OPCDATASOURCE dataSource, int transactionID)
        {
            return DoComCall(ComObject, "IOpcAsyncIO2::Refresh2", () =>
            {
                int dwCancelId;
                ComObject.Refresh2(dataSource, transactionID, out dwCancelId);
                return dwCancelId;
            }, dataSource, transactionID);
        }

        public void Cancel2(int cancelId)
        {
            DoComCall(ComObject, "IOpcAsyncIO2::Cancel2", () => ComObject.Cancel2(cancelId), cancelId);
        }
    }
}