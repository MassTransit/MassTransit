namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class RequestClient<TRequest> :
        IRequestClient<TRequest>
        where TRequest : class
    {
        readonly ClientFactoryContext _context;
        readonly IRequestSendEndpoint<TRequest> _requestSendEndpoint;
        readonly RequestTimeout _timeout;

        public RequestClient(ClientFactoryContext context, IRequestSendEndpoint<TRequest> requestSendEndpoint, RequestTimeout timeout)
        {
            _context = context;
            _requestSendEndpoint = requestSendEndpoint;
            _timeout = timeout;
        }

        public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                await _requestSendEndpoint.Send(requestId, message, pipe, token).ConfigureAwait(false);

                return message;
            }

            return new ClientRequestHandle<TRequest>(_context, Request, cancellationToken, timeout.Or(_timeout));
        }

        public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                return await _requestSendEndpoint.Send(requestId, values, pipe, token).ConfigureAwait(false);
            }

            return new ClientRequestHandle<TRequest>(_context, Request, cancellationToken, timeout.Or(_timeout));
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return GetResponse<T>(message, null, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                await _requestSendEndpoint.Send(requestId, message, pipe, token).ConfigureAwait(false);

                return message;
            }

            return GetResponseInternal<T>(Request, cancellationToken, timeout, callback);
        }

        public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return GetResponse<T>(values, null, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                return await _requestSendEndpoint.Send(requestId, values, pipe, token).ConfigureAwait(false);
            }

            return GetResponseInternal<T>(Request, cancellationToken, timeout, callback);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return GetResponse<T1, T2>(message, null, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken, RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                await _requestSendEndpoint.Send(requestId, message, pipe, token).ConfigureAwait(false);

                return message;
            }

            return GetResponseInternal<T1, T2>(Request, cancellationToken, timeout, callback);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            return GetResponse<T1, T2>(values, null, cancellationToken, timeout);
        }

        public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                return await _requestSendEndpoint.Send(requestId, values, pipe, token).ConfigureAwait(false);
            }

            return GetResponseInternal<T1, T2>(Request, cancellationToken, timeout, callback);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return GetResponse<T1, T2, T3>(message, null, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                await _requestSendEndpoint.Send(requestId, message, pipe, token).ConfigureAwait(false);

                return message;
            }

            return GetResponseInternal<T1, T2, T3>(Request, cancellationToken, timeout, callback);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return GetResponse<T1, T2, T3>(values, null, cancellationToken, timeout);
        }

        public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            async Task<TRequest> Request(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken token)
            {
                return await _requestSendEndpoint.Send(requestId, values, pipe, token).ConfigureAwait(false);
            }

            return GetResponseInternal<T1, T2, T3>(Request, cancellationToken, timeout, callback);
        }

        async Task<Response<T>> GetResponseInternal<T>(ClientRequestHandle<TRequest>.SendRequestCallback request,
            CancellationToken cancellationToken, RequestTimeout timeout, RequestPipeConfiguratorCallback<TRequest> callback = null)
            where T : class
        {
            using RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, request, cancellationToken, timeout.Or(_timeout));

            callback?.Invoke(handle);

            return await handle.GetResponse<T>().ConfigureAwait(false);
        }

        async Task<Response<T1, T2>> GetResponseInternal<T1, T2>(ClientRequestHandle<TRequest>.SendRequestCallback request, CancellationToken cancellationToken,
            RequestTimeout timeout, RequestPipeConfiguratorCallback<TRequest> callback = null)
            where T1 : class
            where T2 : class
        {
            using RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, request, cancellationToken, timeout.Or(_timeout));

            callback?.Invoke(handle);

            Task<Response<T1>> result1 = handle.GetResponse<T1>(false);
            Task<Response<T2>> result2 = handle.GetResponse<T2>();

            var task = await Task.WhenAny(result1, result2).ConfigureAwait(false);

            await task.ConfigureAwait(false);

            return new Response<T1, T2>(result1, result2);
        }

        async Task<Response<T1, T2, T3>> GetResponseInternal<T1, T2, T3>(ClientRequestHandle<TRequest>.SendRequestCallback request, CancellationToken
            cancellationToken, RequestTimeout timeout, RequestPipeConfiguratorCallback<TRequest> callback = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            using RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, request, cancellationToken, timeout.Or(_timeout));

            callback?.Invoke(handle);

            Task<Response<T1>> result1 = handle.GetResponse<T1>(false);
            Task<Response<T2>> result2 = handle.GetResponse<T2>(false);
            Task<Response<T3>> result3 = handle.GetResponse<T3>();

            var task = await Task.WhenAny(result1, result2, result3).ConfigureAwait(false);

            await task.ConfigureAwait(false);

            return new Response<T1, T2, T3>(result1, result2, result3);
        }
    }
}
