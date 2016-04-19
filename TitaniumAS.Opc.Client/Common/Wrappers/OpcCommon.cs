using System;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    public class OpcCommon : ComWrapper
    {
        public OpcCommon(OpcDaServer opcDaServer, object userData) : this(opcDaServer.ComObject, userData)
        {
        }

        public OpcCommon(object comOpcServer, object userData)
        {
            if (comOpcServer == null) throw new ArgumentNullException("comOpcServer");

            ComOpcServer = DoComCall(comOpcServer, "IUnknown::QueryInterface<IOpcCommon>",
                () => comOpcServer.QueryInterface<IOPCCommon>());
        }

        private IOPCCommon ComOpcServer { get; set; }

        public int LocaleID
        {
            get
            {
                return DoComCall(ComOpcServer, "IOpcCommon::GetLocaleID", () =>
                {
                    int pwdLcid;
                    ComOpcServer.GetLocaleID(out pwdLcid);
                    return pwdLcid;
                });
            }
            set { ComOpcServer.SetLocaleID(value); }
        }

        public string ClientName
        {
            set
            {
                DoComCall(ComOpcServer, "IOpcCommon::SetClientName", () => ComOpcServer.SetClientName(value), value);
            }
        }

        public int[] QueryAvailableLocaleIDs()
        {
            return DoComCall(ComOpcServer, "IOpcCommon::QueryAvailableLocaleIDs", () =>
            {
                int dwCount;
                int[] pdwLcid;
                ComOpcServer.QueryAvailableLocaleIDs(out dwCount, out pdwLcid);
                return pdwLcid;
            });
        }

        public string GetErrorString(int error)
        {
            return DoComCall(ComOpcServer, "IOpcCommon::GetErrorString", () =>
            {
                string ppString;
                ComOpcServer.GetErrorString(error, out ppString);
                return ppString;
            }, error);
        }
    }
}