// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Integration;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Policies;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqHost :
        IRabbitMqHost,
        IBusHostControl
    {
        static readonly ILog _log = Logger.Get<RabbitMqHost>();
        readonly RabbitMqConnectionCache _connectionCache;
        readonly IRetryPolicy _connectionRetryPolicy;
        readonly RabbitMqHostSettings _settings;
        readonly TaskSupervisor _supervisor;
        readonly IRabbitMqHostTopology _topology;

        public RabbitMqHost(RabbitMqHostSettings settings, IRabbitMqHostTopology topology)
        {
            _settings = settings;
            _topology = topology;

            var exceptionFilter = Retry.Selected<RabbitMqConnectionException>();
            ReceiveEndpoints = new ReceiveEndpointCollection();

            _connectionRetryPolicy = exceptionFilter.Exponential(1000, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<RabbitMqHost>.ShortName} - {_settings.ToDebugString()}");

            _connectionCache = new RabbitMqConnectionCache(settings, _topology, _supervisor);
        }

        public IRabbitMqReceiveEndpointFactory ReceiveEndpointFactory { get; set; }

        public IReceiveEndpointCollection ReceiveEndpoints { get; }

        public async Task<HostHandle> Start()
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", _settings.ToDebugString());

                try
                {
                    await _supervisor.StopRequested.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting connection to {0}", _settings.ToDebugString());

            var connectionTask = _connectionRetryPolicy.RetryUntilCancelled(() => _connectionCache.Send(connectionPipe, _supervisor.StoppingToken),
                _supervisor.StoppingToken);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            return new Handle(connectionTask, handles, _supervisor, this);
        }

        public bool Matches(Uri address)
        {
            var settings = address.GetHostSettings();

            return RabbitMqHostEqualityComparer.Default.Equals(_settings, settings);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "RabbitMQ",
                _settings.Host,
                _settings.Port,
                _settings.VirtualHost,
                _settings.Username,
                Password = new string('*', _settings.Password.Length),
                _settings.Heartbeat,
                _settings.Ssl
            });

            if (_settings.Ssl)
                scope.Set(new
                {
                    _settings.SslServerName
                });

            _connectionCache.Probe(scope);

            ReceiveEndpoints.Probe(scope);
        }

        public IConnectionCache ConnectionCache => _connectionCache;
        public RabbitMqHostSettings Settings => _settings;

        public IRabbitMqHostTopology Topology => _topology;
        IHostTopology IHost.Topology => Topology;

        public IRetryPolicy ConnectionRetryPolicy => _connectionRetryPolicy;
        public ITaskSupervisor Supervisor => _supervisor;

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_topology.CreateTemporaryQueueName("endpoint"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            if (ReceiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            ReceiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return ReceiveEndpoints.Start(queueName);
        }

        public Uri Address => _settings.HostAddress;

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