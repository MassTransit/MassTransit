// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;


    [Obsolete("This will be deprecated in the next release")]
    public class MessageRequestClientFactory<TRequest, TResponse> :
        IRequestClientFactory<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly Action<SendContext<TRequest>> _callback;
        readonly IAsyncDisposable _context;
        readonly Uri _destinationAddress;
        readonly IClientFactory _clientFactory;
        readonly TimeSpan? _timeToLive;

        public MessageRequestClientFactory(IAsyncDisposable context, IClientFactory clientFactory, Uri destinationAddress, TimeSpan? timeToLive,
            Action<SendContext<TRequest>> callback)
        {
            _context = context;
            _clientFactory = clientFactory;
            _destinationAddress = destinationAddress;
            _timeToLive = timeToLive;
            _callback = callback;
        }

        IRequestClient<TRequest, TResponse> IRequestClientFactory<TRequest, TResponse>.CreateRequestClient(ConsumeContext consumeContext,
            TimeSpan? timeout, TimeSpan? timeToLive, Action<SendContext<TRequest>> callback)
        {
            Action<SendContext<TRequest>> actualCallback = null;
            if (_callback != null)
            {
                if (callback != null)
                    actualCallback = x =>
                    {
                        _callback(x);
                        callback(x);
                    };
                else
                    actualCallback = _callback;
            }
            else if (callback != null)
                actualCallback = callback;

            RequestTimeout requestTimeout = timeout ?? RequestTimeout.None;

            var client = _destinationAddress == null
                ? _clientFactory.CreateRequestClient<TRequest>(consumeContext, requestTimeout)
                : _clientFactory.CreateRequestClient<TRequest>(consumeContext, _destinationAddress, requestTimeout);

            return new MessageRequestClient<TRequest, TResponse>(client, timeToLive ?? _timeToLive, actualCallback);
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _context.DisposeAsync(cancellationToken);
        }
    }
}
