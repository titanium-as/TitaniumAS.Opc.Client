using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da.Internal
{
    internal static class ArrayHelpers
    {
        public static OpcDaItemProperties[] CreateEmptyItemProperties(int size)
        {
            var result = new OpcDaItemProperties[size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new OpcDaItemProperties {Properties = new OpcDaItemProperty[0]};
            }
            return result;
        }

        public static int[] GetServerHandles(IList<OpcDaItem> items)
        {
            var serverHandles = new int[items.Count];
            for (int i = 0; i < serverHandles.Length; i++)
            {
                serverHandles[i] = items[i].ServerHandle;
            }
            return serverHandles;
        }

        public static TimeSpan[] CreateMaxAges(int count, TimeSpan maxAge)
        {
            var maxAges = new TimeSpan[count];
            for (int i = 0; i < maxAges.Length; i++)
            {
                maxAges[i] = maxAge;
            }
            return maxAges;
        }

        public static int[] CreateMaxAgeArray(IList<TimeSpan> maxAge, int itemCount)
        {
            if (maxAge == null)
            {
                var intMaxAge = new int[itemCount];
                for (int i = 0; i < intMaxAge.Length; i++)
                {
                    intMaxAge[i] = unchecked((int) 0xFFFFFFFF);
                }
                return intMaxAge;
            }
            else
            {
                var intMaxAge = new int[maxAge.Count];
                for (int i = 0; i < intMaxAge.Length; i++)
                {
                    TimeSpan t = maxAge[i];
                    if (t == TimeSpan.Zero) // FROM DEVICE
                        intMaxAge[i] = 0;
                    else if (t == TimeSpan.MaxValue) // FROM CACHE
                        intMaxAge[i] = unchecked((int) 0xFFFFFFFF);
                    else
                        intMaxAge[i] = (int) t.TotalMilliseconds;
                }
                return intMaxAge;
            }
        }

        public static OPCITEMVQT[] CreateOpcItemVQT(IList<OpcDaVQT> values)
        {
            var vqts = new OPCITEMVQT[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                OpcDaVQT value = values[i];
                vqts[i].bQualitySpecified = false;
                if (value.Quality != short.MinValue)
                {
                    vqts[i].bQualitySpecified = true;
                    vqts[i].wQuality = value.Quality;
                }

                vqts[i].bTimeStampSpecified = false;
                if (value.Timestamp != DateTimeOffset.MinValue)
                {
                    vqts[i].bTimeStampSpecified = true;
                    vqts[i].ftTimeStamp = FileTimeConverter.ToFileTime(value.Timestamp);
                }

                vqts[i].vDataValue = value.Value;
            }
            return vqts;
        }

        public static OPCITEMDEF[] CreateOPITEMDEFs(IList<OpcDaItemDefinition> opcItemDefinitions)
        {
            var pItemArray = new OPCITEMDEF[opcItemDefinitions.Count];
            for (int i = 0; i < opcItemDefinitions.Count; i++)
            {
                pItemArray[i] = opcItemDefinitions[i].ToStruct();
            }
            return pItemArray;
        }

        public static OPCITEMRESULT[] CreateOpcItemResults(OPCITEMDEF[] pItemArray, IntPtr ppAddResults)
        {
            var results = new OPCITEMRESULT[pItemArray.Length];
            for (int i = 0; i < pItemArray.Length; i++)
            {
                IntPtr current = ppAddResults + i*Marshal.SizeOf(typeof (OPCITEMRESULT));

                results[i] = (OPCITEMRESULT) Marshal.PtrToStructure(current, typeof (OPCITEMRESULT));
                Marshal.DestroyStructure(current, typeof (OPCITEMRESULT));
                Marshal.FreeCoTaskMem(pItemArray[i].pBlob); // Delete allocated blobs
            }
            return results;
        }
    }
}