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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Hosting;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Transports;


    public class HttpHost :
        Supervisor,
        IHttpHostControl
    {
        static readonly ILog _log = Logger.Get<HttpHost>();
        readonly HttpHostCache _httpHostCache;

        readonly HttpHostSettings _settings;

        public HttpHost(HttpHostSettings hostSettings, IHostTopology topology)
        {
            _settings = hostSettings;
            Topology = topology;

            ReceiveEndpoints = new ReceiveEndpointCollection();

            _httpHostCache = new HttpHostCache(Settings);
        }

        public IHttpReceiveEndpointFactory ReceiveEndpointFactory { get; set; }

        public IReceiveEndpointCollection ReceiveEndpoints { get; }

        public async Task<HostHandle> Start()
        {
            var handlesReady = new TaskCompletionSource<HostReceiveEndpointHandle[]>();
            var hostStarted = new TaskCompletionSource<bool>();

            IPipe<HttpHostContext> connectionPipe = Pipe.ExecuteAsync<HttpHostContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", Settings.Host);

                try
                {
                    HostReceiveEndpointHandle[] endpointHandles = await handlesReady.Task.ConfigureAwait(false);

                    await context.Start(context.CancellationToken).ConfigureAwait(false);

                    hostStarted.TrySetResult(true);

                    await Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    hostStarted.TrySetException(ex);
                }
                catch (Exception ex)
                {
                    hostStarted.TrySetException(ex);
                    throw;
                }
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting connection to {0}", Settings.Host);

            var connectionTask = HttpHostCache.Send(connectionPipe, Stopping);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            var hostHandle = new Handle(handles, this, _httpHostCache, connectionTask);

            await hostHandle.Ready.ConfigureAwait(false);

            handlesReady.TrySetResult(handles);

            await hostStarted.Task.ConfigureAwait(false);

            return hostHandle;
        }

        public bool Matches(Uri address)
        {
            var settings = address.GetHostSettings();

            return HttpHostEqualityComparer.Default.Equals(_settings, settings);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            ReceiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "HTTP",
                Settings.Host,
                Settings.Port,
                Settings.Method.Method
            });

            ReceiveEndpoints.Probe(scope);
        }

        public IHttpHostCache HttpHostCache => _httpHostCache;
        public HttpHostSettings Settings => _settings;
        public Uri Address => new Uri($"{_settings.Scheme}://{_settings.Host}:{_settings.Port}");

        public IHostTopology Topology { get; }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return ReceiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return ReceiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return ReceiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return ReceiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return ReceiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return ReceiveEndpoints.ConnectSendObserver(observer);
        }


        class Handle :
            HostHandle
        {
            readonly IAgent _hostAgent;
            readonly Task _connectionTask;
            readonly HostReceiveEndpointHandle[] _handles;
            readonly HttpHost _host;

            public Handle(HostReceiveEndpointHandle[] handles, HttpHost host, IAgent hostAgent, Task connectionTask)
            {
                _host = host;
                _handles = handles;
                _hostAgent = hostAgent;
                _connectionTask = connectionTask;
            }

            public Task<HostReady> Ready
            {
                get { return ReadyOrNot(_handles.Select(x => x.Ready)); }
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                await Task.WhenAll(_handles.Select(x => x.StopAsync(cancellationToken))).ConfigureAwait(false);

                await _host.Stop("Host Stopped", cancellationToken).ConfigureAwait(false);

                await _hostAgent.Stop("Host stopped", cancellationToken).ConfigureAwait(false);

                await _connectionTask.ConfigureAwait(false);
            }

            async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();

                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _hostAgent.Ready.ConfigureAwait(false);

                ReceiveEndpointReady[] endpointsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

                return new HostReadyEvent(_host.Address, endpointsReady);
            }
        }
    }
}