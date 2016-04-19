using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents an OPC server URL builder.
    /// </summary>
    public static class UrlBuilder
    {
        /// <summary>
        /// The scheme of OPC DA server URL.
        /// </summary>
        public const string OpcDaScheme = "opcda";

        /// <summary>
        /// Builds an OPC DA server URL.
        /// </summary>
        /// <param name="progIdOrClsid">The OPC server programmatic or class identifier.</param>
        /// <param name="host">The host name. If <c>null</c> the localhost will be used.</param>
        /// <returns>
        /// The OPC DA server URL.
        /// </returns>
        public static Uri Build(string progIdOrClsid, string host = null)
        {
            host = string.IsNullOrEmpty(host) ? "localhost" : host;
            return new UriBuilder(OpcDaScheme, host) {Path = progIdOrClsid}.Uri;
        }

        /// <summary>
        /// Builds an OPC DA server URL.
        /// </summary>
        /// <param name="clsid">The OPC server class identifier.</param>
        /// <param name="host">The host name. If <c>null</c> the localhost will be used.</param>
        /// <returns>
        /// The OPC DA server URL.
        /// </returns>
        public static Uri Build(Guid clsid, string host = null)
        {
            host = string.IsNullOrEmpty(host) ? "localhost" : host;
            return new UriBuilder(OpcDaScheme, host) {Path = clsid.ToString()}.Uri;
        }

        /// <summary>
        /// Combines the specified OPC DA server URL with path.
        /// </summary>
        /// <param name="url">The OPC DA server URL.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The OPC DA server URL with path.
        /// </returns>
        public static Uri Combine(Uri url, string path)
        {
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Path = string.Concat(uriBuilder.Path, @"/", Uri.EscapeDataString(path)); //workaround for split ProgId & ItemId in URL path
            Uri result = uriBuilder.Uri;
            return result;
        }
    }
}