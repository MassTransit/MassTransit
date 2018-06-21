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
namespace MassTransit.EndpointSpecifications
{
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using SagaConfigurators;


    public class ConfigurationReceiveEndpointSpecification :
        IReceiveEndpointSpecification<IBusBuilder>,
        IConsumerConfigurationObserverConnector,
        ISagaConfigurationObserverConnector,
        IHandlerConfigurationObserverConnector
    {
        readonly IReceiveEndpointConfiguration _configuration;
        IReceiveEndpoint _receiveEndpoint;

        public ConfigurationReceiveEndpointSpecification(IReceiveEndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISendEndpointProvider SendEndpointProvider => _receiveEndpoint?.Context.SendEndpointProvider;

        public IPublishEndpointProvider PublishEndpointProvider => _receiveEndpoint?.Context.PublishEndpointProvider;

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configuration.ConnectConsumerConfigurationObserver(observer);
        }

        public void Apply(IBusBuilder builder)
        {
            _receiveEndpoint = _configuration.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configuration.Validate();
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configuration.ConnectSagaConfigurationObserver(observer);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _configuration.ConnectHandlerConfigurationObserver(observer);
        }
    }
}