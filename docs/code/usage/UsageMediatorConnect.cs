namespace UsageMediatorConnect
{
    using System.Threading.Tasks;
    using UsageConsumer;
    using Microsoft.Extensions.DependencyInjection;
    using MassTransit;
    using MassTransit.Mediator;

    public class Program
    {
        public static async Task Main()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg => { })
                .BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            var handle = mediator.ConnectConsumer<SubmitOrderConsumer>();
        }
    }
}
