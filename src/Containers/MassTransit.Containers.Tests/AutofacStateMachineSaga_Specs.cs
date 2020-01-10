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
namespace MassTransit.Containers.Tests
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using TestFramework.Sagas;


    public class AutofacStateMachineSaga_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_first_event_successfully()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest {CorrelationId = NewId.NextGuid(), TestKey = "Unique"});

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        IContainer _container;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(InMemorySagaRepository<>))
                .As(typeof(ISagaRepository<>))
                .SingleInstance();

            builder.RegisterType<PublishTestStartedActivity>();

            builder.RegisterSagaStateMachines(typeof(TestStateMachineSaga).GetTypeInfo().Assembly);

            _container = builder.Build();

            configurator.LoadStateMachineSagas(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }
    }
}
