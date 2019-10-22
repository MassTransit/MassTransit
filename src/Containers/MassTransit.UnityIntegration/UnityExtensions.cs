namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Courier;
    using Internals.Extensions;
    using PipeConfigurators;
    using Registration;
    using Saga;
    using Scoping;
    using Unity;
    using UnityIntegration;


    public static class UnityExtensions
    {
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IUnityContainer container)
        {
            var scopeProvider = new UnityConsumerScopeProvider(container);

            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
            {
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);
            }

            var sagaRepositoryFactory = new UnitySagaRepositoryFactory(container);

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
            }
        }

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IUnityContainer container,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new ScopeConsumerFactory<T>(new UnityConsumerScopeProvider(container));

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IUnityContainer container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var repository = container.Resolve<ISagaRepository<T>>();

            var scopeProvider = new UnitySagaScopeProvider<T>(container);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IUnityContainer container)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new UnityExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IUnityContainer container)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new UnityExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IUnityContainer container)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new UnityCompensateActivityScopeProvider<TActivity, TLog>(container);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        static IList<Type> FindTypes<T>(IUnityContainer container, Func<Type, bool> filter)
        {
            return container.Registrations
                .Where(r => r.MappedToType.HasInterface<T>())
                .Select(r => r.MappedToType)
                .Where(filter)
                .ToList();
        }
    }
}
