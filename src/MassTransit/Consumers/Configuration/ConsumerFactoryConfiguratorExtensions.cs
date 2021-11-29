namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;


    public static class ConsumerFactoryConfiguratorExtensions
    {
        public static IEnumerable<ValidationResult> ValidateConsumer<TConsumer>(this ISpecification configurator)
            where TConsumer : class
        {
            if (!typeof(TConsumer).HasInterface<IConsumer>())
            {
                yield return configurator.Warning("Consumer",
                    $"The consumer class {TypeCache<TConsumer>.ShortName} does not implement any IConsumer interfaces");
            }

            IEnumerable<ValidationResult> warningForMessages = ConsumerMetadataCache<TConsumer>
                .ConsumerTypes
                .Where(x => !IntrospectionExtensions.GetTypeInfo(x.MessageType).IsInterface)
                .Where(x => !(HasProtectedDefaultConstructor(x.MessageType) || HasSinglePublicConstructor(x.MessageType)))
                .Select(x =>
                    $"The {TypeCache.GetShortName(x.MessageType)} message should have a public or protected default constructor."
                    + " Without an available constructor, MassTransit will initialize new message instances"
                    + " without calling a constructor, which can lead to unpredictable behavior if the message"
                    + " depends upon logic in the constructor to be executed.")
                .Select(message => configurator.Warning("Message", message));

            foreach (var message in warningForMessages)
                yield return message;
        }

        public static IEnumerable<ValidationResult> Validate<TConsumer>(this IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class
        {
            if (consumerFactory == null)
                yield return ValidationResultExtensions.Failure(null, "UseConsumerFactory", "must not be null");

            foreach (var result in ValidateConsumer<TConsumer>(null))
                yield return result;
        }

        static bool HasProtectedDefaultConstructor(Type type)
        {
            return type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(constructorInfo => !constructorInfo.GetParameters().Any());
        }

        static bool HasSinglePublicConstructor(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .All(constructorInfo => !constructorInfo.GetParameters().Any())
                && type.GetConstructors().Length == 1;
        }
    }
}
