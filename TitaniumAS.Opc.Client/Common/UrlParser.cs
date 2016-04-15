using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents an OPC server URL parser.
    /// </summary>
    public static class UrlParser
    {
        /// <summary>
        /// Parses an OPC DA server URL.
        /// </summary>
        /// <param name="opcServerUrl">The OPC DA server URL.</param>
        /// <param name="withProgId">An action (host, progId) called when the URL contains programmatic identifier.</param>
        /// <param name="withCLSID">An action (host, clsid) called when the URL contains class identifier.</param>
        public static void Parse(Uri opcServerUrl, Action<string, string> withProgId, Action<string, Guid> withCLSID)
        {
            string segment = opcServerUrl.Segments[1];
            string progIdOrClsid = segment.TrimEnd('/'); //workaround for split ProgId & ItemId in URL path
            Guid clsid;
            if (Guid.TryParse(progIdOrClsid, out clsid))
            {
                withCLSID(opcServerUrl.Host, clsid);
            }
            else
            {
                withProgId(opcServerUrl.Host, progIdOrClsid);
            }
        }

        /// <summary>
        /// Parses an OPC DA server URL.
        /// </summary>
        /// <typeparam name="T">The type of returned value.</typeparam>
        /// <param name="opcServerUrl">The OPC DA server URL.</param>
        /// <param name="withProgId">A function (host, progId) called when the URL contains programmatic identifier.</param>
        /// <param name="withCLSID">A function (host, clsid) called when the URL contains class identifier.</param>
        /// <returns>
        /// The returned value of one called function.
        /// </returns>
        public static T Parse<T>(Uri opcServerUrl, Func<string, string, T> withProgId, Func<string, Guid, T> withCLSID)
        {
            string sergment = opcServerUrl.Segments[1];
            string progIdOrClsid = sergment.TrimEnd('/'); //workaround for split ProgId & ItemId in URL path
            Guid clsid;
            if (Guid.TryParse(progIdOrClsid, out clsid))
            {
                return withCLSID(opcServerUrl.Host, clsid);
            }
            return withProgId(opcServerUrl.Host, progIdOrClsid);
        }
    }
}