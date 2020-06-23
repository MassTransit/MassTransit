namespace UsageMediator
{
    using System;
    using System.Threading.Tasks;
    using UsageConsumer;
    using MassTransit;
    using MassTransit.Mediator;

    public class Program
    {
        public static async Task Main()
        {
            IMediator mediator = Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Consumer<SubmitOrderConsumer>();
            });
        }
    }
}