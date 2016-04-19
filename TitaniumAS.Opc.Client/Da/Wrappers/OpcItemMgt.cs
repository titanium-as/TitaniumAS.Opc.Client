using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    internal class OpcItemMgt : ComWrapper
    {
        public OpcItemMgt(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCItemMgt>",() => comObject.QueryInterface<IOPCItemMgt>());
        }

        internal IOPCItemMgt ComObject { get; set; }

        public OPCITEMRESULT[] AddItems(
            OPCITEMDEF[] itemDefinitions,
            out HRESULT[] errors)
        {
            var _errors = new HRESULT[itemDefinitions.Length];
            OPCITEMRESULT[] result = DoComCall(ComObject, "IOPCItemMgt::AddItems", () =>
            {
                IntPtr ppAddResults;
                ComObject.AddItems(itemDefinitions.Length, itemDefinitions, out ppAddResults, out _errors);
                OPCITEMRESULT[] opcDaItemResults = ArrayHelpers.CreateOpcItemResults(itemDefinitions, ppAddResults);
                Marshal.FreeCoTaskMem(ppAddResults);
                return opcDaItemResults;
            }, itemDefinitions.Length, itemDefinitions);
            errors = _errors;
            return result;
        }

        public OPCITEMRESULT[] ValidateItems(
            OPCITEMDEF[] itemDefinitions,
            bool blobUpdate,
            out HRESULT[] errors)
        {
            IntPtr ppAddResults;
            var _errors = new HRESULT[itemDefinitions.Length];
            OPCITEMRESULT[] result = DoComCall(ComObject, "IOPCItemMgt::ValidateItems", () =>
            {
                ComObject.ValidateItems(itemDefinitions.Length, itemDefinitions, blobUpdate, out ppAddResults,
                    out _errors);
                OPCITEMRESULT[] opcDaItemResults = ArrayHelpers.CreateOpcItemResults(itemDefinitions, ppAddResults);
                Marshal.FreeCoTaskMem(ppAddResults);
                return opcDaItemResults;
            }, itemDefinitions.Length, itemDefinitions, blobUpdate);
            errors = _errors;
            return result;
        }

        public HRESULT[] RemoveItems(int[] serverHandles)
        {
            return DoComCall(ComObject, "IOPCItemMgt::RemoveItems", () =>
            {
                HRESULT[] ppErrors;
                ComObject.RemoveItems(serverHandles.Length, serverHandles, out ppErrors);
                return ppErrors;
            }, serverHandles);
        }

        public HRESULT[] SetActiveState(int[] serverHandles, bool bActive)
        {
            return DoComCall(ComObject, "IOPCItemMgt::SetActiveState", () =>
            {
                HRESULT[] ppErrors;
                ComObject.SetActiveState(serverHandles.Length, serverHandles, bActive, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, bActive);
        }

        public HRESULT[] SetClientHandles(int[] serverHandles, int[] clientHandles)
        {
            return DoComCall(ComObject, "IOPCItemMgt::SetClientHandles", () =>
            {
                HRESULT[] ppErrors;
                ComObject.SetClientHandles(serverHandles.Length, serverHandles, clientHandles, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, clientHandles);
        }

        public HRESULT[] SetDatatypes(
            int[] serverHandles,
            IList<VarEnum> requestedDatatypes)
        {
            var requestedTypeShorts = new short[requestedDatatypes.Count];
            for (int i = 0; i < requestedDatatypes.Count; i++)
            {
                requestedTypeShorts[i] = (short) requestedDatatypes[i];
            }
            return DoComCall(ComObject, "IOPCItemMgt::SetDataTypes", () =>
            {
                HRESULT[] ppErrors;
                ComObject.SetDatatypes(serverHandles.Length, serverHandles, requestedTypeShorts, out ppErrors);
                return ppErrors;
            }, serverHandles.Length, serverHandles, requestedDatatypes);
        }


        public IEnumOPCItemAttributes CreateEnumerator()
        {
            return DoComCall(ComObject, "IOPCItemMgt::CreateEnumerator", () =>
            {
                Guid iid = typeof (IEnumOPCItemAttributes).GUID;
                object enumObj;
                ComObject.CreateEnumerator(ref iid, out enumObj);
                return (IEnumOPCItemAttributes) enumObj;
            });
        }
    }
}