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
namespace MassTransit.PipeConfigurators
{
    using System;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes.Configurators;


    public class RetryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<IRetryConfigurator> _configure;

        public RetryConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, Action<IRetryConfigurator> configure)
            : base(receiveEndpointConfigurator)
        {
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification =
                new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>((x, r) => new RetryConsumeContext<TMessage>(x, r));

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}