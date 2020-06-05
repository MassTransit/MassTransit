namespace MassTransit.Registration
{
    using Riders;


    public abstract class RegistrationRiderFactory<TContainerContext, TRider> :
        IRegistrationRiderFactory<TContainerContext, TRider>
        where TContainerContext : class
        where TRider : IRider
    {
        public abstract IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context);

        protected void ConfigureRider(IRiderFactoryConfigurator configurator, IRiderRegistrationContext<TContainerContext> context)
        {
            context.UseHealthCheck(configurator);
        }
    }
}
