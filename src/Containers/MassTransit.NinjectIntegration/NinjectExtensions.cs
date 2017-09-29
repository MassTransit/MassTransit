// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ConsumeConfigurators;
    using Internals.Extensions;
    using Ninject;
    using NinjectIntegration;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Util;
    using Util.Scanning;


    /// <summary>
    /// For NInject, it seems that using named scopes is the way to get per-message implementations
    /// http://www.planetgeek.ch/2010/12/08/how-to-use-the-additional-ninject-scopes-of-namedscope/
    /// </summary>
    public static class NinjectExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="kernel">The Ninject kernel.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            var consumerCache = kernel.TryGet<IConsumerRegistry>();
            if (consumerCache != null)
            {
                foreach (var cachedConfigurator in consumerCache.GetConfigurators())
                {
                    cachedConfigurator.Configure(configurator, kernel);
                }
            }
            else
            {
                IList<Type> consumerTypes = FindTypes<IConsumer>(kernel, x => !x.HasInterface<ISaga>());
                if (consumerTypes.Count > 0)
                {
                    foreach (Type type in consumerTypes)
                        ConsumerConfiguratorCache.Configure(type, configurator, kernel);
                }
            }

            var sagaCache = kernel.TryGet<ISagaRegistry>();
            if (sagaCache != null)
            {
                foreach (var cachedConfigurator in sagaCache.GetConfigurators())
                {
                    cachedConfigurator.Configure(configurator, kernel);
                }
            }
            else
            {
                IList<Type> sagaTypes = FindTypes<ISaga>(kernel, x => true);
                if (sagaTypes.Count > 0)
                {
                    foreach (Type sagaType in sagaTypes)
                        SagaConfiguratorCache.Configure(sagaType, configurator, kernel);
                }
            }
        }

        public static void ConfigureMassTransit(this IKernel kernel, Action<IKernelConfigurator> configure = null)
        {
            var configurator = new KernelConfigurator(kernel);

            configure?.Invoke(configurator);
        }

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IKernel kernel, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new NinjectConsumerFactory<T>(kernel);

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IKernel kernel, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var sagaRepository = kernel.Get<ISagaRepository<T>>();

            var ninjectSagaRepository = new NinjectSagaRepository<T>(sagaRepository);

            configurator.Saga(ninjectSagaRepository, configure);
        }

        static IList<Type> FindTypes<T>(IKernel kernel, Func<Type, bool> filter)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            TypeSet types = AssemblyTypeCache.FindTypes(assemblies, typeof(T).IsAssignableFrom).Result;

            return types.AllTypes()
                .SelectMany(kernel.GetBindings)
                .Select(x => x.Service)
                .Distinct()
                .Where(filter)
                .ToList();
        }
    }
}