using System;
using System.Collections.Generic;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcAsyncIO3 : ComWrapper
    {
        public OpcAsyncIO3(object comObject, object userData): base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCAsyncIO3>",
                () => comObject.QueryInterface<IOPCAsyncIO3>());
        }

        private IOPCAsyncIO3 ComObject { get; set; }

        public bool Enable
        {
            get
            {
                return DoComCall(ComObject, "IOpcAsyncIO3::GetEnable", () =>
                {
                    bool enable;
                    ComObject.GetEnable(out enable);
                    return enable;
                });
            }
            set { DoComCall(ComObject, "IOpcAsyncIO3::SetEnable", () => ComObject.SetEnable(value), value); }
        }

        public int Read(int[] serverHandles, int transactionId, out HRESULT[] errors)
        {
            var _errors = new HRESULT[serverHandles.Length];
            int result = DoComCall(ComObject, "IOpcAsyncIO3::Read", () =>
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
            int result = DoComCall(ComObject, "IOpcAsyncIO3::Write", () =>
            {
                int dwCancelId;
                ComObject.Write(serverHandles.Length, serverHandles, values, transactionId, out dwCancelId, out _errors);
                return dwCancelId;
            }, serverHandles, values, transactionId);
            errors = _errors;
            return result;
        }

        public int Refresh2(OPCDATASOURCE dataSource, int transactionId)
        {
            return DoComCall(ComObject, "IOpcAsyncIO3::Refresh2", () =>
            {
                int dwCancelId;
                ComObject.Refresh2(dataSource, transactionId, out dwCancelId);
                return dwCancelId;
            }, dataSource, transactionId);
        }

        public void Cancel2(int cancelId)
        {
            DoComCall(ComObject, "IOpcAsyncIO3::Cancel2", () => ComObject.Cancel2(cancelId), cancelId);
        }

        public int RefreshMaxAge(TimeSpan maxAge, int transactionId)
        {
            return DoComCall(ComObject, "IOpcAsyncIO3::RefreshMaxAge", () =>
            {
                int dwCancelId;
                var intMaxAge = (int) maxAge.TotalMilliseconds;
                ComObject.RefreshMaxAge(intMaxAge, transactionId, out dwCancelId);
                return dwCancelId;
            }, maxAge, transactionId);
        }

        public int ReadMaxAge(int[] serverHandles, IList<TimeSpan> maxAge, int transactionId, out HRESULT[] errors)
        {
            int[] intMaxAge = ArrayHelpers.CreateMaxAgeArray(maxAge, maxAge.Count);
            var _errors = new HRESULT[serverHandles.Length];
            int result = DoComCall(ComObject, "IOpcAsyncIO3::ReadMaxAge", () =>
            {
                int dwCancelId;
                ComObject.ReadMaxAge(serverHandles.Length, serverHandles, intMaxAge, transactionId, out dwCancelId,
                    out _errors);
                return dwCancelId;
            }, serverHandles, maxAge, transactionId);
            errors = _errors;
            return result;
        }

        public int WriteVQT(int[] serverHandles, OPCITEMVQT[] vqts, int transactionId, out HRESULT[] errors)
        {
            var _errors = new HRESULT[serverHandles.Length];
            int result = DoComCall(ComObject, "IOpcAsyncIO3::WriteVQT", () =>
            {
                int dwCancelId;
                ComObject.WriteVQT(serverHandles.Length, serverHandles, vqts, transactionId, out dwCancelId, out _errors);
                return dwCancelId;
            }, serverHandles, vqts, transactionId);
            errors = _errors;
            return result;
        }
    }
}