// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using Ninject;
    using Saga;
    using Scenarios;
    using SubscriptionConfigurators;
    using Testing;

    /// <summary>
    /// Note:
    /// 
    /// Ninject doesn't use the same tests as the other containers due to this issue:
    /// 
    /// https://github.com/ninject/Ninject/issues/125
    /// 
    /// The original tests from When_registering_a_consumer can be made to pass if activation blocks are used, however in a production scenario this setup fails
    /// as activation blocks are broken and will dispose the activation block, not just the consumer.
    /// 
    /// In theory using a named scope would allow this test to pass as well, however that also doesn't seem to work in practice. Hopefully a better solution will
    /// arise in Ninject 4.0
    /// </summary>
    [Scenario]
    public class Ninject_Consumer :
        Given_a_service_bus_instance
    {
        readonly IKernel _container;

        public Ninject_Consumer()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleConsumer>()
                      .ToSelf();
            _container.Bind<ISimpleConsumerDependency>()
                      .To<SimpleConsumerDependency>();
            _container.Bind<AnotherMessageConsumer>()
                      .To<AnotherMessageConsumerImpl>();
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

        [Then]
        public void Should_have_a_subscription_for_the_consumer_message_type()
        {
            LocalBus.HasSubscription<SimpleMessageInterface>().Count()
                    .ShouldEqual(1, "No subscription for the SimpleMessageInterface was found.");
        }

        [Then]
        public void Should_have_a_subscription_for_the_nested_consumer_type()
        {
            LocalBus.HasSubscription<AnotherMessageInterface>().Count()
                    .ShouldEqual(1, "Only one subscription should be registered for another consumer");
        }

        [Then]
        public void Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            var complete = new ManualResetEvent(false);

            LocalBus.SubscribeHandler<SimpleMessageClass>(x => complete.Set());
            LocalBus.Publish(new SimpleMessageClass(name));

            complete.WaitOne(8.Seconds());

            SimpleConsumer lastConsumer = SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBeNull();

            lastConsumer.Last.Name
                .ShouldEqual(name);

            lastConsumer.Dependency.SomethingDone
                .ShouldBeTrue("Dependency was disposed before consumer executed");
        }

    }


    [Scenario]
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

        [Finally]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void SubscribeLocalBus(SubscriptionBusServiceConfigurator subscriptionBusServiceConfigurator)
        {
            // we have to do this explicitly, since the metadata is not exposed by Ninject
            subscriptionBusServiceConfigurator.Saga<SimpleSaga>(_container);
        }
    }
}