using System;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcItemSamplingMgt : ComWrapper
    {
        public OpcItemSamplingMgt(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCItemSamplingMgt>",
                () => comObject.QueryInterface<IOPCItemSamplingMgt>());
        }

        private IOPCItemSamplingMgt ComObject { get; set; }

        public TimeSpan[] SetItemSamplingRate(
            int[] serverHandles,
            TimeSpan[] requestedSamplingRate,
            out HRESULT[] errors)
        {
            throw new NotImplementedException();
            int[] pdwRequestedSamplingRate = ArrayHelpers.CreateMaxAgeArray(requestedSamplingRate,
                requestedSamplingRate.Length);
            IntPtr ppdwRevisedSamplingRate;
            IntPtr ppErrors;
            ComObject.SetItemSamplingRate(serverHandles.Length, serverHandles, pdwRequestedSamplingRate,
                out ppdwRevisedSamplingRate, out ppErrors);
            return null;
        }

        public TimeSpan[] GetItemSamplingRate(
            int[] serverHandles,
            out HRESULT[] errors)
        {
            throw new NotImplementedException();
        }

        public void ClearItemSamplingRate(
            int[] serverHandles,
            out HRESULT[] errors)
        {
            throw new NotImplementedException();
        }

        public void SetItemBufferEnable(
            int[] serverHandles,
            bool[] enable,
            out HRESULT[] errors)
        {
            throw new NotImplementedException();
        }

        public bool[] GetItemBufferEnable(
            int[] serverHandles,
            out HRESULT[] errors)
        {
            throw new NotImplementedException();
        }
    }
}