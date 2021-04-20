namespace MassTransit
{
    using System;
    using Configurators;
    using Registration;


    public static class DelayedMessageSchedulerConfigurationExtensions
    {
        /// <summary>
        /// Use the built-in transport message delay to schedule messages
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseDelayedMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses transport message delay to schedule messages
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddDelayedMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddMessageScheduler(new MessageSchedulerRegistration());
        }


        class MessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.Register(provider =>
                {
                    var bus = provider.GetRequiredService<IBus>();
                    var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                    return sendEndpointProvider.CreateDelayedMessageScheduler(bus.Topology);
                });
            }
        }
    }
}
