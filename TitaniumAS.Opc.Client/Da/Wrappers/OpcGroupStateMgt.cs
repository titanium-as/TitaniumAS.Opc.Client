using System;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    internal class OpcGroupStateMgt : ComWrapper
    {
        public OpcGroupStateMgt(object comObject, object userData):base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCGroupStateMgt>",
                () => comObject.QueryInterface<IOPCGroupStateMgt>());
        }

        internal IOPCGroupStateMgt ComObject { get; set; }

        public void GetState(out TimeSpan updateRate, out bool active, out string name, out TimeSpan timeBias,
            out float percentDeadband, out int lcid, out int clientHandle, out int serverHandle)
        {
            bool _active = false;
            string _name = string.Empty;
            float _percentDeadband = 0;
            int _lcid = 0;
            int _clientHandle = 0;
            int _serverHandle = 0;
            var _updateRate = new TimeSpan();
            var _timeBias = new TimeSpan();
            DoComCall(ComObject, "IOPCGroupStateMgt::GetState", () =>
            {
                int pUpdateRate;
                int pTimeBias;
                ComObject.GetState(out pUpdateRate, out _active, out _name, out pTimeBias, out _percentDeadband,
                    out _lcid, out _clientHandle, out _serverHandle);

                _updateRate = TimeSpan.FromMilliseconds(pUpdateRate);
                _timeBias = TimeSpan.FromMinutes(pTimeBias);
            });
            active = _active;
            name = _name;
            percentDeadband = _percentDeadband;
            lcid = _lcid;
            clientHandle = _clientHandle;
            serverHandle = _serverHandle;
            updateRate = _updateRate;
            timeBias = _timeBias;
        }

        public TimeSpan SetState(TimeSpan? requestedUpdateRate, bool? active,
            TimeSpan? timeBias,
            float? percentDeadband,
            int? LCID,
            int? clientHandle)
        {
            int pRevisedUpdateRate = 0;

            int[] pRequestedUpdateRate = null;
            if (requestedUpdateRate.HasValue)
            {
                pRequestedUpdateRate = new[] {(int) requestedUpdateRate.Value.TotalMilliseconds};
            }
            bool[] pActive = null;
            if (active.HasValue)
            {
                pActive = new[] {active.Value};
            }
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
            int[] pLCID = null;
            if (LCID.HasValue)
            {
                pLCID = new[] {LCID.Value};
            }
            int[] phClientGroup = null;
            if (clientHandle.HasValue)
            {
                phClientGroup = new[] {clientHandle.Value};
            }
            DoComCall(ComObject, "IOPCGroupStateMgt::SetState", () =>
                ComObject.SetState(pRequestedUpdateRate, out pRevisedUpdateRate, pActive, pTimeBias, pPercentDeadband,
                    pLCID, phClientGroup),
                pRequestedUpdateRate, pActive, pTimeBias, pPercentDeadband, pLCID);
            return TimeSpan.FromMilliseconds(pRevisedUpdateRate);
        }

        public void SetName(string name)
        {
            DoComCall(ComObject, "IOPCGroupStateMgt::SetName", () => ComObject.SetName(name), name);
        }

        public object CloneGroup(string name)
        {
            return DoComCall(ComObject, "IOPCGroupStateMgt::CloneGroup", () =>
            {
                object ppUnk;
                Guid riid = Com.IUnknownIID;
                ComObject.CloneGroup(name, ref riid, out ppUnk);
                return ppUnk;
            }, name);
        }
    }
}