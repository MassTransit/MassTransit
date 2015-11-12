// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Management;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;
    using Topology;
    using Util;


    public class RabbitMqConsumerPipeSpecification :
        IPipeSpecification<ConnectionContext>
    {
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _settings;
        readonly IReceiveObserver _receiveObserver;
        readonly IReceiveEndpointObserver _endpointObserver;
        readonly ITaskSupervisor _taskSupervisor;
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly Mediator<ISetPrefetchCount> _mediator;

        public RabbitMqConsumerPipeSpecification(IPipe<ReceiveContext> receivePipe, ReceiveSettings settings, IReceiveObserver receiveObserver, IReceiveEndpointObserver endpointObserver, IEnumerable<ExchangeBindingSettings> exchangeBindings, ITaskSupervisor taskSupervisor, Mediator<ISetPrefetchCount> mediator)
        {
            _settings = settings;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _taskSupervisor = taskSupervisor;
            _exchangeBindings = exchangeBindings.ToArray();
            _receivePipe = receivePipe;
            _mediator = mediator;
        }

        public void Apply(IPipeBuilder<ConnectionContext> builder)
        {
            IPipe<ModelContext> pipe = Pipe.New<ModelContext>(x =>
            {
                x.UseFilter(new PrepareReceiveQueueFilter(_settings, _mediator, _exchangeBindings));

                x.UseFilter(new RabbitMqConsumerFilter(_receivePipe, _receiveObserver, _endpointObserver, _taskSupervisor));
            });

            IFilter<ConnectionContext> modelFilter = new ReceiveModelFilter(pipe);

            builder.AddFilter(modelFilter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}