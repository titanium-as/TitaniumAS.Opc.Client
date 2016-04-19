using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcItemIO : ComWrapper
    {
        public OpcItemIO(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCItemIO>",
                () => comObject.QueryInterface<IOPCItemIO>());
        }

        private IOPCItemIO ComObject { get; set; }

        public OpcDaVQTE[] Read(IList<string> itemIds, IList<TimeSpan> maxAge)
        {
            if (maxAge == null)
                maxAge = new TimeSpan[itemIds.Count];
            if (itemIds.Count != maxAge.Count)
                throw new ArgumentException("Invalid size of maxAge", "maxAge");

            int[] intMaxAge = ArrayHelpers.CreateMaxAgeArray(maxAge, itemIds.Count);

            string[] pszItemIDs = itemIds.ToArray();
            var ppvValues = new object[pszItemIDs.Length];
            var ppwQualities = new short[pszItemIDs.Length];
            var ppftTimeStamps = new FILETIME[pszItemIDs.Length];
            var ppErrors = new HRESULT[pszItemIDs.Length];
            DoComCall(ComObject, "IOPCItemIO::Read", () =>
                    ComObject.Read(pszItemIDs.Length, pszItemIDs, intMaxAge, out ppvValues, out ppwQualities,
                        out ppftTimeStamps, out ppErrors), pszItemIDs.Length, pszItemIDs,
                maxAge);

            var result = new OpcDaVQTE[itemIds.Count];
            for (int i = 0; i < ppvValues.Length; i++)
            {
                var vqte = new OpcDaVQTE
                {
                    Value = ppvValues[i],
                    Quality = ppwQualities[i],
                    Timestamp = FileTimeConverter.FromFileTime(ppftTimeStamps[i]),
                    Error = ppErrors[i]
                };
                result[i] = vqte;
            }
            return result;
        }

        public HRESULT[] WriteVQT(IList<string> itemIds, IList<OpcDaVQT> values)
        {
            if (itemIds.Count != values.Count)
                throw new ArgumentException("Invalid size of values", "values");

            var vqts = new OPCITEMVQT[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                OpcDaVQT opcItemValue = values[i];
                vqts[i].bQualitySpecified = false;
                if (opcItemValue.Quality != short.MinValue)
                {
                    vqts[i].bQualitySpecified = true;
                    vqts[i].wQuality = opcItemValue.Quality;
                }

                vqts[i].bTimeStampSpecified = false;
                if (opcItemValue.Timestamp != DateTimeOffset.MinValue)
                {
                    vqts[i].bTimeStampSpecified = true;
                    vqts[i].ftTimeStamp = FileTimeConverter.ToFileTime(opcItemValue.Timestamp);
                }

                vqts[i].vDataValue = opcItemValue.Value;
            }

            string[] pszItemIDs = itemIds.ToArray();
            var ppErrors = new HRESULT[pszItemIDs.Length];
            DoComCall(ComObject, "IOPCItemIO::WriteVQT",
                () => ComObject.WriteVQT(pszItemIDs.Length, pszItemIDs, vqts, out ppErrors), pszItemIDs.Length, pszItemIDs);
            return ppErrors;
        }
    }
}