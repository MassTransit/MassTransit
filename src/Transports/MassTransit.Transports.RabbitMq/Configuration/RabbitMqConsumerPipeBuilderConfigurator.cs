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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System.Collections.Generic;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;


    public class RabbitMqConsumerPipeBuilderConfigurator :
        IPipeBuilderConfigurator<ConnectionContext>
    {
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _settings;
        readonly IEnumerable<ExchangeBindingSettings> _exchangeBindings;

        public RabbitMqConsumerPipeBuilderConfigurator(IPipe<ReceiveContext> receivePipe, ReceiveSettings settings,
            IEnumerable<ExchangeBindingSettings> exchangeBindings)
        {
            _settings = settings;
            _exchangeBindings = exchangeBindings;
            _receivePipe = receivePipe;
        }

        public void Configure(IPipeBuilder<ConnectionContext> builder)
        {
            IPipe<ModelContext> pipe = Pipe.New<ModelContext>(x =>
            {
                x.Filter(new PrepareReceiveQueueFilter(_settings));

                foreach (ExchangeBindingSettings binding in _exchangeBindings)
                    x.Filter(new ExchangeBindingModelFilter(binding));

                x.Filter(new RabbitMqConsumerFilter(_receivePipe));
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