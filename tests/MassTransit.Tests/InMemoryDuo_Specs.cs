namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Running_two_in_memory_transports
    {
        [Test]
        public async Task Should_keep_em_separated()
        {
            var internalHarness = new InMemoryTestHarness("internal");
            var externalHarness = new InMemoryTestHarness("external");

            ConsumerTestHarness<RelayConsumer> internalConsumer = internalHarness.Consumer(() => new RelayConsumer(externalHarness.Bus));
            ConsumerTestHarness<RelayConsumer> externalConsumer = externalHarness.Consumer(() => new RelayConsumer(internalHarness.Bus));

            ConsumerTestHarness<RealConsumer> realConsumer = internalHarness.Consumer<RealConsumer>();

            await internalHarness.Start();
            try
            {
                await externalHarness.Start();
                try
                {
                    await externalHarness.Bus.Publish(new A());

                    realConsumer.Consumed.Select<A>().Any().ShouldBeTrue();
                }
                finally
                {
                    await externalHarness.Stop();
                }
            }
            finally
            {
                await internalHarness.Stop();
            }
        }


        class RelayConsumer :
            IConsumer<A>
        {
            readonly IPublishEndpoint _otherHost;

            public RelayConsumer(IPublishEndpoint otherHost)
            {
                _otherHost = otherHost;
            }

            public Task Consume(ConsumeContext<A> context)
            {
                if (GetVirtualHost(context.SourceAddress) != GetVirtualHost(context.ReceiveContext.InputAddress))
                    return Task.CompletedTask;

                LogContext.Info?.Log("Forwarding message: {MessageId} from {SourceAddress}", context.MessageId, context.SourceAddress);

                IPipe<SendContext> contextPipe = new CopyContextPipe(context);

                return _otherHost.Publish(context.Message, contextPipe);
            }
        }


        static string GetVirtualHost(Uri address)
        {
            return address.AbsolutePath.Split('/').First(x => !string.IsNullOrWhiteSpace(x));
        }


        class RealConsumer :
            IConsumer<A>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return Task.CompletedTask;
            }
        }


        public class A
        {
        }
    }
}
