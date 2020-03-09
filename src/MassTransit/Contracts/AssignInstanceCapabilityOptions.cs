namespace MassTransit.Contracts
{
    using System;


    [Flags]
    public enum AssignInstanceCapabilityOptions
    {
        /// <summary>
        /// Assignment is required
        /// </summary>
        Required = 1,
    }
}
