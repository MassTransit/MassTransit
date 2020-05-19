namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using NUnit.Framework;
    using Request_Specs;
    using Saga;
    using Testing;


    [TestFixture]
    public class Sending_a_request_that_times_out :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_timeout_notification()
        {
            var memberNumber = Guid.NewGuid().ToString();

            await InputQueueSendEndpoint.Send<RegisterMember>(new { MemberNumber = memberNumber, Name = "Frank", Address = "123 American Way"});

            Guid? saga = await _repository.ShouldContainSaga(x => x.MemberNumber == memberNumber
                && GetCurrentState(x) == _machine.AddressValidationTimeout, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        InMemorySagaRepository<TestState> _repository;
        TestStateMachine _machine;

        State GetCurrentState(TestState state)
        {
            return _machine.GetState(state).Result;
        }

        public Sending_a_request_that_times_out()
        {
            _serviceQueueAddress = new Uri("loopback://localhost/service_queue");
            EndpointConvention.Map<ValidateName>(_serviceQueueAddress);
        }

        Uri _serviceQueueAddress;

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

            var settings = new RequestSettingsImpl(ServiceQueueAddress, HangfireAddress, TimeSpan.FromSeconds(1));

            _machine = new TestStateMachine(settings);

            configurator.StateMachineSaga(_machine, _repository);
        }

        protected virtual void ConfigureServiceQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }
    }
}
