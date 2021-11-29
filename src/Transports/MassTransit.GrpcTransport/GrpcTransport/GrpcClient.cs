namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts;
    using Grpc.Core;
    using MassTransit.Middleware;
    using RetryPolicies;


    public class GrpcClient :
        Agent,
        IGrpcClient
    {
        readonly TransportService.TransportServiceClient _client;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly IGrpcHostNode _hostNode;
        readonly IGrpcNode _node;
        readonly Task _runTask;

        public GrpcClient(IGrpcHostConfiguration hostConfiguration, IGrpcHostNode hostNode, TransportService.TransportServiceClient client, IGrpcNode node)
        {
            _hostConfiguration = hostConfiguration;
            _hostNode = hostNode;
            _client = client;
            _node = node;

            _runTask = Task.Run(() => RunAsync());
        }

        async Task RunAsync()
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            try
            {
                LogContext.Info?.Log("gRPC Connect: {Server}", _node.NodeAddress);

                await _hostConfiguration.ReceiveTransportRetryPolicy.Retry(async () =>
                {
                    try
                    {
                        AsyncDuplexStreamingCall<TransportMessage, TransportMessage> eventStream = _client.EventStream(cancellationToken: Stopping);

                        await SendJoin(eventStream.RequestStream).ConfigureAwait(false);

                        SetReady(_node.Ready);

                        await _node.Connect(eventStream.RequestStream, eventStream.ResponseStream, Stopping).ConfigureAwait(false);
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
                Join = new Join { Node = new Node().Initialize(_hostNode) }
            };

            await requestStream.WriteAsync(message).ConfigureAwait(false);

            _node.LogSent(message);
        }
    }
}
