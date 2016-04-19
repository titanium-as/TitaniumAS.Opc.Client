using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    public class OpcEnumGuid : ComWrapper, IDisposable
    {
        internal OpcEnumGuid(IOPCEnumGUID comServer)
        {
            if (comServer == null) throw new ArgumentNullException("comServer");

            ComServer = comServer;
        }

        internal IOPCEnumGUID ComServer { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OpcEnumGuid()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (ComServer != null)
            {
                Marshal.ReleaseComObject(ComServer);
                ComServer = null;
            }
        }

        public int Next(Guid[] guids)
        {
            return DoComCall(ComServer, "IOpcEnumGuid::Next", () =>
            {
                int fetched;
                ComServer.Next(guids.Length, guids, out fetched);
                return fetched;
            }, guids);
        }

        public void Skip(int count)
        {
            DoComCall(ComServer, "IOpcEnumGuid::Skip", () => ComServer.Skip(count), count);
        }

        public void Reset()
        {
            DoComCall(ComServer, "IOpcEnumGuid::Reset", () => ComServer.Reset());
        }

        public OpcEnumGuid Clone()
        {
            return DoComCall(ComServer, "IOpcEnumGuid::Clone", () =>
            {
                IOPCEnumGUID ppenum;
                ComServer.Clone(out ppenum);
                return new OpcEnumGuid(ppenum);
            });
        }
    }
}