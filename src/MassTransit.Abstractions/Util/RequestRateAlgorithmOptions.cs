namespace MassTransit.Util
{
    using System;


    public class RequestRateAlgorithmOptions
    {
        /// <summary>
        /// The number of messages to keep in the pipeline at any given time
        /// </summary>
        public int PrefetchCount { get; set; }

        /// <summary>
        /// The maximum number of results that can be retrieved per request
        /// </summary>
        public int RequestResultLimit { get; set; }

        /// <summary>
        /// The maximum number of requests within the given request rate interval
        /// </summary>
        public int? RequestRateLimit { get; set; }

        /// <summary>
        /// The interval at which the request rate limit is reset
        /// </summary>
        public TimeSpan? RequestRateInterval { get; set; }
    }
}
