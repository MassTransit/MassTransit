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
    using Configuration;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Hosting;
    using Transports;


    public class HttpHost :
        BaseHost,
        IHttpHostControl
    {
        readonly IHttpHostConfiguration _hostConfiguration;
        readonly HttpHostContextSupervisor _httpHostContextSupervisor;

        public HttpHost(IHttpHostConfiguration hostConfiguration)
            : base(hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _httpHostContextSupervisor = new HttpHostContextSupervisor(hostConfiguration);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            throw new NotImplementedException();
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<HostHandle> Start(CancellationToken cancellationToken)
        {
            var handlesReady = new TaskCompletionSource<HostReceiveEndpointHandle[]>();
            var hostStarted = new TaskCompletionSource<bool>();

            IPipe<HttpHostContext> connectionPipe = Pipe.ExecuteAsync<HttpHostContext>(async context =>
            {
                try
                {
                    await handlesReady.Task.ConfigureAwait(false);

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

            var connectionTask = HttpHostContextSupervisor.Send(connectionPipe, Stopping);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints(cancellationToken);

            var hostHandle = new Handle(handles, this, _httpHostContextSupervisor, connectionTask);

            await hostHandle.Ready.ConfigureAwait(false);

            handlesReady.TrySetResult(handles);

            await hostStarted.Task.ConfigureAwait(false);

            return hostHandle;
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "HTTP",
                Settings.Host,
                Settings.Port,
                Settings.Method.Method
            });
        }

        public IHttpHostContextSupervisor HttpHostContextSupervisor => _httpHostContextSupervisor;
        public HttpHostSettings Settings => _hostConfiguration.Settings;

        class Handle :
            HostHandle
        {
            readonly IAgent _hostAgent;
            readonly Task _connectionTask;
            readonly HostReceiveEndpointHandle[] _handles;
            readonly IBusHostControl _host;

            public Handle(HostReceiveEndpointHandle[] handles, IBusHostControl host, IAgent hostAgent, Task connectionTask)
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
