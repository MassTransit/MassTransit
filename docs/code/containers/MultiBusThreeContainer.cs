namespace MultiBusThreeContainer
{
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public interface IThirdBus :
        IBus
    {
    }

    class ThirdBus :
        BusInstance<IThirdBus>,
        IThirdBus
    {
        public ThirdBus(IBusControl busControl, ISomeService someService)
            : base(busControl)
        {
            SomeService = someService;
        }

        public ISomeService SomeService { get; }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit<IThirdBus>(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("third-host");
                });
            });
        }
    }

    public interface ISomeService
    {
    }
}
