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
namespace MassTransit.RabbitMqTransport.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Pipeline;
    using Topology;
    using Util;


    public class RabbitMqConsumerPipeSpecification :
        IPipeSpecification<ConnectionContext>
    {
        readonly IRabbitMqHost _host;
        readonly IConsumePipeConnector _managementPipe;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _settings;
        readonly ITaskSupervisor _supervisor;
        readonly IRabbitMqReceiveEndpointTopology _topology;
        readonly IReceiveTransportObserver _transportObserver;

        public RabbitMqConsumerPipeSpecification(IPipe<ReceiveContext> receivePipe, ReceiveSettings settings, IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver, ITaskSupervisor supervisor, IConsumePipeConnector managementPipe, IRabbitMqHost host,
            IRabbitMqReceiveEndpointTopology topology)
        {
            _settings = settings;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _receivePipe = receivePipe;
            _managementPipe = managementPipe;
            _host = host;
            _topology = topology;
        }

        public void Apply(IPipeBuilder<ConnectionContext> builder)
        {
            IPipe<ModelContext> pipe = Pipe.New<ModelContext>(x =>
            {
                x.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, _topology.BrokerTopology));

                if (_settings.PurgeOnStartup)
                    x.UseFilter(new PurgeOnStartupFilter(_settings.QueueName));

                x.UseFilter(new PrefetchCountFilter(_managementPipe, _settings.PrefetchCount));

                x.UseFilter(new RabbitMqConsumerFilter(_receivePipe, _receiveObserver, _transportObserver, _supervisor, _topology));
            });

            IFilter<ConnectionContext> modelFilter = new ReceiveModelFilter(pipe, _supervisor, _host);

            builder.AddFilter(modelFilter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}