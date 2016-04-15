using System.Text;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Interop.Helpers
{
    internal class OpcDaQualityHelpers
    {
        public static string QualityToString(short quality)
        {
            var stringBuilder = new StringBuilder(256);
            var opcQualityMaster = (OPC_QUALITY_MASTER) (quality & 192);
            var opcQualityStatus = (OPC_QUALITY_STATUS) (quality & 252);
            var opcQualityLimit = (OPC_QUALITY_LIMIT) (quality & 3);
            stringBuilder.AppendFormat("{0}+{1}+{2}", opcQualityMaster, opcQualityStatus, opcQualityLimit);
            return stringBuilder.ToString();
        }
    }
}