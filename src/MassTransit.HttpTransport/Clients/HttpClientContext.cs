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
namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Util;


    public class HttpClientContext :
        BasePipeContext,
        ClientContext,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<HttpClientContext>();
        readonly ITaskParticipant _participant;
        readonly HttpClient _client;

        public HttpClientContext(HttpClient client, ITaskScope taskScope)
            :
                this(client, taskScope.CreateParticipant($"{TypeMetadataCache<HttpClientContext>.ShortName} - {client.BaseAddress}"))
        {
        }

        public HttpClientContext(HttpClient client, ITaskParticipant participant)
            : base(new PayloadCache())
        {
            _client = client;
            _participant = participant;
        }

        CancellationToken PipeContext.CancellationToken => _participant.StoppedToken;
        

        public Uri BaseAddress => _client.BaseAddress;

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return await _client.SendAsync(request, completionOption);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(request, completionOption, cancellationToken);
        }

        public void Dispose()
        {
            Close("ClientContext Disposed");
        }

        void Close(string reason)
        {
            if (_log.IsDebugEnabled)
                _log.Debug($"Closing client: {_client.BaseAddress}");

            try
            {
                _client.Dispose();
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error("Fault waiting for Dispose", ex);
            }

            _participant.SetComplete();
        }

        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }
    }
}