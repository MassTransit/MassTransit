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
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class HttpClientCache :
        ClientCache,
        IProbeSite
    {
        static readonly ILog _log = Logger.Get<HttpClientCache>();
        readonly ITaskScope _cacheTaskScope;

        readonly object _scopeLock = new object();
        ClientScope _scope;

        public HttpClientCache(ITaskSupervisor supervisor)
        {
            _cacheTaskScope = supervisor.CreateScope($"{TypeMetadataCache<HttpClientCache>.ShortName}", CloseScope);
        }

        public Task DoWith(IPipe<ClientContext> clientPipe, CancellationToken cancellationToken)
        {
            ClientScope newScope = null;
            ClientScope existingScope;

            lock (_scopeLock)
            {
                existingScope = _scope;
                if (existingScope == null)
                {
                    newScope = new ClientScope(_cacheTaskScope);
                    _scope = newScope;
                }
            }
            if (existingScope != null)
                return SendUsingExistingClient(clientPipe, existingScope, cancellationToken);

            return SendUsingNewClient(clientPipe, newScope, cancellationToken);
        }

        public async Task Close()
        {
            Interlocked.Exchange(ref _scope, null);

            _cacheTaskScope.Stop(new StopEventArgs("Closed by owner"));
        }

        public void Probe(ProbeContext context)
        {
            var clientScope = _scope;
            if (clientScope != null)
            {
                context.Set(new
                {
                    Connected = true
                });
            }
        }

        Task CloseScope()
        {
            return TaskUtil.Completed;
        }

        async Task SendUsingNewClient(IPipe<ClientContext> clientPipe, ClientScope scope, CancellationToken cancellationToken)
        {
            try
            {
                var client = new HttpClient();
                var clientContext = new HttpClientContext(client, _cacheTaskScope);

                scope.Created(clientContext);
            }
            catch (Exception ex)
            {
                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.CreateFaulted(ex);

                throw;
            }

            await SendUsingExistingClient(clientPipe, scope, cancellationToken).ConfigureAwait(false);
        }

        async Task SendUsingExistingClient(IPipe<ClientContext> clientPipe, ClientScope scope, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = await scope.Attach(cancellationToken).ConfigureAwait(false))
                {
                    await clientPipe.Send(context).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("The client usage threw an exception", exception);

                Interlocked.CompareExchange(ref _scope, null, scope);

                scope.CreateFaulted(exception);

                throw;
            }
        }


        class ClientScope
        {
            readonly TaskCompletionSource<HttpClientContext> _clientContext;
            readonly ITaskScope _taskScope;

            public ClientScope(ITaskScope supervisor)
            {
                _clientContext = new TaskCompletionSource<HttpClientContext>();

                _taskScope = supervisor.CreateScope("ClientScope", CloseContext);
            }

            public void Created(HttpClientContext clientContext)
            {
                _clientContext.TrySetResult(clientContext);

                _taskScope.SetReady();
            }

            public void CreateFaulted(Exception exception)
            {
                _clientContext.TrySetException(exception);

                _taskScope.SetNotReady(exception);

                _taskScope.Stop(new StopEventArgs($"Client faulted: {exception.Message}"));
            }

            public async Task<HttpClientContext> Attach(CancellationToken cancellationToken)
            {
                var clientContext = await _clientContext.Task.ConfigureAwait(false);

                //TODO: new SharedClientContext(clientContext, concellationToken, _taskScope);
                return clientContext;
            }

            async Task CloseContext()
            {
                if (_clientContext.Task.Status == TaskStatus.RanToCompletion)
                {
                    var clientContext = await _clientContext.Task.ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Disposing client: {0}", ((ClientContext)clientContext).Client.BaseAddress);

                    clientContext.Client.CancelPendingRequests();
                    clientContext.Dispose();
                }
            }
        }
    }
}