namespace MassTransit.QuartzIntegration.Tests
{
    namespace Request_Specs
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using Saga;
        using Testing;


        [TestFixture]
        [Category("Flaky")]
        public class Sending_a_request_from_a_state_machine :
            QuartzInMemoryTestFixture
        {
            [Test]
            public async Task Should_handle_the_response()
            {
                Task<ConsumeContext<MemberRegistered>> handler = await ConnectPublishHandler<MemberRegistered>();

                var memberNumber = Guid.NewGuid().ToString();

                await InputQueueSendEndpoint.Send<RegisterMember>(new
                {
                    MemberNumber = memberNumber,
                    Name = "Frank",
                    Address = "123 American Way"
                });

                ConsumeContext<MemberRegistered> registered = await handler;

                Guid? saga = await _repository.ShouldContainSagaInState(x => x.MemberNumber == memberNumber, _machine, x => x.Registered, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                var sagaInstance = _repository[saga.Value].Instance;
                Assert.IsFalse(sagaInstance.ValidateAddressRequestId.HasValue);
            }

            static Sending_a_request_from_a_state_machine()
            {
                _serviceAddress = new Uri("loopback://localhost/service_queue");
                EndpointConvention.Map<ValidateName>(_serviceAddress);
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;

            public Sending_a_request_from_a_state_machine()
            {
                _serviceQueueAddress = _serviceAddress;
            }

            Uri _serviceQueueAddress;
            static readonly Uri _serviceAddress;

            Uri ServiceQueueAddress
            {
                get => _serviceQueueAddress;
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

                _repository = new InMemorySagaRepository<TestState>();

                var settings = new RequestSettingsImpl(ServiceQueueAddress, QuartzAddress, TestTimeout);

                _machine = new TestStateMachine(settings);

                configurator.UseInMemoryOutbox();

                configurator.StateMachineSaga(_machine, _repository);
            }

            protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
            {
                configurator.Handler<ValidateAddress>(async context =>
                {
                    Console.WriteLine("Address validated: {0}", context.Message.CorrelationId);

                    if (context.IsResponseAccepted<AddressValidated>(false))
                        await context.RespondAsync<AddressValidated>(new { });

                    throw new InvalidOperationException("Response type not accepted");
                });

                configurator.Handler<ValidateName>(async context =>
                {
                    Console.WriteLine("Name validated: {0}", context.Message.CorrelationId);

                    await context.RespondAsync<NameValidated>(new {RequestName = context.Message.Name});
                });
            }
        }


        class RequestSettingsImpl :
            RequestSettings
        {
            public RequestSettingsImpl(Uri serviceAddress, Uri schedulingServiceAddress, TimeSpan timeout)
            {
                ServiceAddress = serviceAddress;
                SchedulingServiceAddress = schedulingServiceAddress;
                Timeout = timeout;
            }

            public Uri SchedulingServiceAddress { get; }

            public Uri ServiceAddress { get; }

            public TimeSpan Timeout { get; }
        }


        class TestState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public string MemberNumber { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }

            public Guid? ValidateAddressRequestId { get; set; }
            public Guid? ValidateNameRequestId { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public interface RegisterMember
        {
            string MemberNumber { get; }
            string Name { get; }
            string Address { get; }
        }


        public interface MemberRegistered
        {
            string Name { get; }
            string Address { get; }
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


        public interface ValidateName :
            CorrelatedBy<Guid>
        {
            string Name { get; }
        }


        public interface NameValidated :
            CorrelatedBy<Guid>
        {
            string Name { get; }

            string RequestName { get; }
        }


        class TestStateMachine :
            MassTransitStateMachine<TestState>
        {
            public TestStateMachine(RequestSettings settings)
            {
                Event(() => Register, x =>
                {
                    x.CorrelateBy(p => p.MemberNumber, p => p.Message.MemberNumber);
                    x.SelectId(context => NewId.NextGuid());
                });

                Request(() => ValidateAddress, x => x.ValidateAddressRequestId, settings);
                Request(() => ValidateName, x => x.ValidateNameRequestId, cfg =>
                {
                    cfg.Timeout = settings.Timeout;
                });

                Initially(When(Register)
                    .Then(context =>
                    {
                        Console.WriteLine("Registration received: {0}", context.Data.MemberNumber);

                        Console.WriteLine("TestState ID: {0}", context.Instance.CorrelationId);

                        context.Instance.Name = context.Data.Name;
                        context.Instance.Address = context.Data.Address;
                        context.Instance.MemberNumber = context.Data.MemberNumber;
                    })
                    .Request(ValidateAddress, x => ValidateAddress.Settings.ServiceAddress, x => x.Init<ValidateAddress>(x.Instance))
                    .TransitionTo(ValidateAddress.Pending));

                During(ValidateAddress.Pending,
                    When(ValidateAddress.Completed)
                        .ThenAsync(async context =>
                        {
                            await Console.Out.WriteLineAsync("Request Completed!");

                            context.Instance.Address = context.Data.Address;
                        })
                        .Request(ValidateName, context => context.Init<ValidateName>(context.Instance))
                        .TransitionTo(ValidateName.Pending),
                    When(ValidateAddress.Faulted)
                        .ThenAsync(async context => await Console.Out.WriteLineAsync("Request Faulted"))
                        .TransitionTo(AddressValidationFaulted),
                    When(ValidateAddress.TimeoutExpired)
                        .ThenAsync(async context => await Console.Out.WriteLineAsync("Request timed out"))
                        .TransitionTo(AddressValidationTimeout));

                During(ValidateName.Pending,
                    When(ValidateName.Completed)
                        .ThenAsync(async context =>
                        {
                            await Console.Out.WriteLineAsync("Request Completed!");

                            context.Instance.Name = context.Data.Name;
                        })
                        .PublishAsync(context => context.Init<MemberRegistered>(context.Instance))
                        .TransitionTo(Registered),
                    When(ValidateName.Faulted)
                        .ThenAsync(async context => await Console.Out.WriteLineAsync("Request Faulted"))
                        .TransitionTo(NameValidationFaulted),
                    When(ValidateName.TimeoutExpired)
                        .ThenAsync(async context => await Console.Out.WriteLineAsync("Request timed out"))
                        .TransitionTo(NameValidationTimeout));
            }

            // ReSharper disable UnassignedGetOnlyAutoProperty
            public Request<TestState, ValidateAddress, AddressValidated> ValidateAddress { get; }
            public Request<TestState, ValidateName, NameValidated> ValidateName { get; }

            public Event<RegisterMember> Register { get; }

            public State Registered { get; }
            public State AddressValidationFaulted { get; }
            public State AddressValidationTimeout { get; }

            public State NameValidationFaulted { get; }
            public State NameValidationTimeout { get; }
        }
    }
}
