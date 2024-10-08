#nullable enable
namespace MassTransit.Clients
{
    using System;


    public class ReceiveEndpointClientFactoryContext :
        ClientFactoryContext
    {
        readonly HostReceiveEndpointHandle _handle;
        readonly IReceiveEndpoint _receiveEndpoint;

        public ReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle handle, RequestTimeout defaultTimeout = default)
        {
            _handle = handle;
            _receiveEndpoint = handle.ReceiveEndpoint;

            ResponseAddress = _receiveEndpoint.InputAddress;

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
            return new ReceiveEndpointPublishRequestSendEndpoint<T>(_handle, consumeContext);
        }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class
        {
            return new ReceiveEndpointSendRequestSendEndpoint<T>(_handle, destinationAddress, consumeContext);
        }

        public RequestTimeout DefaultTimeout { get; }
    }
}
