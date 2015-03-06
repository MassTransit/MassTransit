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
namespace MassTransit.AutomatonymousTests
{
    namespace Request_Specs
    {
        using System;
        using System.Threading.Tasks;
        using Automatonymous;
        using NUnit.Framework;
        using Saga;
        using TestFramework;


        [TestFixture]
        public class Sending_a_request_from_a_state_machine :
            StateMachineTestFixture
        {
            [Test]
            public async Task Should_handle_the_response()
            {
                Task<ConsumeContext<MemberRegistered>> handler = SubscribeHandler<MemberRegistered>();

                RegisterMember registerMember = new RegisterMemberCommand
                {
                    CorrelationId = NewId.NextGuid(),
                    Name = "Frank",
                    Address = "123 american way",
                };

                await InputQueueSendEndpoint.Send(registerMember);

                ConsumeContext<MemberRegistered> registered = await handler;

                Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == registerMember.CorrelationId
                    && GetCurrentState(x) == _machine.Registered, TestTimeout);
                Assert.IsTrue(saga.HasValue);

                TestState sagaInstance = _repository[saga.Value];
                Assert.IsTrue(sagaInstance.ValidateAddressRequestId.HasValue);
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;

            State GetCurrentState(TestState state)
            {
                return ((StateMachine<TestState>)_machine).InstanceStateAccessor.GetState(state);
            }

            public Sending_a_request_from_a_state_machine()
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

            protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
            {
                base.ConfigureBus(configurator);

                configurator.ReceiveEndpoint("service_queue", ConfigureServiceQueueEndpoint);
            }

            protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

                _repository = new InMemorySagaRepository<TestState>();

                var settings = new RequestSettingsImpl(ServiceQueueAddress, QuartzQueueAddress, TestTimeout);

                _machine = new TestStateMachine(settings);

                configurator.StateMachineSaga(_machine, _repository);
            }

            protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                configurator.Handler<ValidateAddress>(async context =>
                {
                    Console.WriteLine("Address validated: {0}", context.Message.CorrelationId);

                    context.Respond(new AddressValidatedResponse(context.Message));
                });
            }


        }


        class RegisterMemberCommand :
            RegisterMember
        {
            public Guid CorrelationId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
        }


        class RequestSettingsImpl :
            RequestSettings
        {
            readonly Uri _schedulingServiceAddress;
            readonly Uri _serviceAddress;
            readonly TimeSpan _timeout;

            public RequestSettingsImpl(Uri serviceAddress, Uri schedulingServiceAddress, TimeSpan timeout)
            {
                _serviceAddress = serviceAddress;
                _schedulingServiceAddress = schedulingServiceAddress;
                _timeout = timeout;
            }

            public Uri ServiceAddress
            {
                get { return _serviceAddress; }
            }

            public Uri SchedulingServiceAddress
            {
                get { return _schedulingServiceAddress; }
            }

            public TimeSpan Timeout
            {
                get { return _timeout; }
            }
        }


        class AddressValidatedResponse :
            AddressValidated
        {
            readonly ValidateAddress _message;

            public AddressValidatedResponse(ValidateAddress message)
            {
                _message = message;
            }

            public string Address
            {
                get { return _message.Address.ToUpperInvariant(); }
            }

            public string RequestAddress
            {
                get { return _message.Address; }
            }

            public Guid CorrelationId
            {
                get { return _message.CorrelationId; }
            }
        }

        class TestState :
            SagaStateMachineInstance
        {
            public TestState(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public TestState()
            {
            }

            public State CurrentState { get; set; }

            public string Name { get; set; }
            public string Address { get; set; }

            public Guid? ValidateAddressRequestId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public interface RegisterMember :
            CorrelatedBy<Guid>
        {
            string Name { get; }
            string Address { get; }
        }


        public interface MemberRegistered
        {
            string Name { get; }
            string Address { get; }
        }


        class MemberRegisteredImpl :
            MemberRegistered
        {
            readonly TestState _state;

            public MemberRegisteredImpl(TestState state)
            {
                _state = state;
            }

            public string Name
            {
                get { return _state.Name; }
            }

            public string Address
            {
                get { return _state.Address; }
            }
        }


        public interface ValidateAddress :
            CorrelatedBy<Guid>
        {
            string Address { get; }
        }


        public interface AddressValidated :
            CorrelatedBy<Guid>
        {
            string Address { get; }

            string RequestAddress { get; }
        }


        class ValidateAddressRequest :
            ValidateAddress
        {
            readonly RegisterMember _message;

            public ValidateAddressRequest(RegisterMember message)
            {
                _message = message;
            }

            public Guid CorrelationId
            {
                get { return _message.CorrelationId; }
            }

            public string Address
            {
                get { return _message.Address; }
            }
        }


        class TestStateMachine :
            MassTransitStateMachine<TestState>
        {
            public TestStateMachine(RequestSettings settings)
            {
                InstanceState(x => x.CurrentState);


                State(() => Registered);
                State(() => AddressValidationFaulted);
                State(() => AddressValidationTimeout);

                Event(() => Register, x => x.CorrelateById(context => context.Message.CorrelationId));

                Request(() => ValidateAddress, x => x.ValidateAddressRequestId, settings);

                Initially(
                    When(Register)
                        .Then(context =>
                        {
                            Console.WriteLine("Registration received: {0}", context.Data.CorrelationId);

                            Console.WriteLine("TestState ID: {0}", context.Instance.CorrelationId);

                            context.Instance.Name = context.Data.Name;
                        })
                        .Request(ValidateAddress, context => new ValidateAddressRequest(context.Message))
                        .TransitionTo(ValidateAddress.Pending));

                During(ValidateAddress.Pending,
                    When(ValidateAddress.Completed)
                        .Then(context =>
                        {
                            Console.WriteLine("Request Completed!");

                            context.Instance.Address = context.Data.Address;
                        })
                        .Publish(context => new MemberRegisteredImpl(context.Instance))
                        .TransitionTo(Registered),
                    When(ValidateAddress.Faulted)
                        .Then(context => Console.WriteLine("Request Faulted"))
                        .TransitionTo(AddressValidationFaulted),
                    When(ValidateAddress.TimeoutExpired)
                        .Then(context => Console.WriteLine("Request timed out"))
                        .TransitionTo(AddressValidationTimeout));
            }

            public Request<TestState, ValidateAddress, AddressValidated> ValidateAddress { get; private set; }

            public Event<RegisterMember> Register { get; private set; }

            public State Registered { get; private set; }
            public State AddressValidationFaulted { get; private set; }
            public State AddressValidationTimeout { get; private set; }
        }
    }
}