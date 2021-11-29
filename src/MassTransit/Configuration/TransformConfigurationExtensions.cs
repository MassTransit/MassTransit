namespace MassTransit
{
    using System;
    using Configuration;


    public static class TransformConfigurationExtensions
    {
        /// <summary>
        /// Apply a message transform, the behavior of which is defined inline using the configurator
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IConsumePipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new ConsumeTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="getSpecification"></param>
        public static void UseTransform<T>(this IConsumePipeConfigurator configurator,
            Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            IConsumeTransformSpecification<T> specification = getSpecification(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a message transform, the behavior of which is defined inline using the configurator
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new ConsumeTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="getSpecification"></param>
        public static void UseTransform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator,
            Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            IConsumeTransformSpecification<T> specification = getSpecification(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a transform on send to the message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this ISendPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new SendTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a transform on send to the message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IPublishPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new SendTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}
