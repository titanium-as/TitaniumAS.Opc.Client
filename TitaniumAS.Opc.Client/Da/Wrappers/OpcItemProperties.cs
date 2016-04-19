using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcItemProperties : ComWrapper
    {
        public OpcItemProperties(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCItemProperties>",
                () => comObject.QueryInterface<IOPCItemProperties>());
        }

        internal IOPCItemProperties ComObject { get; set; }

        public int[] QueryAvailableProperties(string itemId, out string[] descriptions, out VarEnum[] dataTypes)
        {
            int dwCount = 0;
            int[] pdwPropertyIDs = {};
            int[] ppvtDataTypes = {};
            string[] _descriptions = {};
            DoComCall(ComObject, "IOpcItemProperties::QueryAvailableProperties",() =>
                    ComObject.QueryAvailableProperties(itemId, out dwCount, out pdwPropertyIDs, out _descriptions,
                        out ppvtDataTypes), itemId);
            dataTypes = new VarEnum[ppvtDataTypes.Length];
            for (int i = 0; i < dwCount; i++)
            {
                dataTypes[i] = (VarEnum) ppvtDataTypes[i];
            }
            descriptions = _descriptions;
            return pdwPropertyIDs;
        }

        public object[] GetItemProperties(string itemId, int[] propertyIds, out HRESULT[] errors)
        {
            HRESULT[] _errors = {};
            object[] results = DoComCall(ComObject, "IOpcItemProperties::GetItemProperties", () =>
            {
                object[] ppvData;
                ComObject.GetItemProperties(itemId, propertyIds.Length, propertyIds, out ppvData, out _errors);
                return ppvData;
            }, itemId, propertyIds.Length, propertyIds);
            errors = _errors;
            return results;
        }

        public string[] LookupItemIDs(string itemId, int[] propertyIds, out HRESULT[] ppErrors)
        {
            string[] ppszNewItemIDs = {};
            HRESULT[] _ppErrors = {};
            DoComCall(ComObject,"IOpcItemProperties::LookupItemIds", () =>
                    ComObject.LookupItemIDs(itemId, propertyIds.Length, propertyIds, out ppszNewItemIDs, out _ppErrors), itemId, propertyIds);
            ppErrors = _ppErrors;
            return ppszNewItemIDs;
        }
    }
}