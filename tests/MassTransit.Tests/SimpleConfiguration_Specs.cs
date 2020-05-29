namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;
    using TestFramework.Messages;


    [TestFixture]
    [Explicit]
    public class SimpleConfiguration_Specs
    {
        [Test]
        public void FirstTestName()
        {
            var busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.UseTransform<PingMessage>(v =>
                {
                });

                x.UseConcurrencyLimit(3);
                x.UseRateLimit(1000);
                x.UseTransaction();

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Saga(new InMemorySagaRepository<SimpleSaga>(), s =>
                    {
                        s.UseConcurrentMessageLimit(1);
                        s.UseRateLimit(1000);
                    });

                    e.Consumer<MyConsumer>(c =>
                    {
                        c.UseConcurrentMessageLimit(1);
                        c.UseRateLimit(100);
                    });

                    e.UseTransaction();
                    e.UseConcurrencyLimit(7);
                    e.UseRateLimit(100);
                });
            });
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
