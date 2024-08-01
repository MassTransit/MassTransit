namespace MassTransit
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to tags added to metrics.
    /// </summary>
    public sealed class MetricsContext : IMetricsContext
    {
        /// <summary>
        /// Gets the tag collection.
        /// </summary>
        public ICollection<KeyValuePair<string, object>> Tags { get; } = new List<KeyValuePair<string, object>>();
    }
}
