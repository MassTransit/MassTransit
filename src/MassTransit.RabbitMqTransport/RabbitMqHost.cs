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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Integration;
    using Logging;
    using MassTransit.Pipeline;
    using Policies;
    using Transports;
    using Util;


    public class RabbitMqHost :
        IRabbitMqHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<RabbitMqHost>();
        readonly RabbitMqConnectionCache _connectionCache;
        readonly IRetryPolicy _connectionRetryPolicy;
        readonly RabbitMqHostSettings _hostSettings;
        readonly TaskSupervisor _supervisor;

        public RabbitMqHost(RabbitMqHostSettings hostSettings)
        {
            _hostSettings = hostSettings;

            var exceptionFilter = Retry.Selected<RabbitMqConnectionException>();

            _connectionRetryPolicy = exceptionFilter.Exponential(1000, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<RabbitMqHost>.ShortName} - {_hostSettings.ToDebugString()}");

            _connectionCache = new RabbitMqConnectionCache(hostSettings, _supervisor);
        }

        public HostHandle Start()
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", _hostSettings.ToDebugString());

                try
                {
                    await _supervisor.StopRequested.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting connection to {0}", _hostSettings.ToDebugString());

            var connectionTask = _connectionRetryPolicy.RetryUntilCancelled(() => _connectionCache.Send(connectionPipe, _supervisor.StoppingToken), _supervisor.StoppingToken);

            return new Handle(connectionTask, _supervisor);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "RabbitMQ",
                _hostSettings.Host,
                _hostSettings.Port,
                _hostSettings.VirtualHost,
                _hostSettings.Username,
                Password = new string('*', _hostSettings.Password.Length),
                _hostSettings.Heartbeat,
                _hostSettings.Ssl
            });

            if (_hostSettings.Ssl)
            {
                scope.Set(new
                {
                    _hostSettings.SslServerName
                });
            }

            _connectionCache.Probe(scope);
        }

        public IConnectionCache ConnectionCache => _connectionCache;
        public RabbitMqHostSettings Settings => _hostSettings;
        public IRetryPolicy ConnectionRetryPolicy => _connectionRetryPolicy;
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