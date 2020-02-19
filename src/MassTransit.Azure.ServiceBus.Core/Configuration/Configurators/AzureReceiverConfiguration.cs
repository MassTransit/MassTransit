namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;
    using GreenPipes;
    using MassTransit.Configuration;
    using Transports;


    public abstract class AzureReceiverConfiguration :
        ReceiverConfiguration,
        IReceiverConfigurator
    {
        readonly IReceiveEndpointConfiguration _configuration;

        protected AzureReceiverConfiguration(IReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            InputAddress = new Uri("sb://localhost/");
        }

        public Uri InputAddress { get; set; }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configuration.ConnectReceiveEndpointObserver(observer);
        }
    }
}
