namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a quality status of specific item.
    /// </summary>
    public enum OpcDaQualityStatus : short
    {
        /// <summary>
        /// The value is bad but no specific reason is known.
        /// </summary>
        Bad = 0,

        /// <summary>
        /// There is some server specific problem with the configuration. For example the item in question has been deleted from the configuration.
        /// </summary>
        BadConfigurationError = (short) 4,

        /// <summary>
        /// The input is required to be logically connected to something but is not. This quality may reflect that no value is available at this time, for reasons like the value may have not been provided by the data source.
        /// </summary>
        BadNotConnected = (short) 8,

        /// <summary>
        /// A device failure has been detected.
        /// </summary>
        BadDeviceFailure = BadNotConnected | BadConfigurationError,

        /// <summary>
        /// A sensor failure had been detected (the 'Limits' field can provide additional diagnostic information in some situations).
        /// </summary>
        BadSensorFailure = (short) 16,

        /// <summary>
        /// Communications have failed. However, the last known value is available. Note that the 'age' of the value may be determined from the TIMESTAMP in the OPCITEMSTATE.
        /// </summary>
        BadLastKnown = BadSensorFailure | BadConfigurationError,

        /// <summary>
        /// Communications have failed. There is no last known value is available.
        /// </summary>
        BadCommFailure = BadSensorFailure | BadNotConnected,

        /// <summary>
        /// The block is off scan or otherwise locked. This quality is also used when the active state of the item or the group containing the item is InActive.
        /// </summary>
        BadOutOfService = BadCommFailure | BadConfigurationError,

        /// <summary>
        /// After Items are added to a group, it may take some time for the server to actually obtain values for these items. In such cases the client might perform a read (from cache), or establish a ConnectionPoint based subscription and/or execute a Refresh on such a subscription before the values are available. This substatus is only available from OPC DA 3.0 or newer servers.
        /// </summary>
        BadWaitingForInitialData = (short) 32,

        /// <summary>
        /// There is no specific reason why the value is uncertain.
        /// </summary>
        Uncertain = (short) 64,

        /// <summary>
        /// Whatever was writing this value has stopped doing so. The returned value should be regarded as 'stale'. Note that this differs from a BAD value with Substatus 5 (Last Known Value). That status is associated specifically with a detectable communications error on a 'fetched' value. This error is associated with the failure of some external source to 'put' something into the value within an acceptable period of time. Note that the 'age' of the value can be determined from the TIMESTAMP in OPCITEMSTATE.
        /// </summary>
        UncertainLastUsableValue = Uncertain | BadConfigurationError,

        /// <summary>
        /// Either the value has 'pegged' at one of the sensor limits (in which case the limit field should be set to 1 or 2) or the sensor is otherwise known to be out of calibration via some form of internal diagnostics (in which case the limit field should be 0).
        /// </summary>
        UncertainSensorNotAccurate = Uncertain | BadSensorFailure,

        /// <summary>
        /// The returned value is outside the limits defined for this parameter. Note that in this case (per the Fieldbus Specification) the 'Limits' field indicates which limit has been exceeded but does NOT necessarily imply that the value cannot move farther out of range.
        /// </summary>
        UncertainEngineeringUnitsExceeded = UncertainSensorNotAccurate | BadConfigurationError,

        /// <summary>
        /// The value is derived from multiple sources and has less than the required number of Good sources.
        /// </summary>
        UncertainSubNormal = UncertainSensorNotAccurate | BadNotConnected,

        /// <summary>
        /// The value is good. There are no special conditions.
        /// </summary>
        Good = (short) 192,

        /// <summary>
        /// The value has been Overridden. Typically this is means the input has been disconnected and a manually entered value has been 'forced'.
        /// </summary>
        GoodLocalOverride = Good | BadCommFailure
    }
}