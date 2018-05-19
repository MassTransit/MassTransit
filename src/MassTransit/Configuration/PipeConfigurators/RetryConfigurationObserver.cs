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
    using System.Threading;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;


    public class RetryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly CancellationToken _cancellationToken;
        readonly Action<IRetryConfigurator> _configure;

        public RetryConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, CancellationToken cancellationToken, Action<IRetryConfigurator> configure)
            : base(receiveEndpointConfigurator)
        {
            _cancellationToken = cancellationToken;
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        static RetryConsumeContext<TMessage> Factory<TMessage>(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy)
            where TMessage : class
        {
            return new RetryConsumeContext<TMessage>(context, retryPolicy);
        }
    }
}