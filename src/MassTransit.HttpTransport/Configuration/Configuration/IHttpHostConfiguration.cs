namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using Hosting;
    using MassTransit.Configuration;


    public interface IHttpHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IHttpReceiveEndpointConfigurator>
    {
        HttpHostSettings Settings { get; set; }

        IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure = null);

        IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch, IHttpEndpointConfiguration endpointConfiguration,
            Action<IHttpReceiveEndpointConfigurator> configure = null);
    }
}
