#nullable enable
namespace MassTransit.Mediator.Contexts
{
    using System;
    using Transports;


    public class MediatorClientFactoryContext :
        ClientFactoryContext
    {
        readonly IConsumePipe _connector;
        readonly ISendEndpoint _endpoint;

        public MediatorClientFactoryContext(ISendEndpoint endpoint, IConsumePipe connector, Uri responseAddress, RequestTimeout defaultTimeout = default)
        {
            _endpoint = endpoint;
            _connector = connector;

            ResponseAddress = responseAddress;
            DefaultTimeout = defaultTimeout.Or(RequestTimeout.Default);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _connector.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _connector.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _connector.ConnectRequestPipe(requestId, pipe);
        }

        public Uri ResponseAddress { get; }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext? consumeContext = default)
            where T : class
        {
            return new MediatorRequestSendEndpoint<T>(_endpoint, consumeContext);
        }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class
        {
            return new MediatorRequestSendEndpoint<T>(_endpoint, consumeContext);
        }

        public RequestTimeout DefaultTimeout { get; }
    }
}
