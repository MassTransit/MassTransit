namespace GreenPipes
{
    using System;
    using Configurators;
    using MassTransit;
    using MassTransit.PipeConfigurators;
    using MassTransit.Saga;


    public static class RescueFilterConfiguratorExtensions
    {
        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="configure"></param>
        public static void UseRescue(this IPipeConfigurator<ReceiveContext> configurator, IPipe<ExceptionReceiveContext> rescuePipe,
            Action<IExceptionConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var rescueConfigurator = new ReceiveContextRescuePipeSpecification(rescuePipe);

            configure?.Invoke(rescueConfigurator);

            configurator.AddPipeSpecification(rescueConfigurator);
        }

        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="configure"></param>
        public static void UseRescue(this IPipeConfigurator<ConsumeContext> configurator, IPipe<ExceptionConsumeContext> rescuePipe,
            Action<IExceptionConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var rescueConfigurator = new ConsumeContextRescuePipeSpecification(rescuePipe);

            configure?.Invoke(rescueConfigurator);

            configurator.AddPipeSpecification(rescueConfigurator);
        }

        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="configure"></param>
        public static void UseRescue<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IPipe<ExceptionConsumeContext<T>> rescuePipe,
            Action<IExceptionConfigurator> configure = null)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var rescueConfigurator = new ConsumeContextRescuePipeSpecification<T>(rescuePipe);

            configure?.Invoke(rescueConfigurator);

            configurator.AddPipeSpecification(rescueConfigurator);
        }

        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="configure"></param>
        public static void UseRescue<T>(this IPipeConfigurator<ConsumerConsumeContext<T>> configurator, IPipe<ExceptionConsumerConsumeContext<T>> rescuePipe,
            Action<IExceptionConfigurator> configure = null)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var rescueConfigurator = new ConsumerConsumeContextRescuePipeSpecification<T>(rescuePipe);

            configure?.Invoke(rescueConfigurator);

            configurator.AddPipeSpecification(rescueConfigurator);
        }

        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="configure"></param>
        public static void UseRescue<T>(this IPipeConfigurator<SagaConsumeContext<T>> configurator, IPipe<ExceptionSagaConsumeContext<T>> rescuePipe,
            Action<IExceptionConfigurator> configure = null)
            where T : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var rescueConfigurator = new SagaConsumeContextRescuePipeSpecification<T>(rescuePipe);

            configure?.Invoke(rescueConfigurator);

            configurator.AddPipeSpecification(rescueConfigurator);
        }
    }
}
