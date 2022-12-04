#nullable enable
namespace MassTransit.DependencyInjection
{
    using System;


    public class ScopedClientFactoryContext :
        ClientFactoryContext
    {
        readonly IClientFactory _clientFactory;
        readonly IServiceProvider _serviceProvider;

        public ScopedClientFactoryContext(IClientFactory clientFactory, IServiceProvider serviceProvider)
        {
            _clientFactory = clientFactory;
            _serviceProvider = serviceProvider;
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
            return new ScopedRequestSendEndpoint<T>(endpoint, _serviceProvider);
        }

        public IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class
        {
            IRequestSendEndpoint<T> endpoint = _clientFactory.Context.GetRequestEndpoint<T>(destinationAddress, consumeContext);
            return new ScopedRequestSendEndpoint<T>(endpoint, _serviceProvider);
        }
    }
}
