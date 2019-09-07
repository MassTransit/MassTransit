namespace MassTransit.HttpTransport
{
    using System;
    using Hosting;


    public interface IHttpBusFactoryConfigurator :
        IBusFactoryConfigurator<IHttpReceiveEndpointConfigurator>,
        IReceiveConfigurator<IHttpHost, IHttpReceiveEndpointConfigurator>
    {
        IHttpHost Host(HttpHostSettings settings);

        /// <summary>
        /// Maps a handler to the path specified
        /// </summary>
        /// <param name="configure">Configures the receive endpoint on this path</param>
        void ReceiveEndpoint(Action<IHttpReceiveEndpointConfigurator> configure = null);
    }
}
