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
namespace MassTransit.Tests.Saga
{
    using System;
    using Magnum.TestFramework;
    using MassTransit.Testing;
    using MassTransit.Testing.ScenarioBuilders;
    using NUnit.Framework;

    [TestFixture]
    public class When_completing_CustomerRegistrationSaga
    {
        [Test]
        public void Should_complete_saga()
        {
            test.Saga.AnyInState(CustomerRegistrationSaga.Completed).ShouldBeTrue();
        }

        [Test]
        public void Should_create_a_new_saga_for_the_message()
        {
            bool createdSaga = test.Saga.Created.Any(x => x.SystemId == systemId);
            createdSaga.ShouldBeTrue();
        }

        Guid systemId;
        SagaTest<BusTestScenario, CustomerRegistrationSaga> test;

        [TestFixtureSetUp]
        public void A_state_machine_saga_is_being_tested()
        {
            systemId = Guid.NewGuid();

            test = TestFactory.ForSaga<CustomerRegistrationSaga>()
                .InSingleBusScenario()
                .New(x =>
                    {
                        x.UseScenarioBuilder(() => new CustomBusScenarioBuilder());
                        x.Send(new ClientSystemCreatedEvent
                            {
                                SystemId = systemId
                            });
                        x.Send(new UserCreatedEvent
                            {
                                SystemId = systemId
                            });
                        x.Send(new SettingsCreatedEvent
                            {
                                SystemId = systemId
                            });
                        x.Send(new SystemActivatedEvent
                            {
                                SystemId = systemId
                            });
                    });

            test.Execute();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            test.Dispose();
            test = null;
        }

        public class CustomBusScenarioBuilder :
            BusScenarioBuilderImpl
        {
            const string DefaultUri = "loopback://localhost/mt_client";

            public CustomBusScenarioBuilder()
                : base(new Uri(DefaultUri))
            {
                ConfigureBus(x => x.SetConcurrentConsumerLimit(1));
            }
        }
    }
}