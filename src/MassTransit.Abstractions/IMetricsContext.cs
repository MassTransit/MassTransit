namespace MassTransit
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to tags added to metrics.
    /// </summary>
    public interface IMetricsContext
    {
        /// <summary>
        /// Gets the tag collection.
        /// </summary>
        ICollection<KeyValuePair<string, object?>> Tags { get; }
    }
}
