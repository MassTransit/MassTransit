using NUnit.Framework;

namespace MassTransit.ServiceBus.MSMQ.Tests
{
    using System;
    using Exceptions;
    using Internal;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_MsmqMessageReceiver
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void I_should_not_allow_null_consumers()
        {
            IMsmqEndpoint mockEndpoint = mocks.CreateMock<IMsmqEndpoint>();
            MsmqMessageReceiver mr = new MsmqMessageReceiver(mockEndpoint);

            mr.Subscribe(null);
        }

        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void I_should_only_allow_one_consumer()
        {
            IMsmqEndpoint mockEndpoint = mocks.CreateMock<IMsmqEndpoint>();
            IEnvelopeConsumer mockConsumer = mocks.CreateMock<IEnvelopeConsumer>();
            IEnvelopeConsumer mockConsumer2 = mocks.CreateMock<IEnvelopeConsumer>();

            MsmqMessageReceiver mr = new MsmqMessageReceiver(mockEndpoint);

            mr.Subscribe(mockConsumer);
            mr.Subscribe(mockConsumer2);
        }
    }
}