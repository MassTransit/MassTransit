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
namespace MassTransit.RabbitMqTransport.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Pipeline.Pipes;
    using Pipeline;
    using Topology;
    using Util;


    public class RabbitMqConsumerPipeSpecification :
        IPipeSpecification<ConnectionContext>
    {
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly IManagementPipe _managementPipe;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ReceiveSettings _settings;
        readonly ITaskSupervisor _supervisor;
        readonly IReceiveTransportObserver _transportObserver;
        readonly IRabbitMqHost _host;

        public RabbitMqConsumerPipeSpecification(IPipe<ReceiveContext> receivePipe, ReceiveSettings settings, IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver, IEnumerable<ExchangeBindingSettings> exchangeBindings, ITaskSupervisor supervisor,
            IManagementPipe managementPipe, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider, IRabbitMqHost host)
        {
            _settings = settings;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _exchangeBindings = exchangeBindings.Distinct().ToArray();
            _receivePipe = receivePipe;
            _managementPipe = managementPipe;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _host = host;
        }

        public void Apply(IPipeBuilder<ConnectionContext> builder)
        {
            IPipe<ModelContext> pipe = Pipe.New<ModelContext>(x =>
            {
                x.UseFilter(new PrepareReceiveQueueFilter(_settings, _managementPipe, _exchangeBindings));

                x.UseFilter(new RabbitMqConsumerFilter(_receivePipe, _receiveObserver, _transportObserver, _supervisor, _sendEndpointProvider,
                    _publishEndpointProvider));
            });

            IFilter<ConnectionContext> modelFilter = new ReceiveModelFilter(pipe, _supervisor, _host);

            builder.AddFilter(modelFilter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}