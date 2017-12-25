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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosting;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Transports;
    using Util;


    [DebuggerDisplay("{DebuggerDisplay}")]
    public class HttpHost :
        IHttpHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<HttpHost>();
        readonly OwinHostCache _owinHostCache;
        // Retry
        readonly HttpHostSettings _settings;
        readonly TaskSupervisor _supervisor;
        Uri _address;

        public HttpHost(HttpHostSettings hostSettings, IHostTopology topology)
        {
            _settings = hostSettings;
            Topology = topology;

            //exception Filter
            ReceiveEndpoints = new ReceiveEndpointCollection();

            //connection retry policy

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<HttpHost>.ShortName} - {Settings.Host}");

            _owinHostCache = new OwinHostCache(Settings, _supervisor);
        }

        public IHttpReceiveEndpointFactory ReceiveEndpointFactory { get; set; }

        public IReceiveEndpointCollection ReceiveEndpoints { get; }

        public async Task<HostHandle> Start()
        {
            TaskCompletionSource<HostReceiveEndpointHandle[]> handlesReady = new TaskCompletionSource<HostReceiveEndpointHandle[]>();
            TaskCompletionSource<bool> hostStarted = new TaskCompletionSource<bool>();

            IPipe<OwinHostContext> connectionPipe = Pipe.ExecuteAsync<OwinHostContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", Settings.Host);

                try
                {
                    var endpointHandles = await handlesReady.Task.ConfigureAwait(false);

                    context.StartHost();

                    hostStarted.TrySetResult(true);

                    //Wait until someone shuts down the bus - Parked thread.
                    await _supervisor.StopRequested.ConfigureAwait(false);
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

            var connectionTask = OwinHostCache.Send(connectionPipe, _supervisor.StoppingToken);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            handlesReady.TrySetResult(handles);

            await hostStarted.Task.ConfigureAwait(false);

            return new Handle(connectionTask, handles, _supervisor, this);
        }

        public bool Matches(Uri address)
        {
            var settings = address.GetHostSettings();

            return HttpHostEqualityComparer.Default.Equals(_settings, settings);
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


            _owinHostCache.Probe(scope);

            ReceiveEndpoints.Probe(scope);
        }

        public IOwinHostCache OwinHostCache => _owinHostCache;
        public HttpHostSettings Settings => _settings;
        public ITaskSupervisor Supervisor => _supervisor;


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
        
        string DebuggerDisplay => $"{Address}";

        class Handle :
            BaseHostHandle
        {
            readonly Task _connectionTask;
            readonly TaskSupervisor _supervisor;

            public Handle(Task connectionTask, HostReceiveEndpointHandle[] handles, TaskSupervisor supervisor, IHost host)
                : base(host, handles)
            {
                _connectionTask = connectionTask;
                _supervisor = supervisor;
            }

            public override async Task Stop(CancellationToken cancellationToken)
            {
                await base.Stop(cancellationToken).ConfigureAwait(false);

                await _supervisor.Stop("Host stopped", cancellationToken).ConfigureAwait(false);

                try
                {
                    await _connectionTask.ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}