namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts;
    using Grpc.Core;
    using Internals;


    public class GrpcTransportService :
        TransportService.TransportServiceBase
    {
        readonly INodeCollection _collection;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly IGrpcTransportProvider _transportProvider;

        public GrpcTransportService(IGrpcTransportProvider transportProvider, IGrpcHostConfiguration hostConfiguration, INodeCollection collection)
        {
            _transportProvider = transportProvider;
            _hostConfiguration = hostConfiguration;
            _collection = collection;
        }

        public override async Task EventStream(IAsyncStreamReader<TransportMessage> requestStream, IServerStreamWriter<TransportMessage> responseStream,
            ServerCallContext context)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            try
            {
                var ready = await requestStream.MoveNext(CancellationToken.None).OrCanceled(context.CancellationToken).ConfigureAwait(false);
                if (ready)
                {
                    var message = requestStream.Current;
                    if (message.ContentCase == TransportMessage.ContentOneofCase.Join)
                    {
                        var joinNode = message.Join.Node;

                        var nodeAddress = new Uri(joinNode.Address);

                        Guid.TryParse(joinNode.SessionId, out var sessionId);

                        var nodeContext = new ServerNodeContext(context, nodeAddress, sessionId, joinNode.Host);

                        nodeContext.LogReceived(message);

                        var node = _collection.GetNode(nodeContext);

                        node.Join(nodeContext, joinNode.Topology);

                        await node.SendWelcome(_transportProvider.HostNode).ConfigureAwait(false);

                        await node.Connect(responseStream, requestStream, context.CancellationToken).ConfigureAwait(false);

                        nodeContext.LogDisconnect();
                    }
                }
                else
                    LogContext.Warning?.Log("GRPC no content received: {Address}", context.Peer);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Connection {NodeAddress} faulted", context.Peer);
            }
        }
    }
}
