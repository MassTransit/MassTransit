namespace MassTransit.Registration
{
    using Riders;


    public abstract class RegistrationRiderFactory<TRider> :
        IRegistrationRiderFactory<TRider>
        where TRider : IRider
    {
        public abstract IBusInstanceSpecification CreateRider(IRiderRegistrationContext context);

        protected void ConfigureRider(IRiderFactoryConfigurator configurator, IRiderRegistrationContext context)
        {
            context.UseHealthCheck(configurator);
        }
    }
}
