namespace MassTransit.UsageTracking;

public interface IUsageTracker
{
    public void PreConfigureBus<T>(T configurator, IBusRegistrationContext context)
        where T : IBusFactoryConfigurator;

    void PreConfigureRider<T>(T configurator)
        where T : IRiderFactoryConfigurator;
}
