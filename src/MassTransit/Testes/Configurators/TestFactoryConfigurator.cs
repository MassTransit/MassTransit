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
namespace MassTransit.Testes.Configurators
{
    using System;
    using Builders;
    using BusConfigurators;
    using EndpointConfigurators;
    using GreenPipes;


    public abstract class TestFactoryConfigurator<TBuilder> :
        ITestFactoryConfigurator
        where TBuilder : IBusBuilder
    {
        readonly BusFactoryConfigurator<TBuilder> _configurator;
        readonly ReceiveEndpointSpecification _specification;

        protected TestFactoryConfigurator(BusFactoryConfigurator<TBuilder> configurator, ReceiveEndpointSpecification specification)
        {
            _configurator = configurator;
            _specification = specification;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification) where T : class
        {
            _specification.AddPipeSpecification(specification);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _specification.ConfigureSend(callback);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _specification.ConfigurePublish(callback);
        }
    }
}