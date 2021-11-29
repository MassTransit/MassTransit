namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public class Accessing_a_scope_from_middleware :
        InMemoryTestFixture
    {
        readonly IServiceProvider _provider;

        public Accessing_a_scope_from_middleware()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.AddConsumer<Consumer>();
                    x.AddBus(provider => BusControl);
                })
                .BuildServiceProvider(true);
        }

        [Test]
        public async Task Should_respond_successfully()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>();

            await client.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConnectConsumerConfigurationObserver(new FilterConfigurationObserver());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_provider.GetRequiredService<IBusRegistrationContext>());
        }


        class FilterConfigurationObserver :
            IConsumerConfigurationObserver
        {
            void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            {
            }

            void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            {
                configurator.UseFilter(new Filter<TConsumer, TMessage>());
            }
        }


        class Filter<TConsumer, T> :
            IFilter<ConsumerConsumeContext<TConsumer, T>>
            where T : class
            where TConsumer : class
        {
            public async Task Send(ConsumerConsumeContext<TConsumer, T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            {
                var scope = context.GetPayload<IServiceScope>();

                await next.Send(context).ConfigureAwait(false);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}
