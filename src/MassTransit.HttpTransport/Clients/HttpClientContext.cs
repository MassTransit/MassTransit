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
namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    public class HttpClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<HttpClientContext>();
        readonly HttpClient _client;
        readonly IReceivePipe _receivePipe;

        public HttpClientContext(HttpClient client, IReceivePipe receivePipe, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _client = client;
            _receivePipe = receivePipe;
        }

        public Uri BaseAddress => _client.BaseAddress;

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _client.SendAsync(request, cancellationToken);
        }

        public Task ReceiveResponse(ReceiveContext receiveContext)
        {
            return _receivePipe.Send(receiveContext);
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.Debug($"Closing client: {_client.BaseAddress}");

            try
            {
                _client.CancelPendingRequests();

                _client.Dispose();
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error("Fault waiting for Dispose", ex);
            }

            return TaskUtil.Completed;
        }
    }
}