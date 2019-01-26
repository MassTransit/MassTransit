// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scenarios.StateMachines;
    using TestFramework;


    public abstract class Common_SagaStateMachine :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_the_first_event()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest {CorrelationId = NewId.NextGuid(), TestKey = "Unique"});

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            ConfigureSagaStateMachine(configurator);
        }

        protected abstract void ConfigureSagaStateMachine(IInMemoryReceiveEndpointConfigurator configurator);
    }
}