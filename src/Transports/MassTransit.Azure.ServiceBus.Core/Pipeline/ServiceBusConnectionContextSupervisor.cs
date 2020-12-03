namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using Topology;
    using Transport;
    using Transports;


    public class ServiceBusConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly IServiceBusTopologyConfiguration _topologyConfiguration;

        public ServiceBusConnectionContextSupervisor(IServiceBusHostConfiguration hostConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
            : base(new ConnectionContextFactory(hostConfiguration))
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            IServiceBusMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings();

            var transport = CreateSendTransport(publishAddress, settings);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            var endpointAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var transport = CreateSendTransport(endpointAddress, settings);

            return Task.FromResult(transport);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        ISendTransport CreateSendTransport(Uri address, SendSettings settings)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            TransportLogMessages.CreateSendTransport(address);

            var endpointContextSupervisor = CreateSendEndpointContextSupervisor(settings);

            var transportContext = new SendTransportContext(address, endpointContextSupervisor, _hostConfiguration.SendLogContext);

            return new ServiceBusSendTransport(transportContext);
        }

        public ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings)
        {
            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), false, Stopping);

            var contextFactory = new SendEndpointContextFactory(this, configureTopology.ToPipe<SendEndpointContext>(), settings);

            var contextSupervisor = new SendEndpointContextSupervisor(contextFactory);

            AddAgent(contextSupervisor);

            return contextSupervisor;
        }


        class SendTransportContext :
            BaseSendTransportContext,
            ServiceBusSendTransportContext
        {
            public SendTransportContext(Uri address, ISendEndpointContextSupervisor source, ILogContext logContext)
                : base(logContext)
            {
                Address = address;
                Supervisor = source;
            }

            public Uri Address { get; }
            public ISendEndpointContextSupervisor Supervisor { get; }
        }
    }
}
