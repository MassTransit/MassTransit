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
namespace MassTransit.Containers.Tests
{
    using NUnit.Framework;
    using Ninject;
    using Ninject.Extensions.NamedScope;
    using Saga;
    using Scenarios;


    
    public class Ninject_Consumer :
        When_registering_a_consumer
    {
        readonly IKernel _container;

        public Ninject_Consumer()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleConsumer>()
                .ToSelf().DefinesNamedScope("message");
            _container.Bind<ISimpleConsumerDependency>()
                .To<SimpleConsumerDependency>().InNamedScope("message");
            _container.Bind<AnotherMessageConsumer>()
                .To<AnotherMessageConsumerImpl>().InNamedScope("message");
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    
    public class Ninject_Saga :
        When_registering_a_saga
    {
        readonly IKernel _container;

        public Ninject_Saga()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleSaga>()
                .ToSelf();
            _container.Bind(typeof(ISagaRepository<>))
                .To(typeof(InMemorySagaRepository<>))
                .InSingletonScope();
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
//            configurator.Saga
        }

//        protected override void SubscribeLocalBus(SubscriptionBusServiceConfigurator subscriptionBusServiceConfigurator)
//        {
//            // we have to do this explicitly, since the metadata is not exposed by Ninject
//            subscriptionBusServiceConfigurator.Saga<SimpleSaga>(_container);
//        }
    }
}