namespace MassTransit
{
    using System;


    /// <summary>
    /// Interface that specifies a class is usable as a saga instance, including
    /// the ability to get and set the CorrelationId on the saga instance.
    /// </summary>
    public interface ISaga
    {
        /// <summary>
        /// Identifies the saga instance uniquely, and is the primary correlation
        /// for the instance. While the setter is not typically called, it is there
        /// to support persistence consistently across implementations.
        /// </summary>
        Guid CorrelationId { get; set; }
    }
}
