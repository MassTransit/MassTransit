namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts;
    using Fabric;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Internals;
    using MassTransit.Middleware;
    using Metadata;
    using Transports;
    using Transports.Fabric;


    public sealed class GrpcTransportProvider :
        Supervisor,
        IGrpcTransportProvider
    {
        const int MaxMessageLengthBytes = int.MaxValue;
        readonly IList<IGrpcClient> _clients;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly GrpcHostNode _hostNode;
        readonly IMessageFabric<NodeContext, GrpcTransportMessage> _messageFabric;
        readonly NodeCollection _nodeCollection;
        readonly Server _server;
        readonly Lazy<Task> _startupTask;
        readonly IGrpcTopologyConfiguration _topologyConfiguration;

        public GrpcTransportProvider(IGrpcHostConfiguration hostConfiguration, IGrpcTopologyConfiguration topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _messageFabric = new MessageFabric<NodeContext, GrpcTransportMessage>();

            _nodeCollection = new NodeCollection(this, _messageFabric);
            _clients = new List<IGrpcClient>();

            var transport = new GrpcTransportService(this, _hostConfiguration, _nodeCollection);

            _server = new Server(GetChannelOptions())
            {
                Services = { TransportService.BindService(transport) },
                Ports = { new ServerPort(hostConfiguration.BaseAddress.Host, hostConfiguration.BaseAddress.Port, ServerCredentials.Insecure) }
            };

            var serverPort = _server.Ports.First();

            HostAddress = new UriBuilder(_hostConfiguration.BaseAddress)
            {
                Host = serverPort.Host,
                Port = serverPort.BoundPort
            }.Uri;

            HostNodeContext = new HostNodeContext(HostAddress);

            _hostNode = new GrpcHostNode(_messageFabric, HostNodeContext);

            var observer = new NodeMessageFabricObserver(_nodeCollection, _hostNode);

            _messageFabric.ConnectMessageFabricObserver(observer);

            _startupTask = new Lazy<Task>(() => Task.Run(() => Startup()));
        }

        public IGrpcHostNode HostNode => _hostNode;
        public Task StartupTask => _startupTask.Value;

        public Uri HostAddress { get; }
        public NodeContext HostNodeContext { get; }

        public IMessageFabric<NodeContext, GrpcTransportMessage> MessageFabric => _messageFabric;

        public Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new GrpcEndpointAddress(HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            IMessageExchange<GrpcTransportMessage> exchange = _messageFabric.GetExchange(HostNodeContext, endpointAddress.Name, endpointAddress.ExchangeType);

            var transportContext = new ExchangeGrpcSendTransportContext(_hostConfiguration, receiveEndpointContext, exchange);

            return Task.FromResult<ISendTransport>(new GrpcSendTransport(transportContext));
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new GrpcEndpointAddress(HostAddress, address);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext receiveEndpointContext, Uri publishAddress)
            where T : class
        {
            IGrpcMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            publishTopology.Apply(new MessageFabricPublishTopologyBuilder<NodeContext, GrpcTransportMessage>(HostNodeContext, _messageFabric));

            return CreateSendTransport(receiveEndpointContext, publishAddress);
        }

        public void Probe(ProbeContext context)
        {
            _messageFabric.Probe(context);
        }

        IGrpcClient GetClient(Uri address)
        {
            var channel = HostMetadataCache.IsNetFramework
                ? (ChannelBase)new Channel(address.Host, address.Port, ChannelCredentials.Insecure)
                : GrpcChannel.ForAddress(address.GetLeftPart(UriPartial.Authority), new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure,
                    MaxReceiveMessageSize = MaxMessageLengthBytes,
                });

            var client = new TransportService.TransportServiceClient(channel);

            var clientNodeContext = new ClientNodeContext(address);

            var node = _nodeCollection.GetNode(clientNodeContext);

            return new GrpcClient(_hostConfiguration, _hostNode, client, node);
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

            await _messageFabric.Stop(context).ConfigureAwait(false);
        }
    }
}
