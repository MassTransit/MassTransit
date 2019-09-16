// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Internals.Extensions;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;
    using Util;


    public static class SagaClassMapping
    {
        public static void ConfigureDefaultSagaMap<T>(this ModelBuilder modelBuilder)
            where T : class, ISaga
        {
            var propertyInfo = typeof(T).GetProperty("CorrelationId");
            if (propertyInfo?.CanWrite == false)
                throw new ConfigurationException("The CorrelationId property must be read/write for use with Entity Framework. Add a setter to the property.");

            modelBuilder.Entity<T>().HasKey(t => t.CorrelationId);

            modelBuilder.Entity<T>().Property(t => t.CorrelationId)
                .ValueGeneratedNever();
        }

        public static void ConfigureDefaultSagaMaps(this ModelBuilder modelBuilder, params Type[] sagaTypes)
        {
            IEnumerable<Type> types = sagaTypes;

            ConfigureDefaultSagaMaps(modelBuilder, types);
        }

        public static void ConfigureDefaultSagaMaps(this ModelBuilder modelBuilder, IEnumerable<Type> sagaTypes)
        {
            foreach (var sagaType in sagaTypes)
            {
                if (!sagaType.HasInterface<ISaga>())
                    throw new ArgumentException($"Type {TypeMetadataCache.GetShortName(sagaType)} does not implement {nameof(ISaga)}\'", nameof(sagaTypes));

                MappingCache.Configure(modelBuilder, sagaType);
            }
        }


        static class MappingCache
        {
            static CachedConfigurator GetOrAdd(Type type)
            {
                return Cached.Instance.GetOrAdd(type, _ =>
                    (CachedConfigurator)Activator.CreateInstance(typeof(CachedDefaultMapping<>).MakeGenericType(type)));
            }

            public static void Configure(ModelBuilder modelBuilder, Type sagaType)
            {
                GetOrAdd(sagaType).Configure(modelBuilder);
            }


            static class Cached
            {
                internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                    new ConcurrentDictionary<Type, CachedConfigurator>();
            }


            interface CachedConfigurator
            {
                void Configure(ModelBuilder modelBuilder);
            }


            class CachedDefaultMapping<T> :
                CachedConfigurator
                where T : class, ISaga
            {
                public void Configure(ModelBuilder modelBuilder)
                {
                    modelBuilder.ConfigureDefaultSagaMap<T>();
                }
            }
        }
    }
}