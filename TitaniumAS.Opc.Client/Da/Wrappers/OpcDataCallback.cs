using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OpcDataCallback : IOPCDataCallback
    {
        public delegate void DataChangeFunc(int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror, int dwCount,
            int[] phClientItems, object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors);

        public delegate void ReadCompleteFunc(
            int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror, int dwCount,
            int[] phClientItems, object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors);


        public delegate void WriteCompleteFunc(int dwTransid, int hGroup, HRESULT hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors);
        public delegate void CancelCompleteFunc(int dwTransid, int hGroup);


        public DataChangeFunc DataChange;
        public ReadCompleteFunc ReadComplete;
        public WriteCompleteFunc WriteComplete;
        public CancelCompleteFunc CancelComplete;

        public void OnDataChange(int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror, int dwCount,
            int[] phClientItems, object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors)
        {
            var f = DataChange;
            if (f != null)
            {
                f(dwTransid, hGroup, hrMasterquality, hrMastererror, dwCount, phClientItems, pvValues, pwQualities,
                    pftTimeStamps, pErrors);
            }
        }

        public void OnReadComplete(int dwTransid, int hGroup, HRESULT hrMasterquality, HRESULT hrMastererror, int dwCount,
            int[] phClientItems, object[] pvValues, short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors)
        {
            var f = ReadComplete;
            if (f != null)
            {
                f(dwTransid, hGroup, hrMasterquality, hrMastererror, dwCount, phClientItems, pvValues, pwQualities,
                    pftTimeStamps, pErrors);
            }

        }

        public void OnWriteComplete(int dwTransid, int hGroup, HRESULT hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors)
        {
            var f = WriteComplete;
            if (f != null)
            {
                f(dwTransid, hGroup, hrMastererr, dwCount, pClienthandles, pErrors);
            }
        }

        public void OnCancelComplete(int dwTransid, int hGroup)
        {
            var f = CancelComplete;
            if (f != null)
            {
                f(dwTransid, hGroup);
            }
        }
    }
}