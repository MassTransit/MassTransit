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
    using System.Threading.Tasks;


    public class RequestClient<TRequest> :
        IRequestClient<TRequest>
        where TRequest : class
    {
        readonly ClientFactoryContext _context;
        readonly Uri _destinationAddress;
        readonly Timeout _timeout;

        public RequestClient(ClientFactoryContext context, Uri destinationAddress, Timeout timeout)
        {
            _context = context;
            _destinationAddress = destinationAddress;
            _timeout = timeout;
        }

        public RequestHandle<TRequest> Send(TRequest message, CancellationToken cancellationToken, Timeout timeout)
        {
            return new ClientRequestHandle<TRequest>(_context, _destinationAddress, message, cancellationToken, timeout);
        }

        public async Task<Result<T>> GetResult<T>(TRequest message, CancellationToken cancellationToken)
            where T : class
        {
            using (RequestHandle<TRequest> handle = new ClientRequestHandle<TRequest>(_context, _destinationAddress, message, cancellationToken, _timeout))
            {
                return await handle.GetResult<T>().ConfigureAwait(false);
            }
        }

        public Task<(Result<T1>, Result<T2>)> GetResult<T1, T2>(TRequest message, CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            throw new NotImplementedException();
        }
    }
}