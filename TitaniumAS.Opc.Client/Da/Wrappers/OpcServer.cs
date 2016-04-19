using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcServer : ComWrapper
    {
        public OpcServer(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCServer>",
                () => comObject.QueryInterface<IOPCServer>());
        }

        internal IOPCServer ComObject { get; set; }

        public object AddGroup(string name, bool active, TimeSpan requestedUpdateRate, int clientHandle,
            TimeSpan? timeBias, float? percentDeadband, int lcid, out int serverHandle, out TimeSpan revisedUpdateRate)
        {
            int pRevisedUpdateRate = 0;
            Guid interfaceGuid = typeof (IOPCItemMgt).GUID;
            object opcDaGroup = null;
            int[] pTimeBias = null;
            if (timeBias.HasValue)
            {
                pTimeBias = new[] {(int) timeBias.Value.TotalMinutes};
            }
            float[] pPercentDeadband = null;
            if (percentDeadband.HasValue)
            {
                pPercentDeadband = new[] {percentDeadband.Value};
            }
            int _serverHandle = 0;
            DoComCall(ComObject,
                "IOpcServer::AdGroup", () => ComObject.AddGroup(name, active, (int) requestedUpdateRate.TotalMilliseconds, clientHandle,
                    pTimeBias, pPercentDeadband, lcid, out _serverHandle, out pRevisedUpdateRate, ref interfaceGuid,
                    out opcDaGroup), name, active, requestedUpdateRate.TotalMilliseconds, clientHandle,
                pTimeBias, pPercentDeadband, lcid);
            serverHandle = _serverHandle;
            revisedUpdateRate = TimeSpan.FromMilliseconds(pRevisedUpdateRate);
            return opcDaGroup;
        }

        public string GetErrorString(int error, int locale)
        {
            return DoComCall(ComObject, "IOpcServer::GetErrorString", () =>
            {
                string str;
                ComObject.GetErrorString(error, locale, out str);
                return str;
            }, error, locale);
        }

        public object GetGroupByName(string name)
        {
            return DoComCall(ComObject, "IOpcServer::GetGroupName", () =>
            {
                Guid interfaceGuid = typeof (IOPCItemMgt).GUID;
                object ppUnk;
                ComObject.GetGroupByName(name, ref interfaceGuid, out ppUnk);
                return ppUnk;
            }, name);
        }

        public OPCSERVERSTATUS GetStatus()
        {
            return DoComCall(ComObject, "IOpcServer::GetStatus", () =>
            {
                IntPtr serverStatus;
                ComObject.GetStatus(out serverStatus);
                var ss = (OPCSERVERSTATUS) Marshal.PtrToStructure(serverStatus, typeof (OPCSERVERSTATUS));
                Marshal.DestroyStructure(serverStatus, typeof (OPCSERVERSTATUS));
                Marshal.FreeCoTaskMem(serverStatus);
                return ss;
            });
        }

        public void RemoveGroup(int hServerGroup, bool bForce)
        {
            DoComCall(ComObject, "IOpcServer::RemoveGroup", () => ComObject.RemoveGroup(hServerGroup, bForce),
                hServerGroup, bForce);
        }

        public object CreateGroupEnumerator(int scope)
        {
            return DoComCall(ComObject, "IOpcServer::CreateGroupEnumerator", () =>
            {
                Guid iid = typeof (IEnumUnknown).GUID;
                object ppUnk;
                ComObject.CreateGroupEnumerator(scope, ref iid, out ppUnk);
                return ppUnk;
            }, scope);
        }
    }
}