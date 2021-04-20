namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using Contracts;
    using GreenPipes.Agents;
    using Grpc.Core;
    using Policies;


    public class GrpcClient :
        Agent,
        IGrpcClient
    {
        readonly TransportService.TransportServiceClient _client;
        readonly NodeContext _context;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly INodeCollection _nodeCollection;
        readonly Task _runTask;

        public GrpcClient(IGrpcHostConfiguration hostConfiguration, Uri nodeAddress, TransportService.TransportServiceClient client,
            INodeCollection nodeCollection)
        {
            _hostConfiguration = hostConfiguration;
            _client = client;
            _nodeCollection = nodeCollection;

            _context = new GrpcClientNodeContext(nodeAddress);

            _runTask = Task.Run(() => RunAsync());
        }

        async Task RunAsync()
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            try
            {
                LogContext.Info?.Log("gRPC Connect: {Server}", _context.NodeAddress);

                await _hostConfiguration.ReceiveTransportRetryPolicy.Retry(async () =>
                {
                    try
                    {
                        AsyncDuplexStreamingCall<TransportMessage, TransportMessage> eventStream = _client.EventStream(cancellationToken: Stopping);

                        var node = _nodeCollection.GetNode(_context);

                        await SendJoin(eventStream.RequestStream).ConfigureAwait(false);

                        SetReady(node.Ready);

                        await node.Connect(eventStream.RequestStream, eventStream.ResponseStream, Stopping).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        throw new ConnectionException("gRPC Connection Faulted", exception);
                    }
                }, Stopping).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                SetNotReady(exception);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            await _runTask.ConfigureAwait(false);

            await base.StopAgent(context);
        }

        async Task SendJoin(IAsyncStreamWriter<TransportMessage> requestStream)
        {
            var message = new TransportMessage
            {
                MessageId = NewId.NextGuid().ToString(),
                Join = new Join {Node = new Node().Initialize(_nodeCollection.HostNode)}
            };

            await requestStream.WriteAsync(message).ConfigureAwait(false);

            _context.LogSent(message);
        }
    }
}
