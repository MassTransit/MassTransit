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
namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Logging;
    using MassTransit.Pipeline;
    using Transports;
    using Util;


    public class HttpHost :
        IHttpHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<HttpHost>();
        readonly OwinHostCache _owinHostCache;

        readonly TaskSupervisor _supervisor;

        public HttpHost(HttpHostSettings hostSettings)
        {
            Settings = hostSettings;
            _supervisor = new TaskSupervisor($"{TypeMetadataCache<HttpHost>.ShortName} - {Settings.Host}");
            _owinHostCache = new OwinHostCache(Settings, _supervisor);
        }

        public HostHandle Start()
        {
            IPipe<OwinHostContext> connectionPipe = Pipe.ExecuteAsync<OwinHostContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", Settings.Host);

                try
                {
                    //Wait until someone shuts down the bus - Parked thread.
                    await _supervisor.StopRequested.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting connection to {0}", Settings.Host);

            var connectionTask = OwinHostCache.Send(connectionPipe, _supervisor.StoppingToken);

            return new Handle(connectionTask, _supervisor);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "HTTP",
                Settings.Host,
                Settings.Port
            });


            _owinHostCache.Probe(scope);
        }

        public IOwinHostCache OwinHostCache => _owinHostCache;

        public HttpHostSettings Settings { get; }

        public ITaskSupervisor Supervisor => _supervisor;


        class Handle :
            HostHandle
        {
            readonly Task _connectionTask;
            readonly TaskSupervisor _supervisor;

            public Handle(Task connectionTask, TaskSupervisor supervisor)
            {
                _connectionTask = connectionTask;
                _supervisor = supervisor;
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
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