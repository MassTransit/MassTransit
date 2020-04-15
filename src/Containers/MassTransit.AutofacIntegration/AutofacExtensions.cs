namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Autofac;
    using Autofac.Builder;
    using AutofacIntegration;
    using AutofacIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Internals.Extensions;
    using Pipeline.Filters;


    public static class AutofacExtensions
    {

        /// <summary>
        /// Creates a lifetime scope which is shared by any downstream filters (rather than creating a new one).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="name">The name of the lifetime scope</param>
        /// <param name="configureScope">Configuration for scope container</param>
        public static void UseLifetimeScope(this IPipeConfigurator<ConsumeContext> configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            var scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(lifetimeScope), name, configureScope);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Register an accessor for an input type in the container
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="propertyExpression"></param>
        public static IRegistrationBuilder<ILifetimeScopeIdAccessor<TInput, T>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeIdAccessor<TInput, T>(this ContainerBuilder builder, Expression<Func<TInput, T>> propertyExpression)
            where TInput : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var propertyInfo = propertyExpression.GetPropertyInfo();

            return builder.RegisterType<MessageLifetimeScopeIdAccessor<TInput, T>>()
                .As<ILifetimeScopeIdAccessor<TInput, T>>()
                .WithParameter(TypedParameter.From(propertyInfo));
        }

        /// <summary>
        /// Register a lifetime scope registry for the given identifier type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="scopeTag"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<ILifetimeScopeRegistry<string>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeRegistry<T>(this ContainerBuilder builder, object scopeTag)
        {
            return builder.RegisterType<LifetimeScopeRegistry<string>>()
                .As<ILifetimeScopeRegistry<string>>()
                .WithParameter("tag", scopeTag)
                .SingleInstance();
        }
    }
}
