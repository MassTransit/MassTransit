// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using BusConfigurators;
    using MassTransit.Builders;
    using MassTransit.Configurators;
    using PipeConfigurators;


    public class RabbitMqBusFactoryConfigurator :
        IRabbitMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly ConsumePipeSpecification _consumePipeSpecification;
        readonly IList<IRabbitMqHost> _hosts;
        readonly IList<IBusFactorySpecification> _transportBuilderConfigurators;

        public RabbitMqBusFactoryConfigurator()
        {
            _hosts = new List<IRabbitMqHost>();
            _transportBuilderConfigurators = new List<IBusFactorySpecification>();
            _consumePipeSpecification = new ConsumePipeSpecification();
        }

        public IBusControl CreateBus()
        {
            var builder = new RabbitMqBusBuilder(_hosts, _consumePipeSpecification);

            foreach (IBusFactorySpecification configurator in _transportBuilderConfigurators)
                configurator.Apply(builder);

            IBusControl bus = builder.Build();

            return bus;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_hosts.Count == 0)
                yield return this.Failure("Host", "At least one host must be defined");

            foreach (ValidationResult result in _transportBuilderConfigurators.SelectMany(x => x.Validate()))
                yield return result;
            foreach (ValidationResult result in _consumePipeSpecification.Validate())
                yield return result;
        }

        public IRabbitMqHost Host(RabbitMqHostSettings settings)
        {
            var host = new RabbitMqHost(settings);
            _hosts.Add(host);

            return host;
        }

        public void AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _transportBuilderConfigurators.Add(configurator);
        }

        public void Mandatory(bool mandatory = true)
        {
//            _publishSettings.Mandatory = mandatory;
        }

        public void ReceiveEndpoint(IRabbitMqHost host, string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            if (host == null)
                throw new EndpointNotFoundException("The host address specified was not configured.");

            var endpointConfigurator = new RabbitMqReceiveEndpointConfigurator(host, queueName);

            configure(endpointConfigurator);

            AddBusFactorySpecification(endpointConfigurator);
        }


        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _consumePipeSpecification.Add(specification);
        }
        public void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback)
            where T : class
        {
            throw new NotImplementedException();
        }

        public void OnPublish(Action<RabbitMqPublishContext> callback)
        {
            throw new NotImplementedException();
        }
    }
}