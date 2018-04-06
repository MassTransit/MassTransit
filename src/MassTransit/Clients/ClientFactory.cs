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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class ClientFactory :
        IClientFactory
    {
        readonly ClientFactoryContext _context;

        public ClientFactory(ClientFactoryContext context)
        {
            _context = context;
        }

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(timeout);

            return client.Create(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(destinationAddress, timeout);

            return client.Create(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(consumeContext, timeout);

            return client.Create(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T : class
        {
            IRequestClient<T> client = CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

            return client.Create(message, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            if (EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress))
                return CreateRequestClient<T>(destinationAddress, timeout);

            return new RequestClient<T>(_context, new PublishRequestSendEndpoint(_context.PublishEndpoint), timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
            where T : class
        {
            if (EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress))
                return CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

            return new RequestClient<T>(_context, new PublishRequestSendEndpoint(consumeContext), timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            var requestSendEndpoint = new SendRequestSendEndpoint(_context, destinationAddress);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            var requestSendEndpoint = new SendRequestSendEndpoint(consumeContext, destinationAddress);

            return new RequestClient<T>(_context, requestSendEndpoint, timeout);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_context is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync(cancellationToken);

            return TaskUtil.Completed;
        }
    }
}