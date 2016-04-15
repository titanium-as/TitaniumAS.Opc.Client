using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;

namespace TitaniumAS.Opc.Client.Common.Wrappers
{
    public class EnumGuid : ComWrapper, IDisposable
    {
        internal EnumGuid(IEnumGUID comServer)
        {
            if (comServer == null) throw new ArgumentNullException("comServer");

            ComServer = comServer;
        }

        internal IEnumGUID ComServer { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EnumGuid()
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
            return DoComCall(ComServer, "IEnumGuid::Next", () =>
            {
                int fetched;
                ComServer.Next(guids.Length, guids, out fetched);
                return fetched;
            }, guids);
        }

        public void Skip(int count)
        {
            DoComCall(ComServer, "IEnumGuid::Skip", () => ComServer.Skip(count), count);
        }

        public void Reset()
        {
            DoComCall(ComServer, "IEnumGuid::Reset", () => ComServer.Reset());
        }

        public EnumGuid Clone()
        {
            return DoComCall(ComServer, "IEnumGuid::Clone", () =>
            {
                IEnumGUID ppenum;
                ComServer.Clone(out ppenum);
                return new EnumGuid(ppenum);
            });
        }
    }
}