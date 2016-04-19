using System;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    public class OpcServerList : ComWrapper
    {
        public OpcServerList(object comServer)
        {
            if (comServer == null) throw new ArgumentNullException("comServer");

            ComServer = DoComCall(comServer, "IUnknown::QueryInterface<IOPCServerList>",
                () => comServer.QueryInterface<IOPCServerList>());
        }

        internal IOPCServerList ComServer { get; set; }

        public Guid CLSIDFromProgID(string progId)
        {
            return DoComCall(ComServer, "IOpcServerList::CLSIDFromProgID", () =>
            {
                Guid clsid;
                ComServer.CLSIDFromProgID(progId, out clsid);
                return clsid;
            }, progId);
        }

        public string GetClassDetails(Guid clsid, out string userType)
        {
            string _userType = string.Empty;
            string result = DoComCall(ComServer, "IOpcServerList::GetClassDetails", () =>
            {
                string ppszProgId;
                ComServer.GetClassDetails(ref clsid, out ppszProgId, out _userType);
                return ppszProgId;
            }, clsid);
            userType = _userType;
            return result;
        }

        public EnumGuid EnumClassesOfCategories(Guid[] implementedCatid, Guid[] requiredCatid)
        {
            return DoComCall(ComServer, "IOpcServerList::EnumClassesOfCategories", () =>
            {
                int implementedCount = implementedCatid != null ? implementedCatid.Length : 0;
                int requiredCount = requiredCatid != null ? requiredCatid.Length : 0;
                IEnumGUID ppenumClsid;
                ComServer.EnumClassesOfCategories(implementedCount, implementedCatid, requiredCount, requiredCatid,
                    out ppenumClsid);
                return new EnumGuid(ppenumClsid);
            }, implementedCatid, requiredCatid);
        }
    }
}