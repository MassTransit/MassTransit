#nullable enable
namespace MassTransit.Clients
{
    using System;


    public class ReceiveEndpointClientFactoryContext :
        ClientFactoryContext
    {
        readonly IReceiveEndpoint _receiveEndpoint;

        public ReceiveEndpointClientFactoryContext(ReceiveEndpointReady receiveEndpointReady, RequestTimeout defaultTimeout = default)
        {
            _receiveEndpoint = receiveEndpointReady.ReceiveEndpoint;

            ResponseAddress = receiveEndpointReady.InputAddress;
            DefaultTimeout = defaultTimeout.Or(RequestTimeout.Default);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receiveEndpoint.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _receiveEndpoint.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receiveEndpoint.ConnectRequestPipe(requestId, pipe);
        }

        public Uri ResponseAddress { get; }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext? consumeContext = default)
            where T : class
        {
            return new PublishRequestSendEndpoint<T>(_receiveEndpoint, consumeContext);
        }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class
        {
            return new SendRequestSendEndpoint<T>(_receiveEndpoint, destinationAddress, consumeContext);
        }

        public RequestTimeout DefaultTimeout { get; }
    }
}
