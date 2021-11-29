namespace MassTransit
{
    using System;
    using Configuration;
    using Middleware;


    public static class RescueConfigurationExtensions
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

        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TRescue"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="rescueContextFactory">Factory method to convert the pipe context to the rescue pipe context</param>
        /// <param name="configure"></param>
        public static void UseRescue<TContext, TRescue>(this IPipeConfigurator<TContext> configurator, IPipe<TRescue> rescuePipe,
            RescueContextFactory<TContext, TRescue> rescueContextFactory, Action<IRescueConfigurator<TContext, TRescue>> configure = null)
            where TContext : class, PipeContext
            where TRescue : class, TContext
        {
            UseRescue(configurator, rescueContextFactory, x =>
            {
                configure?.Invoke(x);

                x.UseFork(rescuePipe);
            });
        }

        /// <summary>
        /// Adds a filter to the pipe which is of a different type than the native pipe context type
        /// </summary>
        /// <typeparam name="TContext">The context type</typeparam>
        /// <typeparam name="TRescue">The filter context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="rescueContextFactory"></param>
        /// <param name="configure"></param>
        public static void UseRescue<TContext, TRescue>(this IPipeConfigurator<TContext> configurator,
            RescueContextFactory<TContext, TRescue> rescueContextFactory, Action<IRescueConfigurator<TContext, TRescue>> configure = null)
            where TContext : class, PipeContext
            where TRescue : class, TContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RescuePipeSpecification<TContext, TRescue>(rescueContextFactory);

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}
