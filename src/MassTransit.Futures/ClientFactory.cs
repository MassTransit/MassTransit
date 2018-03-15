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
namespace MassTransit
{
    using System;
    using System.Threading;
    using Util;


    public class ClientFactory :
        IClientFactory
    {
        readonly ClientFactoryContext _context;

        public ClientFactory(ClientFactoryContext context)
        {
            _context = context;
        }

        public RequestHandle<T> SendRequest<T>(T message, CancellationToken cancellationToken, Timeout timeout)
            where T : class
        {
            var client = CreateRequestClient(message, timeout);

            return client.Send(message, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(T message, Timeout timeout)
            where T : class
        {
            if (!EndpointConvention.TryGetDestinationAddress(message, out var destinationAddress))
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");

            return new RequestClient<T>(_context, destinationAddress, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, Timeout timeout = default)
            where T : class
        {
            return new RequestClient<T>(_context, destinationAddress, timeout);
        }
    }
}