namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using MassTransit.Tests.ReliableMessaging;


    public class ReliableStateDefinition :
        SagaDefinition<ReliableState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<ReliableState> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseMessageScope(context);
            endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(context);
        }
    }
}
