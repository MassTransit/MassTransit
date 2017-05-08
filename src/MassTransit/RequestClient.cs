// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using GreenPipes;
    using Pipeline;


    public abstract class RequestClient<TRequest, TResponse> :
        IRequestClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly Action<SendContext<TRequest>> _callback;
        readonly IRequestPipeConnector _connector;
        readonly Uri _responseAddress;
        readonly TimeSpan _timeout;
        readonly TimeSpan? _timeToLive;

        protected RequestClient(IRequestPipeConnector connector, Uri responseAddress, TimeSpan timeout, TimeSpan? timeToLive = default(TimeSpan?),
            Action<SendContext<TRequest>> callback = null)
        {
            _connector = connector;
            _responseAddress = responseAddress;
            _timeout = timeout;
            _timeToLive = timeToLive;
            _callback = callback;
        }

        public async Task<TResponse> Request(TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var taskScheduler = SynchronizationContext.Current == null
                ? TaskScheduler.Default
                : TaskScheduler.FromCurrentSynchronizationContext();

            Task<TResponse> responseTask = null;
            var pipe = new SendRequest<TRequest>(_connector, _responseAddress, taskScheduler, cfg =>
            {
                cfg.TimeToLive = _timeToLive;
                cfg.Timeout = _timeout;
                responseTask = cfg.Handle<TResponse>();

                _callback?.Invoke(cfg);
            });

            await SendRequest(request, pipe, cancellationToken).ConfigureAwait(false);

            return await responseTask.ConfigureAwait(false);
        }

        protected abstract Task SendRequest(TRequest request, IPipe<SendContext<TRequest>> requestPipe, CancellationToken cancellationToken);
    }
}