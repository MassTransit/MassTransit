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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Configurators;
    using Topology;
    using Transports;


    public class RabbitMqHost :
        BaseHost,
        IRabbitMqHostControl
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqHost(IRabbitMqHostConfiguration hostConfiguration)
            : base(hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<RabbitMqConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new RabbitMqConnectionContextSupervisor(hostConfiguration);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "RabbitMQ",
                Settings.Host,
                Settings.Port,
                Settings.VirtualHost,
                Settings.Username,
                Password = new string('*', Settings.Password.Length),
                Settings.Heartbeat,
                Settings.Ssl
            });

            if (Settings.Ssl)
            {
                context.Set(new
                {
                    Settings.SslServerName
                });
            }

            ConnectionContextSupervisor.Probe(context);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public RabbitMqHostSettings Settings => _hostConfiguration.Settings;
        IRabbitMqHostTopology IRabbitMqHost.Topology => _hostConfiguration.Topology;

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_hostConfiguration.Topology.CreateTemporaryQueueName("endpoint-"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return ReceiveEndpoints.Start(queueName);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            await ConnectionContextSupervisor.Stop(context).ConfigureAwait(false);
        }
    }
}