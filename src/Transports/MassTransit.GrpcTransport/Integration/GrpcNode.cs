namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Contracts;
    using Fabric;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Grpc.Core;


    public sealed class GrpcNode :
        Agent,
        IGrpcNode
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Channel<TransportMessage> _channel;
        readonly RemoteNodeTopology _remoteTopology;
        readonly HostNodeTopology _hostTopology;
        readonly IMessageFabric _messageFabric;
        NodeContext _context;

        public GrpcNode(IMessageFabric messageFabric, NodeContext context)
        {
            _messageFabric = messageFabric;
            _context = context;

            _cancellationTokenSource = new CancellationTokenSource();

            _hostTopology = new HostNodeTopology();

            _remoteTopology = new RemoteNodeTopology(this, messageFabric);

            var outputOptions = new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = true
            };

            _channel = System.Threading.Channels.Channel.CreateUnbounded<TransportMessage>(outputOptions);

            Writer = _channel.Writer;

            if (context.NodeType == NodeType.Host)
                SetReady();
        }

        public ChannelWriter<TransportMessage> Writer { get; }

        public NodeType NodeType => _context.NodeType;
        public Uri NodeAddress => _context.NodeAddress;
        public Guid SessionId => _context.SessionId;

        public HostInfo Host
        {
            get => _context.Host;
            set => _context.Host = value;
        }

        public async Task Connect(IAsyncStreamWriter<TransportMessage> writer, IAsyncStreamReader<TransportMessage> reader, CancellationToken cancellationToken)
        {
            if (_context.NodeType == NodeType.Server)
                SetReady();

            using var source = CancellationTokenSource.CreateLinkedTokenSource(Stopping, cancellationToken);

            var writerTask = StartWriter(writer, source.Token);
            var readerTask = StartReader(reader, source.Token);

            await Task.WhenAll(readerTask, writerTask).ConfigureAwait(false);
        }

        public TopologyHandle AddTopology(Topology topology, TopologyHandle handle)
        {
            return _hostTopology.Add(topology, handle);
        }

        public IEnumerable<Topology> GetTopology()
        {
            return _hostTopology.GetTopology();
        }

        public void Join(NodeContext context, IEnumerable<Topology> topologies)
        {
            if (context.SessionId != SessionId)
                _context = context;

            _remoteTopology.Join(context.SessionId, topologies);
        }

        protected override async Task StopAgent(StopContext context)
        {
            _channel.Writer.Complete();

            await _channel.Reader.Completion.ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);
        }

        async Task StartWriter(IAsyncStreamWriter<TransportMessage> writer, CancellationToken cancellationToken)
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var message = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(message.MessageId))
                        message.MessageId = NewId.NextGuid().ToString();

                    await writer.WriteAsync(message).ConfigureAwait(false);

                    _context.LogSent(message);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Writer faulted: {InstanceAddress}", NodeAddress);
            }
        }

        async Task StartReader(IAsyncStreamReader<TransportMessage> reader, CancellationToken cancellationToken)
        {
            try
            {
                while (await reader.MoveNext(CancellationToken.None).OrCanceled(cancellationToken).ConfigureAwait(false))
                {
                    var message = reader.Current;

                    DispatchMessageAsync(message);

                    _context.LogReceived(message);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (RpcException exception) when (exception.Status.StatusCode == StatusCode.Cancelled)
            {
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Reader faulted: {InstanceAddress}", NodeAddress);
            }
        }

        void DispatchMessageAsync(TransportMessage message)
        {
            Task.Run(() => ProcessMessage(message), _cancellationTokenSource.Token);
        }

        async Task ProcessMessage(TransportMessage message)
        {
            try
            {
                if (message.ContentCase == TransportMessage.ContentOneofCase.Join)
                    LogContext.Warning?.Log("Join is not allowed on a connected instance: {InstanceId}", NodeAddress);
                else if (message.ContentCase == TransportMessage.ContentOneofCase.Welcome)
                {
                    _context.Host = new DictionaryHostInfo(message.Welcome.Node.Host);

                    Guid.TryParse(message.Welcome.Node.SessionId, out var sessionId);

                    _remoteTopology.Join(sessionId, message.Welcome.Node.Topology);

                    if (_context.NodeType == NodeType.Client)
                        SetReady();
                }
                else if (message.ContentCase == TransportMessage.ContentOneofCase.Topology)
                    _remoteTopology.ProcessTopology(message.Topology);
                else if (message.ContentCase == TransportMessage.ContentOneofCase.Deliver)
                    await DeliverMessage(message).ConfigureAwait(false);
                else
                    LogContext.Warning?.Log("Unsupported message received: {MessageType} on {Instance}", message.ContentCase, NodeAddress);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Message Faulted: {MessageId}", message.MessageId);
            }
        }

        Task DeliverMessage(TransportMessage message)
        {
            var transportMessage = new GrpcTransportMessage(message, _context.Host);

            switch (message.Deliver.DestinationCase)
            {
                case Deliver.DestinationOneofCase.Exchange:
                {
                    var destination = message.Deliver.Exchange;

                    var exchange = _messageFabric.GetExchange(_context, destination.Name);

                    return exchange.Send(transportMessage, Stopping);
                }
                case Deliver.DestinationOneofCase.Queue:
                {
                    var queue = _messageFabric.GetQueue(_context, message.Deliver.Queue.Name);

                    return queue.Send(transportMessage, Stopping);
                }
                case Deliver.DestinationOneofCase.Receiver:
                {
                    var receiver = message.Deliver.Receiver;

                    var queue = _messageFabric.GetQueue(_context, receiver.QueueName);

                    message.Deliver.Receiver.ReceiverId = _remoteTopology.GetLocalConsumerId(receiver.QueueName, receiver.ReceiverId);

                    return queue.Send(transportMessage, Stopping);
                }
                default:
                    return Task.CompletedTask;
            }
        }
    }
}
