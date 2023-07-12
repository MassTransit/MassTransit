namespace MassTransit
{
    using System;


    [Flags]
    public enum CompositeEventOptions
    {
        None = 0,

        /// <summary>
        /// Include the composite event in the initial state
        /// </summary>
        IncludeInitial = 1,

        /// <summary>
        /// Include the composite event in the final state
        /// </summary>
        IncludeFinal = 2,

        /// <summary>
        /// Specifies that the composite event should only be raised once and ignore any subsequent events
        /// </summary>
        RaiseOnce = 4,
    }
}
