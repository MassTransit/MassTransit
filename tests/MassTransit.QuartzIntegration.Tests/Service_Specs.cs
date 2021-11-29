namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_quartz_service_with_json :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

            await handlerA;
            await handlerIA;
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }
    }


    [TestFixture]
    public class Using_the_quartz_service_with_xml :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

            await handlerA;

            ConsumeContext<IA> context = await handlerIA;

            Assert.IsTrue(context.GetQuartzSent().HasValue);
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }


        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();

            base.ConfigureInMemoryBus(configurator);
        }
    }


    [TestFixture]
    public class Using_the_quartz_service_and_cancelling :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();

            ScheduledMessage<A> scheduledMessage =
                await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(3), new A {Name = "Joe"});

            await Task.Delay(1000);

            await Scheduler.CancelScheduledSend(scheduledMessage);

            Assert.That(async () => await handlerA.OrTimeout(5000), Throws.TypeOf<TimeoutException>());
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }
    }
}
