using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcSyncIO2 : ComWrapper
    {
        public OpcSyncIO2(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCSyncIO2>",
                () => comObject.QueryInterface<IOPCSyncIO2>());
        }

        private IOPCSyncIO2 ComObject { get; set; }

        public OPCITEMSTATE[] Read(OPCDATASOURCE dataSource, int[] serverHandles, out HRESULT[] errors)
        {
            HRESULT[] _errors = {};
            OPCITEMSTATE[] results = DoComCall(ComObject, "IOpcSyncIO2::Read", () =>
            {
                OPCITEMSTATE[] ppItemValues;
                ComObject.Read(dataSource, serverHandles.Length, serverHandles, out ppItemValues,
                    out _errors);
                return ppItemValues;
            }, dataSource, serverHandles.Length, serverHandles);
            errors = _errors;
            return results;
        }

        public HRESULT[] Write(int[] serverHandles, object[] values)
        {
            return DoComCall(ComObject, "IOpcSyncIO2::Write", () =>
            {
                HRESULT[] ppErrors;
                ComObject.Write(serverHandles.Length, serverHandles, values, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, values);
        }

        public object[] ReadMaxAge(int[] serverHandles, IList<TimeSpan> maxAge, out OpcDaQuality[] qualities,
            out DateTimeOffset[] timestamps, out HRESULT[] errors)
        {
            int[] intMaxAge = ArrayHelpers.CreateMaxAgeArray(maxAge, maxAge.Count);
            object[] ppvValues = {};
            short[] ppwQualities = {};
            FILETIME[] ppftTimeStamps = {};
            HRESULT[] _errors = {};
            DoComCall(ComObject, "IOpcSyncIO2::ReadMaxAge", () =>
                    ComObject.ReadMaxAge(serverHandles.Length, serverHandles, intMaxAge, out ppvValues, out ppwQualities,
                        out ppftTimeStamps, out _errors), serverHandles.Length,
                serverHandles, intMaxAge);
            timestamps = new DateTimeOffset[ppftTimeStamps.Length];
            qualities = new OpcDaQuality[ppwQualities.Length];

            for (int i = 0; i < timestamps.Length; i++)
            {
                timestamps[i] = FileTimeConverter.FromFileTime(ppftTimeStamps[i]);
                qualities[i] = new OpcDaQuality(ppwQualities[i]);
            }
            errors = _errors;
            return ppvValues;
        }

        public HRESULT[] WriteVQT(int[] serverHandles, OPCITEMVQT[] vqts)
        {
            return DoComCall(ComObject, "IOpcSyncIO2::WriteVQT", () =>
            {
                HRESULT[] ppErrors;
                ComObject.WriteVQT(serverHandles.Length, serverHandles, vqts, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, vqts);
        }
    }
}