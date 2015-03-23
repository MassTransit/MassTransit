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
namespace RapidTransit
{
    using System.Collections.Generic;
    using System.Configuration;
    using Configuration;
    using MassTransit;
    using MassTransit.EndpointConfigurators;
    using MassTransit.Logging;
    using MassTransit.PipeConfigurators;


    /// <summary>
    /// A hosted service bus allows the configurationProvider of a service bus to be contained in a class
    /// making it easier to manage service dependencies
    /// </summary>
    public abstract class ServiceBusInstance :
        IReceiveEndpointConfigurator,
        IServiceBusInstance
    {
        static readonly ILog _log = Logger.Get<ServiceBusInstance>();
        readonly List<IReceiveEndpointSpecification> _configurators;
        readonly int _consumerLimit;
        readonly List<IPipeSpecification<ConsumeContext>> _pipeConfigurators;
        readonly string _queueName;

        protected ServiceBusInstance(IConfigurationProvider configurationProvider, string queueKey, string consumerLimitKey,
            int defaultConsumerLimit)
        {
            string queueName;
            if (!configurationProvider.TryGetSetting(queueKey, out queueName))
                throw new ConfigurationErrorsException("Unable to load queue name from key: " + queueKey);

            _queueName = queueName;
            _consumerLimit = configurationProvider.GetSetting(consumerLimitKey, defaultConsumerLimit);

            _configurators = new List<IReceiveEndpointSpecification>();
            _pipeConfigurators = new List<IPipeSpecification<ConsumeContext>>();
        }

        void IReceiveEndpointConfigurator.AddConfigurator(IReceiveEndpointSpecification configurator)
        {
            _configurators.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _pipeConfigurators.Add(specification);
        }

        public void Configure(IReceiveEndpointConfigurator configurator)
        {
            _log.InfoFormat("{0} Configuring Receive Endpoint for Queue: {1}({2})", GetType().GetServiceDescription(), _queueName,
                _consumerLimit);

            foreach (IReceiveEndpointSpecification builderConfigurator in _configurators)
                configurator.AddConfigurator(builderConfigurator);

            foreach (var pipeConfigurator in _pipeConfigurators)
                configurator.AddPipeSpecification(pipeConfigurator);
        }
    }
}