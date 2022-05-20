namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class ExceptionInfo_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_a_single_fault_when_retried()
        {
            Task<ConsumeContext<Fault<PingMessage>>> faulted = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<Fault<PingMessage>> fault = await faulted;

            Assert.That(fault.Message.Exceptions.Length, Is.EqualTo(1));

            var exceptionInfo = fault.Message.Exceptions.Single();

            Assert.That(exceptionInfo.ExceptionType, Is.EqualTo(TypeCache<IntentionalTestException>.ShortName));

            Assert.That(exceptionInfo.Data.TryGetValue("Username", out string username) ? username : "", Is.EqualTo("Frank"));
            Assert.That(exceptionInfo.Data.TryGetValue("CustomerId", out long? customerId) ? customerId : 0, Is.EqualTo(27));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<PingConsumer>();
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                try
                {
                    throw new IntentionalTestException("This was intentional");
                }
                catch (Exception exception)
                {
                    throw new MassTransitApplicationException(exception, new
                    {
                        Username = "Frank",
                        CustomerId = 27
                    });
                }
            }
        }
    }


    [TestFixture]
    public class Original_exception_data_should_be_included :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_a_single_fault_when_retried()
        {
            Task<ConsumeContext<Fault<PingMessage>>> faulted = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<Fault<PingMessage>> fault = await faulted;

            Assert.That(fault.Message.Exceptions.Length, Is.EqualTo(1));

            var exceptionInfo = fault.Message.Exceptions.Single();

            Assert.That(exceptionInfo.ExceptionType, Is.EqualTo(TypeCache<IntentionalTestException>.ShortName));

            Assert.That(exceptionInfo.Data.TryGetValue("Username", out string username) ? username : "", Is.EqualTo("Frank"));
            Assert.That(exceptionInfo.Data.TryGetValue("CustomerId", out long? customerId) ? customerId : 0, Is.EqualTo(27));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<PingConsumer>();
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                var exception = new IntentionalTestException("This was intentional");
                exception.Data.Add("Username", "Frank");
                exception.Data.Add("CustomerId", 27);
                throw exception;
            }
        }
    }
}
