using System;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    public class OpcServerList2 : ComWrapper
    {
        public OpcServerList2(object comServer)
        {
            if (comServer == null) throw new ArgumentNullException("comServer");

            ComServer = DoComCall(comServer, "IUnknown::QueryInterface<IOpcServerList2>",
                () => comServer.QueryInterface<IOPCServerList2>());
        }

        private IOPCServerList2 ComServer { get; set; }

        public Guid CLSIDFromProgID(string progId)
        {
            return DoComCall(ComServer, "IOpcServerList2::CLSIDFromProgID", () =>
            {
                Guid clsid;
                ComServer.CLSIDFromProgID(progId, out clsid);
                return clsid;
            }, progId);
        }

        public string GetClassDetails(Guid clsid, out string userType, out string vendorIndependentProgId)
        {
            string _vendorIndependentProgId = string.Empty;
            string _userType = string.Empty;
            string result = DoComCall(ComServer, "IOpcServerList2::GetClassDetails", () =>
            {
                string ppszProgId;
                ComServer.GetClassDetails(ref clsid, out ppszProgId, out _userType, out _vendorIndependentProgId);
                return ppszProgId;
            }, clsid);
            userType = _userType;
            vendorIndependentProgId = _vendorIndependentProgId;
            return result;
        }

        public OpcEnumGuid EnumClassesOfCategories(Guid[] implementedCatid, Guid[] requiredCatid)
        {
            return DoComCall(ComServer, "IOpcServerList2::EnumClassesOfCategories", () =>
            {
                int implementedCount = implementedCatid != null ? implementedCatid.Length : 0;
                int requiredCount = requiredCatid != null ? requiredCatid.Length : 0;
                IOPCEnumGUID ppenumClsid;
                ComServer.EnumClassesOfCategories(implementedCount, implementedCatid, requiredCount, requiredCatid,
                    out ppenumClsid);
                return new OpcEnumGuid(ppenumClsid);
            }, implementedCatid, requiredCatid);
        }
    }
}