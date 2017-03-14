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
    using Pipeline;


    public class MessageRequestClientFactory<TRequest, TResponse> :
        IRequestClientFactory<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly Action<SendContext<TRequest>> _callback;
        readonly IRequestPipeConnector _connector;
        readonly Uri _destinationAddress;
        readonly HostReceiveEndpointHandle _endpointHandle;
        readonly Uri _responseAddress;
        readonly TimeSpan _timeout;
        readonly TimeSpan? _timeToLive;

        public MessageRequestClientFactory(HostReceiveEndpointHandle endpointHandle, IRequestPipeConnector connector, Uri responseAddress,
            Uri destinationAddress, TimeSpan timeout, TimeSpan? timeToLive,
            Action<SendContext<TRequest>> callback)
        {
            _endpointHandle = endpointHandle;
            _connector = connector;
            _responseAddress = responseAddress;
            _destinationAddress = destinationAddress;
            _timeout = timeout;
            _timeToLive = timeToLive;
            _callback = callback;
        }

        public IRequestClient<TRequest, TResponse> CreateRequestClient(ISendEndpointProvider sendEndpointProvider, TimeSpan? timeout = null,
            TimeSpan? timeToLive = null, Action<SendContext<TRequest>> callback = null)
        {
            return new MessageRequestClient<TRequest, TResponse>(sendEndpointProvider, _connector, _responseAddress, _destinationAddress, timeout ?? _timeout,
                timeToLive ?? _timeToLive, context =>
                {
                    _callback?.Invoke(context);

                    callback?.Invoke(context);
                });
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return _endpointHandle.StopAsync(cancellationToken);
        }
    }
}