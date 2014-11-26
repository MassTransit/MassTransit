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
namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections.Concurrent;
    using Castle.Windsor;


    public static class ConsumerFactoryConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return InstanceCache.Cached.GetOrAdd(type, _ =>
                (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            GetOrAdd(consumerType).Configure(configurator, container);
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, IWindsorContainer container);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, IConsumer
        {
            public void Configure(IReceiveEndpointConfigurator configurator, IWindsorContainer container)
            {
                configurator.Consumer(new WindsorConsumerFactory<T>(container));
            }
        }


        static class InstanceCache
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Cached =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }
    }
}