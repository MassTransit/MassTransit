namespace MassTransit.Introspection
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// The result of a probe
    /// </summary>
    public interface ProbeResult
    {
        /// <summary>
        /// Unique identifies this result
        /// </summary>
        Guid ResultId { get; }

        /// <summary>
        /// Identifies the initiator of the probe
        /// </summary>
        Guid ProbeId { get; }

        /// <summary>
        /// When the probe was initiated through the system
        /// </summary>
        DateTime StartTimestamp { get; }

        /// <summary>
        /// How long the probe took to execute
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The host from which the result was generated
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The results returned by the probe
        /// </summary>
        IDictionary<string, object> Results { get; }
    }
}
