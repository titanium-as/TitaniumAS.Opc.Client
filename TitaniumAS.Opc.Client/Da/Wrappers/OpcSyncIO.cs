using System;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcSyncIO : ComWrapper
    {
        public OpcSyncIO(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCSyncIO>",
                () => comObject.QueryInterface<IOPCSyncIO>());
        }

        private IOPCSyncIO ComObject { get; set; }

        public OPCITEMSTATE[] Read(OPCDATASOURCE dataSource, int[] serverHandles, out HRESULT[] errors)
        {
            HRESULT[] _errors = {};
            OPCITEMSTATE[] results = DoComCall(ComObject, "IOpcSyncIO::Read", () =>
            {
                OPCITEMSTATE[] ppItemValues;
                ComObject.Read(dataSource, serverHandles.Length, serverHandles, out ppItemValues, out _errors);
                return ppItemValues;
            }, dataSource, serverHandles.Length, serverHandles);
            errors = _errors;
            return results;
        }

        public HRESULT[] Write(int[] serverHandles, object[] values)
        {
            return DoComCall(ComObject, "IOpcSyncIO::Write", () =>
            {
                HRESULT[] ppErrors;
                ComObject.Write(serverHandles.Length, serverHandles, values, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, values);
        }
    }
}