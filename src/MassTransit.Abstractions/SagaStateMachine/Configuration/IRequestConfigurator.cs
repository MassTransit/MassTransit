namespace MassTransit
{
    using System;


    public interface IRequestConfigurator
    {
        /// <summary>
        /// Sets the service address of the request handler
        /// </summary>
        Uri ServiceAddress { set; }

        /// <summary>
        /// Sets the request timeout
        /// </summary>
        TimeSpan Timeout { set; }

        /// <summary>
        /// Set the time to live of the request message sent by the saga. If not specified, and the timeout
        /// is > TimeSpan.Zero, the <see cref="Timeout"/> value is used.
        /// </summary>
        TimeSpan? TimeToLive { set; }
    }
}
