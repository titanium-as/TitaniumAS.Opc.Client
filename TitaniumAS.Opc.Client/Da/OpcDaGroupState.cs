using System;
using System.Globalization;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents OPC DA group state.
    /// </summary>
    public class OpcDaGroupState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaGroupState"/> class.
        /// </summary>
        public OpcDaGroupState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaGroupState"/> class.
        /// </summary>
        /// <param name="updateRate">The update rate of the group (milliseconds).</param>
        /// <param name="isActive"><c>true</c> if the group is active; otherwise, <c>false</c>.</param>
        /// <param name="timeBias">The TimeZone Bias of the group (in minutes).</param>
        /// <param name="percentDeadband">The percent deadband.</param>
        /// <param name="culture">The current culture for the group.</param>
        /// <param name="clientHandle">The client handle.</param>
        /// <param name="serverHandle">The server handle of the group.</param>
        public OpcDaGroupState(TimeSpan? updateRate, bool? isActive, TimeSpan? timeBias, float? percentDeadband,
            CultureInfo culture, int? clientHandle, int? serverHandle)
        {
            UpdateRate = updateRate;
            IsActive = isActive;
            TimeBias = timeBias;
            PercentDeadband = percentDeadband;
            Culture = culture;
            ClientHandle = clientHandle;
            ServerHandle = serverHandle;
        }

        /// <summary>
        /// Gets or sets the update rate requested for the group (milliseconds). If null then omitted.
        /// </summary>
        /// <value>
        /// The update rate of the group (milliseconds).
        /// </value>
        public TimeSpan? UpdateRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group is active. If null then omitted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the group is active; otherwise, <c>false</c>.
        /// </value>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the TimeZone Bias of the group (in minutes). If null then omitted.
        /// </summary>
        /// <value>
        /// The TimeZone Bias of the group (in minutes).
        /// </value>
        public TimeSpan? TimeBias { get; set; }

        /// <summary>
        /// Gets or sets the range of the Deadband is from 0.0 to 100.0 Percent. If null then omitted.
        /// </summary>
        /// <value>
        /// The percent deadband.
        /// </value>
        public float? PercentDeadband { get; set; }

        /// <summary>
        /// Gets or sets the current culture for the group. If null then omitted.
        /// </summary>
        /// <value>
        /// The current culture for the group.
        /// </value>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets or sets the client handle of the group. If null then omitted.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        public int? ClientHandle { get; set; }

        /// <summary>
        /// Gets the server handle of the group. If null then omitted.
        /// </summary>
        /// <value>
        /// The server handle of the group.
        /// </value>
        public int? ServerHandle { get; set; }

        /// <summary>
        /// Gets or set arbitrary user data of the group. If null then omitted.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        public object UserData { get; set; }
    }
}