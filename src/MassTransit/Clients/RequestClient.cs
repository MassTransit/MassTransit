namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;


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
            return new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, Task.FromResult(message), cancellationToken, timeout.Or(_timeout));
        }

        public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            Task<InitializeContext<TRequest>> request = _requestSendEndpoint.CreateMessage(values, cancellationToken);

            async Task<TRequest> Message()
            {
                var message = await request.ConfigureAwait(false);

                return message.Message;
            }

            return new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, Message(), cancellationToken, timeout.Or(_timeout));
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return GetResponse<T>(Task.FromResult(message), cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            Task<InitializeContext<TRequest>> request = _requestSendEndpoint.CreateMessage(values, cancellationToken);

            async Task<TRequest> Message()
            {
                var message = await request.ConfigureAwait(false);

                return message.Message;
            }

            return GetResponse<T>(Message(), cancellationToken, timeout);
        }

        async Task<Response<T>> GetResponse<T>(Task<TRequest> message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            using (RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, message, cancellationToken,
                timeout.Or(_timeout)))
            {
                return await handle.GetResponse<T>().ConfigureAwait(false);
            }
        }

        public Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            return GetResponse<T1, T2>(Task.FromResult(message), cancellationToken, timeout);
        }

        public Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            Task<InitializeContext<TRequest>> request = _requestSendEndpoint.CreateMessage(values, cancellationToken);

            async Task<TRequest> Message()
            {
                var message = await request.ConfigureAwait(false);

                return message.Message;
            }


            return GetResponse<T1, T2>(Message(), cancellationToken, timeout);
        }

        async Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(Task<TRequest> message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            using (RequestHandle<TRequest> handle =
                new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, message, cancellationToken, timeout.Or(_timeout)))
            {
                Task<Response<T1>> result1 = handle.GetResponse<T1>(false);
                Task<Response<T2>> result2 = handle.GetResponse<T2>();

                await Task.WhenAny(result1, result2).ConfigureAwait(false);

                return (result1, result2);
            }
        }
    }
}
