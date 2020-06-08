namespace MassTransit
{
    using System;
    using EventHubIntegration;
    using EventHubIntegration.Registration;
    using Registration;


    public static class EventHubIntegrationExtensions
    {
        public static void UsingEventHub(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);
        }
    }
}
