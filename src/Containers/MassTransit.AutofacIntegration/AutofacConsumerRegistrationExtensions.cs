namespace MassTransit
{
    using System.Reflection;
    using Autofac;
    using Autofac.Builder;
    using Autofac.Features.Scanning;
    using Metadata;
    using Saga;
    using Util;


    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with methods to support MassTransit.
    /// </summary>
    public static class AutofacConsumerRegistrationExtensions
    {
        /// <summary>
        /// Register types that implement <see cref="IConsumer"/> in the provided assemblies.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="consumerAssemblies">Assemblies to scan for consumers.</param>
        /// <returns>Registration builder allowing the consumer components to be customised.</returns>
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterConsumers(this ContainerBuilder builder, params Assembly[] consumerAssemblies)
        {
            return builder.RegisterAssemblyTypes(consumerAssemblies)
                .Where(TypeMetadataCache.HasConsumerInterfaces);
        }

        /// <summary>
        /// Register types that implement <see cref="IConsumer"/> in the provided assemblies.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="consumerAssemblies">Assemblies to scan for consumers.</param>
        /// <returns>Registration builder allowing the consumer components to be customised.</returns>
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterSagas(this ContainerBuilder builder, params Assembly[] consumerAssemblies)
        {
            return builder.RegisterAssemblyTypes(consumerAssemblies)
                .Where(TypeMetadataCache.HasSagaInterfaces);
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterInMemorySagaRepository(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(InMemorySagaRepository<>))
                .As(typeof(ISagaRepository<>))
                .SingleInstance();
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ContainerBuilder builder)
            where T : class, ISaga
        {
            builder.RegisterType<InMemorySagaRepository<T>>()
                .As<ISagaRepository<T>>()
                .SingleInstance();
        }
    }
}
