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
namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Specifications;

    public class DelayedExchangeRedeliveryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<IRetryConfigurator> _configure;

        public DelayedExchangeRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator)
        {
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var redeliverySpecification = new DelayedExchangeRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<TMessage>();

            _configure(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }
    }
}
