namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using DependencyInjection;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using UsageTracking;


    public class EventHubRegistrationRiderFactory :
        IRegistrationRiderFactory<IEventHubRider>
    {
        readonly Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> _configure;

        public EventHubRegistrationRiderFactory(Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new EventHubFactoryConfigurator();

            var usageTracker = context.GetService<IUsageTracker>();
            usageTracker?.PreConfigureRider(configurator);

            _configure?.Invoke(context, configurator);

            return configurator.Build(context);
        }
    }
}
