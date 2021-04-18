namespace MassTransit.GrpcTransport
{
    using System;


    public interface IGrpcHostConfigurator :
        IHostConfigurator
    {
        /// <summary>
        /// Sets the maximum number of threads used by an in-memory transport, for partitioning
        /// the input queue. This setting also specifies how many threads will be used for dispatching
        /// messages to consumers.
        /// </summary>
        int TransportConcurrencyLimit { set; }

        /// <summary>
        /// Set the port for the http server
        /// </summary>
        int Port { set; }

        /// <summary>
        /// Set the host name
        /// </summary>
        string Host { set; }

        /// <summary>
        /// Add a server to connect to on startup as part of the message fabric
        /// </summary>
        /// <param name="address"></param>
        void AddServer(Uri address);
    }
}
