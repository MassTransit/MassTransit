// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum.TestFramework;
    using NUnit.Framework;
    using Ninject;
    using Scenarios;
    using SubscriptionConfigurators;

    [Scenario]
    [Explicit("Unfortunately, Ninject does not expose enough metadata to automatically load consumers")]
    public class Using_a_ninject_simple_consumer :
        When_resolving_a_simple_consumer
    {
        readonly IKernel _container;

        public Using_a_ninject_simple_consumer()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleConsumer>()
                .ToSelf();
        }

        [Finally]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void SubscribeLocalBus(SubscriptionBusServiceConfigurator subscriptionBusServiceConfigurator)
        {
            subscriptionBusServiceConfigurator.LoadFrom(_container);
        }
    }
}