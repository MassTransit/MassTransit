namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using GreenPipes;
    using Internals.Extensions;
    using MessageData;
    using Metadata;
    using Transformation.TransformConfigurators;


    public static class MessageDataConfiguratorExtensions
    {
        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configure"></param>
        public static void UseMessageData<T>(this IConsumePipeConfigurator configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configure?.Invoke(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configure"></param>
        public static void UseMessageData<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configure = null)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configure?.Invoke(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Load the message data as part of the transform (replaces the property on the original message, to avoid multiple
        /// loads of the same data).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="repository"></param>
        public static void LoadMessageData<T, TProperty>(this ITransformConfigurator<T> configurator, Expression<Func<T, TProperty>> propertyExpression,
            IMessageDataRepository repository)
            where T : class
        {
            var configuration = new LoadMessageDataTransformConfiguration<T, TProperty>(repository, propertyExpression.GetPropertyInfo());

            configuration.Apply(configurator);
        }

        static void LoadMessageData<T>(this ITransformConfigurator<T> configurator, IMessageDataRepository repository)
            where T : class
        {
            List<PropertyInfo> messageDataProperties = TypeMetadataCache<T>.Properties.Where(x => x.PropertyType.HasInterface<IMessageData>()).ToList();
            foreach (PropertyInfo messageDataProperty in messageDataProperties)
            {
                Type dataType = messageDataProperty.PropertyType.GetClosingArguments(typeof(MessageData<>)).First();
                Type providerType = typeof(LoadMessageDataTransformConfiguration<,>).MakeGenericType(typeof(T), dataType);
                var configuration = (IMessageDataTransformConfiguration<T>)Activator.CreateInstance(providerType, repository, messageDataProperty);

                configuration.Apply(configurator);
            }
        }
    }
}
