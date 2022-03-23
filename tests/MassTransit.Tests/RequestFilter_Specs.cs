namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_request_filter_with_request_client
    {
        [Test]
        public async Task Should_fault_instead_of_timeout()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<PingConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseConsumeFilter(typeof(RequestValidationScopedFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(1);

            await harness.Start();

            IRequestClient<PingMessage> client = harness.GetRequestClient<PingMessage>();

            Assert.That(async () =>
                await client.GetResponse<PongMessage>(new
                {
                    CorrelationId = InVar.Id,
                }), Throws.TypeOf<RequestFaultException>());
        }


        public class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        public class RequestValidationScopedFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("RequestValidationScopedFilter<TMessage> scope");
            }

            public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                try
                {
                    throw new IntentionalTestException("Failed to validate request");
                }
                catch (Exception exception)
                {
                    await context.NotifyFaulted(context.ReceiveContext.ElapsedTime, TypeCache<RequestValidationScopedFilter<T>>.ShortName, exception)
                        .ConfigureAwait(false);

                    throw;
                }
            }
        }
    }
}
