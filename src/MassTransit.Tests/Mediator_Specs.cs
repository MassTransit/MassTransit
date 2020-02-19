namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Delivering_a_message_via_the_mediator :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            var multiConsumer = new MultiTestConsumer(TimeSpan.FromSeconds(8));
            var received = multiConsumer.Consume<PingMessage>();

            var mediator = MassTransit.Bus.Factory.CreateInMemoryMediator(cfg =>
            {
                multiConsumer.Configure(cfg);
            });

            await mediator.Send(new PingMessage());

            Assert.That(received.Select().Any(), Is.True);
        }

        [Test]
        public async Task Should_fault_at_the_send()
        {
            var mediator = MassTransit.Bus.Factory.CreateInMemoryMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => throw new IntentionalTestException("No thank you!"));
            });

            Assert.That(async () => await mediator.Send(new PingMessage()), Throws.TypeOf<IntentionalTestException>());
        }

        [Test, Explicit]
        public async Task Should_deliver_messages_quickly()
        {
            var mediator = MassTransit.Bus.Factory.CreateInMemoryMediator(cfg =>
            {
                cfg.Handler<PingMessage>(context => TaskUtil.Completed);
            });

            var message = new PingMessage();

            Stopwatch timer = Stopwatch.StartNew();

            const int count = 200000;
            const int split = 20;

            for (int i = 0; i < count / split; i++)
            {
                await Task.WhenAll(Enumerable.Range(0, split).Select(_ => mediator.Send(message)));
            }

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }
    }
}
