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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Configuration.Configuration;
    using Configurators;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Transports;


    public class AmazonSqsHost :
        BaseHost,
        IAmazonSqsHostControl
    {
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public AmazonSqsHost(IAmazonSqsHostConfiguration hostConfiguration)
            : base(hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new AmazonSqsConnectionContextSupervisor(hostConfiguration);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public AmazonSqsHostSettings Settings => _hostConfiguration.Settings;
        public IAmazonSqsHostTopology Topology => _hostConfiguration.Topology;

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "AmazonSQS",
                Settings.Region,
                Settings.AccessKey,
                Password = new string('*', Settings.SecretKey.Length)
            });

            ConnectionContextSupervisor.Probe(context);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            return ConnectReceiveEndpoint(_hostConfiguration.Topology.CreateTemporaryQueueName("endpoint-"), configure);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
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