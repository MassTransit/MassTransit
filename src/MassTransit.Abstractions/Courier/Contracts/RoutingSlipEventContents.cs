namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// Specifies the specific contents of routing slip events to be included for a subscription
    /// </summary>
    [Flags]
    public enum RoutingSlipEventContents
    {
        /// <summary>
        /// Include all event contents
        /// </summary>
        All = 0,

        /// <summary>
        /// Do not include any contents with the routing slip events
        /// </summary>
        None = 0x100,

        /// <summary>
        /// The routing slip variables after the activity was executed or compensated
        /// </summary>
        Variables = 0x0001,

        /// <summary>
        /// The arguments provided to the activity
        /// </summary>
        Arguments = 0x0002,

        /// <summary>
        /// The data logged by an activity when completed or compensated
        /// </summary>
        Data = 0x0004,

        /// <summary>
        /// The itinerary that was added/removed from the routing slip when revised
        /// </summary>
        Itinerary = 0x0008,

        /// <summary>
        /// If specified, encrypted content is excluded from the event
        /// </summary>
        SkipEncrypted = 0x0100
    }
}
