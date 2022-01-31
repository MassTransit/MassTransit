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
        /// Composite bound to all events
        /// </summary>
        All = 2
    }
}
