namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using Contracts;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Topology.Builders;
    using Topology.Configurators;
    using Transports;


    public sealed class GrpcTransportProvider :
        Supervisor,
        IGrpcTransportProvider
    {
        const int MaxMessageLengthBytes = int.MaxValue;
        readonly IList<IGrpcClient> _clients;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly IMessageFabric _messageFabric;
        readonly NodeCollection _nodeCollection;
        readonly Server _server;
        readonly Lazy<Task> _startupTask;
        readonly IGrpcTopologyConfiguration _topologyConfiguration;

        public GrpcTransportProvider(IGrpcHostConfiguration hostConfiguration, IGrpcTopologyConfiguration topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _messageFabric = new MessageFabric();

            _nodeCollection = new NodeCollection(this, _messageFabric);
            _clients = new List<IGrpcClient>();

            var transport = new GrpcTransportService(_hostConfiguration, _nodeCollection);

            _server = new Server(GetChannelOptions())
            {
                Services = {TransportService.BindService(transport)},
                Ports = {new ServerPort(hostConfiguration.BaseAddress.Host, hostConfiguration.BaseAddress.Port, ServerCredentials.Insecure)}
            };

            var serverPort = _server.Ports.First();

            HostAddress = new UriBuilder(_hostConfiguration.BaseAddress)
            {
                Host = serverPort.Host,
                Port = serverPort.BoundPort
            }.Uri;

            HostNodeContext = new GrpcHostNodeContext(HostAddress);

            var hostNode = _nodeCollection.GetNode(HostNodeContext);
            _nodeCollection.HostNode = hostNode;

            var observer = new NodeMessageFabricObserver(_nodeCollection);

            _messageFabric.ConnectMessageFabricObserver(observer);

            _startupTask = new Lazy<Task>(() => Task.Run(() => Startup()));
        }

        public Task StartupTask => _startupTask.Value;

        public Uri HostAddress { get; }
        public NodeContext HostNodeContext { get; }

        public IMessageFabric MessageFabric => _messageFabric;

        public Task<ISendTransport> GetSendTransport(GrpcReceiveEndpointContext context, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new GrpcEndpointAddress(HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            var exchange = _messageFabric.GetExchange(HostNodeContext, endpointAddress.Name, endpointAddress.ExchangeType);

            var transportContext = new ExchangeGrpcSendTransportContext(_hostConfiguration, exchange);

            return Task.FromResult<ISendTransport>(new GrpcSendTransport(transportContext));
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new GrpcEndpointAddress(HostAddress, address);
        }

        public Task<ISendTransport> GetPublishTransport<T>(GrpcReceiveEndpointContext context, Uri publishAddress)
            where T : class
        {
            IGrpcMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            publishTopology.Apply(new GrpcPublishTopologyBuilder(HostNodeContext, _messageFabric));

            return GetSendTransport(context, publishAddress);
        }

        public void Probe(ProbeContext context)
        {
            _messageFabric.Probe(context);
        }

        IGrpcClient GetClient(Uri address)
        {
            var channel = GrpcChannel.ForAddress(address.GetLeftPart(UriPartial.Authority), new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                MaxReceiveMessageSize = MaxMessageLengthBytes
            });

            var client = new TransportService.TransportServiceClient(channel);

            return new GrpcClient(_hostConfiguration, address, client, _nodeCollection);
        }

        async Task Startup()
        {
            _server.Start();

            foreach (var server in _hostConfiguration.ServerConfigurations)
            {
                var client = GetClient(server.Address);

                _clients.Add(client);

                Add(client);
            }

            await Task.WhenAll(_clients.Select(x => x.Ready)).OrCanceled(Stopping).ConfigureAwait(false);
        }

        static IEnumerable<ChannelOption> GetChannelOptions()
        {
            return new[]
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, MaxMessageLengthBytes),
                new ChannelOption(ChannelOptions.MaxSendMessageLength, MaxMessageLengthBytes)
            };
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("gRPC Stopping: {HostAddress}", HostAddress);

            await base.StopSupervisor(context).ConfigureAwait(false);

            var shutdownAsync = _server.ShutdownAsync();

            await shutdownAsync.ConfigureAwait(false);

            await _messageFabric.DisposeAsync().ConfigureAwait(false);
        }
    }
}
