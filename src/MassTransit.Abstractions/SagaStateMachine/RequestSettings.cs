namespace MassTransit
{
    using System;


    /// <summary>
    /// The request settings include the address of the request handler, as well as the timeout to use
    /// for requests.
    /// </summary>
    public interface RequestSettings
    {
        /// <summary>
        /// The endpoint address of the service that handles the request
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// The timeout period before the request times out
        /// </summary>
        TimeSpan Timeout { get; }
    }
}
