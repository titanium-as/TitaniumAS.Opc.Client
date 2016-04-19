using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using TitaniumAS.Opc.Client.Common.Internal;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    ///     Represents a service assists to enumerate available hosts and OPC DA servers.
    /// </summary>
    public class OpcServerEnumeratorAuto
    {
        private static readonly Guid OpcServerListCLSID = new Guid("13486D51-4821-11D2-A494-3CB306C10000");
        private static readonly ILog Log = LogManager.GetLogger<OpcServerEnumeratorAuto>();
        private readonly ComProxyBlanket _comProxyBlanket;
        private string _cachedLocalhost;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OpcServerEnumeratorAuto" /> class.
        /// </summary>
        /// <param name="comProxyBlanket">The COM proxy blanket.</param>
        public OpcServerEnumeratorAuto(ComProxyBlanket comProxyBlanket = null)
        {
            _comProxyBlanket = comProxyBlanket;
        }

        /// <summary>
        ///     Gets the localhost name.
        /// </summary>
        /// <value>
        ///     The localhost name.
        /// </value>
        public string Localhost
        {
            get
            {
                if (string.IsNullOrEmpty(_cachedLocalhost))
                {
                    _cachedLocalhost = Interop.Common.Interop.GetLocalComputer();
                }
                return _cachedLocalhost;
            }
        }

        /// <summary>
        ///     Enumrates the hosts.
        /// </summary>
        /// <returns>Array of available host names.</returns>
        public string[] EnumrateHosts()
        {
            return Interop.Common.Interop.GetNetworkComputers();
        }

        /// <summary>
        ///     Provides OPC server class identifier by programmatic identifier and host name.
        /// </summary>
        /// <param name="progId">The OPC server programmatic identifier.</param>
        /// <param name="host">The OPC server host name.</param>
        /// <returns>The OPC server class identifier.</returns>
        public Guid CLSIDFromProgId(string progId, string host)
        {
            if (string.IsNullOrEmpty(host) || host == Localhost) //empty string throws error.
                host = "localhost";

            // COM objects
            object comObject = null;
            try
            {
                // connect to the server.
                comObject = Com.CreateInstanceWithBlanket(OpcServerListCLSID, host, null, _comProxyBlanket);
                List<IOpcServerListEnumerator> serverListEnumerators = CreateEnumerators(comObject);

                foreach (IOpcServerListEnumerator opcServerListEnumerator in serverListEnumerators)
                {
                    try
                    {
                        return opcServerListEnumerator.CLSIDFromProgID(progId);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("ProgId to CLSID with '{0}' enumerator failed.", ex,
                            opcServerListEnumerator.GetType());
                    }
                }

                return Guid.Empty;
            }
            finally
            {
                comObject.ReleaseComServer();
            }
        }

        /// <summary>
        ///     Gets the OPC server description by host name and programmatic identifier.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="progId">The OPC server programmatic identifier.</param>
        /// <returns>The OPC server description.</returns>
        public OpcServerDescription GetServerDescription(string host, string progId)
        {
            if (string.IsNullOrEmpty(host) || host == Localhost) //empty string throws error.
                host = "localhost";

            // COM objects
            object comObject = null;
            try
            {
                // connect to the server.
                comObject = Com.CreateInstanceWithBlanket(OpcServerListCLSID, host, null, _comProxyBlanket);
                List<IOpcServerListEnumerator> serverListEnumerators = CreateEnumerators(comObject);

                foreach (IOpcServerListEnumerator opcServerListEnumerator in serverListEnumerators)
                {
                    try
                    {
                        Guid clsid = opcServerListEnumerator.CLSIDFromProgID(progId);
                        return opcServerListEnumerator.GetServerDescription(host, clsid);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("ProgId to CLSID with '{0}' enumerator failed.", ex,
                            opcServerListEnumerator.GetType());
                    }
                }

                return new OpcServerDescription(host, progId);
            }
            finally
            {
                comObject.ReleaseComServer();
            }
        }

        /// <summary>
        ///     Gets the OPC server description by host name and programmatic identifier.
        /// </summary>
        /// <param name="host">The OPC server host.</param>
        /// <param name="clsid">The OPC server class identifier.</param>
        /// <returns>The OPC server description.</returns>
        public OpcServerDescription GetServerDescription(string host, Guid clsid)
        {
            if (string.IsNullOrEmpty(host) || host == Localhost) //empty string throws error.
                host = "localhost";

            // COM objects
            object comObject = null;
            try
            {
                // connect to the server.
                comObject = Com.CreateInstanceWithBlanket(OpcServerListCLSID, host, null, _comProxyBlanket);
                List<IOpcServerListEnumerator> serverListEnumerators = CreateEnumerators(comObject);

                foreach (IOpcServerListEnumerator opcServerListEnumerator in serverListEnumerators)
                {
                    try
                    {
                        return opcServerListEnumerator.GetServerDescription(host, clsid);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("ProgId to CLSID with '{0}' enumerator failed.", ex,
                            opcServerListEnumerator.GetType());
                    }
                }

                return new OpcServerDescription(host, clsid);
            }
            finally
            {
                comObject.ReleaseComServer();
            }
        }

        /// <summary>
        ///     Provides OPC server descriptions of the host considering specified OPC server categories.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="categories">The OPC server categories.</param>
        /// <returns>The OPC server descriptions.</returns>
        public OpcServerDescription[] Enumerate(string host,
            params OpcServerCategory[] categories)
        {
            return Enumerate(host, true, categories);
        }

        /// <summary>
        ///     Provides OPC server descriptions of the host considering specified OPC server categories.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="loadAllServerCategories">if set to <c>true</c> all OPC server categories will be loaded.</param>
        /// <param name="categories">The OPC server categories.</param>
        /// <returns>The OPC server descriptions.</returns>
        public OpcServerDescription[] Enumerate(string host, bool loadAllServerCategories,
            params OpcServerCategory[] categories)
        {
            if (string.IsNullOrEmpty(host)) //empty string throws error.
                host = "localhost";

            if (categories.Length == 0)
                return new OpcServerDescription[0];

            // COM objects
            object comObject = null;
            try
            {
                // connect to the server.
                comObject = Com.CreateInstanceWithBlanket(OpcServerListCLSID, host, null, _comProxyBlanket);
                Guid[] categoriesGuids = GetCategoriesGuids(categories);
                List<IOpcServerListEnumerator> serverListEnumerators = CreateEnumerators(comObject);

                foreach (IOpcServerListEnumerator opcServerListEnumerator in serverListEnumerators)
                {
                    try
                    {
                        List<OpcServerDescription> servers = opcServerListEnumerator.Enumerate(host, categoriesGuids);
                        if (servers == null)
                            continue;

                        if (loadAllServerCategories)
                            TryLoadServerCategories(host, servers);

                        return servers.ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("Enumeration with '{0}' enumerator failed.", ex,
                            opcServerListEnumerator.GetType());
                    }
                }

                return new OpcServerDescription[0];
            }
            finally
            {
                comObject.ReleaseComServer();
            }
        }

        private void TryLoadServerCategories(string host, List<OpcServerDescription> servers)
        {
            using (var categoryEnumerator = new CategoryEnumerator(host))
            {
                foreach (OpcServerDescription serverDescription in servers)
                {
                    try
                    {
                        serverDescription.Categories =
                            categoryEnumerator.GetCategories(serverDescription.CLSID);
                    }
                    catch (Exception ex)
                    {
                        Log.WarnFormat("Failed to get categories for server with CLSID '{0}'.", ex,
                            serverDescription.CLSID);
                    }
                }
            }
        }

        private List<IOpcServerListEnumerator> CreateEnumerators(object enumerator)
        {
            var result = new List<IOpcServerListEnumerator>();
            try
            {
                result.Add(new OpcServerList2Enumerator(enumerator));
            }
            catch (Exception ex)
            {
                Log.WarnFormat("Enumerator '{0}' not created.", ex, typeof (OpcServerList2Enumerator));
            }

            try
            {
                result.Add(new OpcServerListEnumerator(enumerator));
            }
            catch (Exception ex)
            {
                Log.WarnFormat("Enumerator '{0}' not created.", ex, typeof (OpcServerListEnumerator));
            }
            return result;
        }

        private Guid[] GetCategoriesGuids(OpcServerCategory[] categories)
        {
            return categories.Select(c => c.CATID).ToArray();
        }
    }
}