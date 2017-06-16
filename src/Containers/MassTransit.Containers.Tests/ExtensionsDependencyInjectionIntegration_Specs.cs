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
namespace MassTransit.Containers.Tests
{
    using System;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using SimpleInjectorIntegration;
    using Microsoft.Extensions.DependencyInjection;

    public class ExtensionsDependencyInjectionIntegration_Consumer :
        When_registering_a_consumer
    {
        readonly IServiceProvider _services;

        public ExtensionsDependencyInjectionIntegration_Consumer()
        {
            var collection = new ServiceCollection();

            collection.AddScoped<SimpleConsumer>();
            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _services = collection.BuildServiceProvider();
        }

        // [OneTimeTearDown]
        // public void Close_container()
        // {
        //     _container.Dispose();
        // }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_services);
        }
    }


    // public class ExtensionsDependencyInjectionIntegration_Saga :
    //     When_registering_a_saga
    // {
    //     [OneTimeTearDown]
    //     public void Close_container()
    //     {
    //         _container.Dispose();
    //     }

    //     readonly Container _container;

    //     public SimpleInjector_Saga()
    //     {
    //         _container = new Container();
    //         _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    //         _container.Register(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>),
    //             Lifestyle.Singleton);
    //     }

    //     protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
    //     {
    //         configurator.Saga<SimpleSaga>(_container);
    //     }

    //     protected override ISagaRepository<T> GetSagaRepository<T>()
    //     {
    //         return _container.GetInstance<ISagaRepository<T>>();
    //     }
    // }
}