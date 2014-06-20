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


    public class ModelConsumerPipeBuilderConfigurator :
        IPipeBuilderConfigurator<ConnectionContext>
    {
        readonly IPipe<ReceiveContext> _pipe;
        readonly ReceiveSettings _settings;
        readonly IEnumerable<SubscriptionSettings> _subscriptions;

        public ModelConsumerPipeBuilderConfigurator(IPipe<ReceiveContext> pipe, ReceiveSettings settings,
            IEnumerable<SubscriptionSettings> subscriptions)
        {
            _settings = settings;
            _subscriptions = subscriptions;
            _pipe = pipe;
        }

        public void Configure(IPipeBuilder<ConnectionContext> builder)
        {
            IPipe<ModelContext> pipe = Pipe.New<ModelContext>(x =>
            {
                x.Filter(new ReceiveSettingsModelFilter(_settings));

                foreach (SubscriptionSettings subscription in _subscriptions)
                    x.Filter(new SubscriptionModelFilter(subscription));

                x.Filter(new ModelConsumerFilter(_pipe));
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