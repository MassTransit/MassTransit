namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts;
    using Grpc.Core;
    using MassTransit.Middleware;


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

            var stoppingContext = new ClientStoppingContext(Stopping);

            RetryPolicyContext<ClientStoppingContext> policyContext = _hostConfiguration.ReceiveTransportRetryPolicy.CreatePolicyContext(stoppingContext);

            try
            {
                RetryContext<ClientStoppingContext> retryContext = null;

                while (!IsStopping)
                {
                    try
                    {
                        if (retryContext?.Delay != null)
                            await Task.Delay(retryContext.Delay.Value, Stopping).ConfigureAwait(false);

                        LogContext.Info?.Log("gRPC Connect: {Server}", _node.NodeAddress);

                        using AsyncDuplexStreamingCall<TransportMessage, TransportMessage> eventStream = _client.EventStream(cancellationToken: Stopping);

                        await SendJoin(eventStream.RequestStream).ConfigureAwait(false);

                        LogContext.Info?.Log("gRPC Connected: {Server}", _node.NodeAddress);

                        SetReady(_node.Ready);

                        await _node.Connect(eventStream.RequestStream, eventStream.ResponseStream, Stopping).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (RpcException exception) when (exception.StatusCode == StatusCode.Cancelled)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        LogContext.Warning?.Log(exception, "gRPC Connection Faulted: {Server}", _node.NodeAddress);

                        if (retryContext != null)
                        {
                            retryContext = retryContext.CanRetry(exception, out RetryContext<ClientStoppingContext> nextRetryContext)
                                ? nextRetryContext
                                : null;
                        }

                        if (retryContext == null && !policyContext.CanRetry(exception, out retryContext))
                            break;
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                if (exception.CancellationToken != Stopping)
                    LogContext.Debug?.Log(exception, "gRPC Client Canceled: {Server}", _node.NodeAddress);
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.Cancelled)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "gRPC Client Faulted: {Server}", _node.NodeAddress);

                SetNotReady(exception);
            }
            finally
            {
                LogContext.Debug?.Log("gRPC Client Exiting: {Server}", _node.NodeAddress);

                policyContext.Dispose();
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


        class ClientStoppingContext :
            BasePipeContext
        {
            public ClientStoppingContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
