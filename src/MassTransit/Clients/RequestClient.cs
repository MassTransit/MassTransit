// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.Clients
{
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class RequestClient<TRequest> :
        IRequestClient<TRequest>
        where TRequest : class
    {
        readonly ClientFactoryContext _context;
        readonly IRequestSendEndpoint _requestSendEndpoint;
        readonly RequestTimeout _timeout;

        public RequestClient(ClientFactoryContext context, IRequestSendEndpoint requestSendEndpoint, RequestTimeout timeout)
        {
            _context = context;
            _requestSendEndpoint = requestSendEndpoint;
            _timeout = timeout;
        }

        public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, message, cancellationToken, timeout.Or(_timeout));
        }

        public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
        {
            var message = TypeMetadataCache<TRequest>.InitializeFromObject(values);

            return Create(message, cancellationToken, timeout);
        }

        public async Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            using (RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, message, cancellationToken,
                timeout.Or(_timeout)))
            {
                return await handle.GetResponse<T>().ConfigureAwait(false);
            }
        }

        public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            var message = TypeMetadataCache<TRequest>.InitializeFromObject(values);

            return GetResponse<T>(message, cancellationToken, timeout);
        }

        public async Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T1 : class
            where T2 : class
        {
            using (RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, _requestSendEndpoint, message, cancellationToken,
                timeout.Or(_timeout)))
            {
                Task<Response<T1>> result1 = handle.GetResponse<T1>(false);
                Task<Response<T2>> result2 = handle.GetResponse<T2>();

                await Task.WhenAny(result1, result2).ConfigureAwait(false);

                return (result1, result2);
            }
        }

        public Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            var message = TypeMetadataCache<TRequest>.InitializeFromObject(values);

            return GetResponse<T1, T2>(message, cancellationToken, timeout);
        }
    }
}