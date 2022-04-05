namespace MassTransit.Containers.Tests
{
    namespace MediatorFilter
    {
        using System.Threading.Tasks;
        using Mediator;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;


        public class SubmitOrder
        {
        }


        public class OrderStatus
        {
            public string Text { get; set; }
        }


        public class SubmitFilter<T> : IFilter<ConsumeContext<T>>
            where T : class
        {
            public void Probe(ProbeContext context)
            {
            }

            public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                await context.NotifyConsumed(context.ReceiveContext.ElapsedTime, TypeCache<SubmitFilter<T>>.ShortName);

                await context.RespondAsync(new OrderStatus { Text = "It works from filter" });
            }
        }


        public class SubmitOrderConsumer : IConsumer<SubmitOrder>
        {
            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                await context.RespondAsync(new OrderStatus { Text = "It works" });
            }
        }


        [TestFixture]
        public class Using_mediator_with_a_filter_to_respond
        {
            [Test]
            public async Task And_also_notify_consumed()
            {
                var services = new ServiceCollection();

                services.AddMediator(cfg =>
                {
                    cfg.AddConsumer<SubmitOrderConsumer>();

                    cfg.ConfigureMediator((context, config) =>
                    {
                        config.UseConsumeFilter(typeof(SubmitFilter<>), context);
                    });
                });

                var provider = services.BuildServiceProvider(true);
                var mediator = provider.GetRequiredService<IMediator>();
                IRequestClient<SubmitOrder> client = mediator.CreateRequestClient<SubmitOrder>();

                Response<OrderStatus> response = await client.GetResponse<OrderStatus>(new SubmitOrder());
            }
        }
    }
}
