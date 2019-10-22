// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Definition;
    using Internals.Extensions;
    using Metadata;
    using Util;


    public static class ConsumerDefinitionRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(IConsumerDefinition<>)))
                throw new ArgumentException($"The type is not a consumer definition: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var consumerType = type.GetClosingArguments(typeof(IConsumerDefinition<>)).Single();

            return Cached.Instance.GetOrAdd(consumerType,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, consumerType)));
        }

        public static void Register(Type consumerDefinitionType, IContainerRegistrar registrar)
        {
            GetOrAdd(consumerDefinitionType).Register(registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
        }


        class CachedRegistration<TDefinition, TConsumer> :
            CachedRegistration
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterConsumerDefinition<TDefinition, TConsumer>();
            }
        }
    }
}
