#nullable enable
namespace MassTransit.DependencyInjection
{
    using System;


    public class ScopedClientFactoryContext<TScope> :
        ClientFactoryContext
        where TScope : class
    {
        readonly IClientFactory _clientFactory;
        readonly TScope _scope;

        public ScopedClientFactoryContext(IClientFactory clientFactory, TScope scope)
        {
            _clientFactory = clientFactory;
            _scope = scope;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _clientFactory.Context.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _clientFactory.Context.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _clientFactory.Context.ConnectRequestPipe(requestId, pipe);
        }

        public RequestTimeout DefaultTimeout => _clientFactory.Context.DefaultTimeout;

        public Uri ResponseAddress => _clientFactory.Context.ResponseAddress;

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext? consumeContext = default)
            where T : class
        {
            IRequestSendEndpoint<T> endpoint = _clientFactory.Context.GetRequestEndpoint<T>(consumeContext);
            return new ScopedRequestSendEndpoint<TScope, T>(endpoint, _scope);
        }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class
        {
            IRequestSendEndpoint<T> endpoint = _clientFactory.Context.GetRequestEndpoint<T>(destinationAddress, consumeContext);
            return new ScopedRequestSendEndpoint<TScope, T>(endpoint, _scope);
        }
    }
}
