namespace MassTransit
{
    using System;
    using RabbitMqTransport.Scheduling;
    using RabbitMqTransport.Topology;
    using Registration;
    using Scheduling;


    public static class RabbitMqMessageSchedulerExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the RabbitMQ Delayed Exchange plug-in to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateRabbitMqMessageScheduler(this IBus bus)
        {
            if (bus.Topology is IRabbitMqHostTopology topology)
                return new MessageScheduler(new DelayedExchangeScheduleMessageProvider(bus, topology), topology);

            throw new ArgumentException("A RabbitMQ bus is required to use the RabbitMQ message scheduler");
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses RabbitMq delayed exchange plug-in to
        /// schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddRabbitMqMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddMessageScheduler(new MessageSchedulerRegistration());
        }


        class MessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSingleInstance(provider => provider.GetRequiredService<IBus>().CreateRabbitMqMessageScheduler());
            }
        }
    }
}
