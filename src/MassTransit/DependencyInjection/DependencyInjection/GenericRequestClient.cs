namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;


    public class GenericRequestClient<TRequest> :
        IRequestClient<TRequest>
        where TRequest : class
    {
        readonly IRequestClient<TRequest> _client;

        public GenericRequestClient(IServiceProvider provider)
        {
            _client = GetRequestClient(provider);
        }

        public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _client.Create(message, cancellationToken, timeout);
        }

        public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _client.Create(values, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _client.GetResponse<T>(message, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _client.GetResponse<T>(message, callback, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _client.GetResponse<T>(values, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _client.GetResponse<T>(values, callback, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return _client.GetResponse<T1, T2>(message, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return _client.GetResponse<T1, T2>(message, callback, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return _client.GetResponse<T1, T2>(values, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return _client.GetResponse<T1, T2>(values, callback, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return _client.GetResponse<T1, T2, T3>(message, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return _client.GetResponse<T1, T2, T3>(message, callback, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return _client.GetResponse<T1, T2, T3>(values, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return _client.GetResponse<T1, T2, T3>(values, callback, cancellationToken, timeout);
        }

        IRequestClient<TRequest> GetRequestClient(IServiceProvider provider)
        {
            var consumeContext = provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

            var clientFactory = provider.GetService<IScopedClientFactory>();
            if (clientFactory != null)
                return clientFactory.CreateRequestClient<TRequest>();

            var mediator = provider.GetService<IMediator>();
            if (mediator != null)
            {
                return consumeContext != null
                    ? mediator.CreateRequestClient<TRequest>(consumeContext)
                    : new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(mediator, provider)).CreateRequestClient<TRequest>(default);
            }

            throw new MassTransitException($"Unable to resolve client factory or mediator for request client: {TypeCache<TRequest>.ShortName}");
        }
    }
}
