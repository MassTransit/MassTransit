namespace MassTransit
{
    using Saga;
    using StructureMap;
    using StructureMapIntegration.Registration;


    public static class StructureMapExtensions
    {
        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ConfigurationExpression registry)
            where T : class, ISaga
        {
            var registrar = new StructureMapContainerRegistrar(registry);

            registrar.RegisterInMemorySagaRepository<T>();
        }

        internal static IContainer CreateNestedContainer(this IContainer container, ConsumeContext context)
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContainer container, SendContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<SendContext<T>>()
                    .Use(context);
                x.For<SendContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContext container, SendContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<SendContext<T>>()
                    .Use(context);
                x.For<SendContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContext container, PublishContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<PublishContext<T>>()
                    .Use(context);
                x.For<PublishContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContainer container, PublishContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<PublishContext<T>>()
                    .Use(context);
                x.For<PublishContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer(this IContext container, ConsumeContext context)
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContainer container, ConsumeContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);

                x.For<ConsumeContext<T>>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContext container, ConsumeContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);

                x.For<ConsumeContext<T>>()
                    .Use(context);
            });

            return nestedContainer;
        }
    }
}
