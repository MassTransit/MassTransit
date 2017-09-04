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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using NUnit.Framework;
    using Request_Specs;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class Sending_a_request_that_times_out :
        StateMachineTestFixture
    {
        [Test]
        public async Task Should_receive_the_timeout_notification()
        {
            RegisterMember registerMember = new RegisterMemberCommand
            {
                MemberNumber = Guid.NewGuid().ToString(),
                Name = "Frank",
                Address = "123 american way",
            };

            await InputQueueSendEndpoint.Send(registerMember);

            Guid? saga = await _repository.ShouldContainSaga(x => x.MemberNumber == registerMember.MemberNumber
                && GetCurrentState(x) == _machine.AddressValidationTimeout, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        InMemorySagaRepository<Request_Specs.TestState> _repository;
        TestStateMachine _machine;

        State GetCurrentState(Request_Specs.TestState state)
        {
            return _machine.GetState(state).Result;
        }

        public Sending_a_request_that_times_out()
        {
            _serviceQueueAddress = new Uri("loopback://localhost/service_queue");
        }

        Uri _serviceQueueAddress;

        Uri ServiceQueueAddress
        {
            get { return _serviceQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _serviceQueueAddress = value;
            }
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint("service_queue", ConfigureServiceQueueEndpoint);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _repository = new InMemorySagaRepository<Request_Specs.TestState>();

            var settings = new RequestSettingsImpl(ServiceQueueAddress, QuartzQueueAddress, TimeSpan.FromSeconds(1));

            _machine = new TestStateMachine(settings);

            configurator.StateMachineSaga(_machine, _repository);
        }

        protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }
    }
}