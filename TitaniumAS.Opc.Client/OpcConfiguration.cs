using System;

namespace TitaniumAS.Opc.Client
{
    /// <summary>
    /// OPC configuration which is used by default.
    /// </summary>
    public static class OpcConfiguration
    {
        /// <summary>
        /// Initializes the <see cref="OpcConfiguration"/> class.
        /// </summary>
        static OpcConfiguration()
        {
            BatchSize = 1024;
            MaxSimultaneousRequests = 64;
            RequestTimeout = TimeSpan.FromSeconds(5);
            EnableQuirks = false;
        }

        /// <summary>
        /// Gets or sets the size of the batch.А
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        public static int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum simultaneous requests.
        /// </summary>
        /// <value>
        /// The maximum simultaneous requests.
        /// </value>
        public static int MaxSimultaneousRequests { get; set; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        /// <value>
        /// The request timeout.
        /// </value>
        public static TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether quirks are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if quirks are enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableQuirks { get; set; }
    }
}