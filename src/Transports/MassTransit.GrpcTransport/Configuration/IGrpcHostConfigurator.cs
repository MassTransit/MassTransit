namespace MassTransit
{
    using System;


    public interface IGrpcHostConfigurator
    {
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
