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
        IncludeFinal = 2
    }
}
