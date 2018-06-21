// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.LamarIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes.Internals.Extensions;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using PipeConfigurators;
    using Saga;
    using Scoping;


    public static class LamarExtensions
    {
        static Type[] FindTypes<T>(IServiceContext container, Func<Type, bool> filter)
        {
            //return container
            //    .Model
            //    .ServiceTypes
            //    .Where(x => x.ServiceType.HasInterface<T>())
            //    .Select(i => i.ServiceType)
            //    .Concat(container.Model.InstancesOf<T>().Select(x => x.ImplementationType))
            //    .Where(filter)
            //    .Distinct()
            //    .ToArray();
            var serviceTypes = container.Model.ServiceTypes.Where(x => x.ServiceType.HasInterface<T>()).Select(x => x.ServiceType);
            var serviceInstanceTypes = serviceTypes.SelectMany(serviceType => container.Model.InstancesOf(serviceType)).Select(x => x.ImplementationType);
            var instanceTypes = container.Model.InstancesOf<T>().Select(x => x.ImplementationType);

            return serviceInstanceTypes
                .Concat(instanceTypes)
                .Where(filter)
                .Distinct()
                .ToArray();



        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The StructureMap container.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            var scopeProvider = new LamarConsumerScopeProvider(container);

            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);

            var sagaRepositoryFactory = new LamarSagaRepositoryFactory(container);

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="context"></param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceContext context)
        {
            var container = context.GetInstance<IContainer>();

            configurator.LoadFrom(container);
        }

        ///// <summary>
        ///// Specify that the service bus should load its subscribers from the container passed as an argument.
        ///// </summary>
        ///// <param name="configurator">The configurator the extension method works on.</param>
        ///// <param name="container">The Lamar container.</param>
        //public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContainer container)
        //{
        //    var scopeProvider = new LamarConsumerScopeProvider(container);

        //    var concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
        //    if (concreteTypes.Any())
        //    {
        //        foreach (var concreteType in concreteTypes)
        //        {
        //            ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);
        //        }
        //    }

        //    var sagaRepositoryFactory = new LamarSagaRepositoryFactory(container);
        //    var sagaTypes = FindTypes<ISaga>(container, x => true);
        //    if (!sagaTypes.Any())
        //        return;

        //    foreach (var sagaType in sagaTypes)
        //    {
        //        SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
        //    }
        //}

        ///// <summary>
        ///// Specify that the service bus should load its subscribers from the container passed as an argument.
        ///// </summary>
        ///// <param name="configurator">The configurator the extension method works on.</param>
        //public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceContext context)
        //{
        //    var container = context.GetInstance<IContainer>();
        //    configurator.LoadFrom(container);
        //}

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new ScopeConsumerFactory<T>(new LamarConsumerScopeProvider(container));
            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var repository = container.GetInstance<ISagaRepository<T>>();
            var sagaRepository = new ScopeSagaRepository<T>(repository, new LamarSagaScopeProvider<T>(container));

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress, IContainer container)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new LamarExecuteActivityScopeProvider<TActivity, TArguments>(container);
            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);
            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IContainer container)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new LamarExecuteActivityScopeProvider<TActivity, TArguments>(container);
            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);
            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IContainer container)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new LamarCompensateActivityScopeProvider<TActivity, TLog>(container);
            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);
            configurator.AddEndpointSpecification(specification);
        }

        public static void AddMasstransit(this ServiceRegistry serviceRegistry)
        {
            serviceRegistry.Injectable<LamarActivityFactory>();
            serviceRegistry.Injectable<ConsumeContext>();
        }

        internal static IContainer GetNestedContainer(this IContainer container, ConsumeContext context)
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Inject(LamarActivityFactory.Instance);
            nestedContainer.Inject(context);

            return nestedContainer;
        }

        internal static IContainer GetNestedContainer<T>(this IContainer container, ConsumeContext<T> context) where T : class
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Inject(LamarActivityFactory.Instance);
            
            nestedContainer.Configure(cfg =>
            {
                cfg.AddScoped(typeof(ConsumeContext), provider => context);
                cfg.AddScoped(typeof(ConsumeContext<T>), provider => context);
            });

            return nestedContainer;
        }
    }
}
