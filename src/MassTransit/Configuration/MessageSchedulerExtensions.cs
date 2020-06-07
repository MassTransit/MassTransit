namespace MassTransit
{
    using System;
    using GreenPipes;
    using PipeConfigurators;
    using Registration;
    using Scheduling;


    public static class MessageSchedulerExtensions
    {
        /// <summary>
        /// Specify an endpoint to use for message scheduling
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerAddress"></param>
        public static void UseMessageScheduler(this IPipeConfigurator<ConsumeContext> configurator, Uri schedulerAddress)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new MessageSchedulerPipeSpecification(schedulerAddress);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Uses Publish (instead of Send) to schedule messages via the Quartz message scheduler. For this to work, a single
        /// queue should be used to schedule all messages. If multiple instances are running, they should be on the same Quartz
        /// cluster.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UsePublishMessageScheduler(this IPipeConfigurator<ConsumeContext> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new PublishMessageSchedulerPipeSpecification();

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that sends <see cref="ScheduleMessage{T}" />
        /// to an external message scheduler on the specified endpoint address, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address where the scheduler is running</param>
        public static void AddMessageScheduler(this IRegistrationConfigurator configurator, Uri schedulerEndpointAddress)
        {
            if (schedulerEndpointAddress == null)
                throw new ArgumentNullException(nameof(schedulerEndpointAddress));

            configurator.AddMessageScheduler(new EndpointMessageSchedulerRegistration(schedulerEndpointAddress));
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that publishes <see cref="ScheduleMessage{T}" />
        /// to an external message scheduler, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddPublishMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddMessageScheduler(new PublishEndpointMessageSchedulerRegistration());
        }


        class EndpointMessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            readonly Uri _schedulerEndpointAddress;

            public EndpointMessageSchedulerRegistration(Uri schedulerEndpointAddress)
            {
                _schedulerEndpointAddress = schedulerEndpointAddress;
            }

            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSingleInstance(provider => provider.GetRequiredService<IBus>().CreateMessageScheduler(_schedulerEndpointAddress));
            }
        }


        class PublishEndpointMessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSingleInstance(provider => provider.GetRequiredService<IBus>().CreateMessageScheduler());
            }
        }
    }
}
