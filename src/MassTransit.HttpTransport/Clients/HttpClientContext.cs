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
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    public class HttpClientContext :
        BasePipeContext,
        ClientContext,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<HttpClientContext>();
        readonly HttpClient _client;
        readonly ITaskParticipant _participant;
        readonly IReceivePipe _receivePipe;

        public HttpClientContext(HttpClient client, IReceivePipe receivePipe, ITaskScope taskScope)
            : this(client, receivePipe, taskScope.CreateParticipant($"{TypeMetadataCache<HttpClientContext>.ShortName} - {client.BaseAddress}"))
        {
        }

        public HttpClientContext(HttpClient client, IReceivePipe receivePipe, ITaskParticipant participant)
        {
            _client = client;
            _receivePipe = receivePipe;
            _participant = participant;
        }

        CancellationToken PipeContext.CancellationToken => _participant.StoppedToken;

        public Uri BaseAddress => _client.BaseAddress;

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _client.SendAsync(request, cancellationToken);
        }

        public Task ReceiveResponse(ReceiveContext receiveContext)
        {
            return _receivePipe.Send(receiveContext);
        }

        public void Dispose()
        {
            Close("ClientContext Disposed");
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return await _client.SendAsync(request, completionOption);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(request, completionOption, cancellationToken);
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


    public class SharedHttpClientContext :
        ClientContext,
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly ClientContext _context;
        readonly ITaskParticipant _participant;

        public SharedHttpClientContext(ClientContext context, CancellationToken cancellationToken, ITaskScope scope)
        {
            _context = context;
            _cancellationToken = cancellationToken;

            _participant = scope.CreateParticipant($"{TypeMetadataCache<SharedHttpClientContext>.ShortName} - {context.BaseAddress}");

            _participant.SetReady();
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

       void IDisposable.Dispose()
        {
            _participant.SetComplete();
        }

        public Uri BaseAddress
        {
            get { return _context.BaseAddress; }
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _context.SendAsync(request, cancellationToken);
        }

        public Task ReceiveResponse(ReceiveContext receiveContext)
        {
            return _context.ReceiveResponse(receiveContext);
        }
    }
}