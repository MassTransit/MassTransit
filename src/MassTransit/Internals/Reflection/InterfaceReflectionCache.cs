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
namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Metadata;


    public class InterfaceReflectionCache
    {
        readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Type>> _cache;

        public InterfaceReflectionCache()
        {
            _cache = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Type>>();
        }

        public Type GetGenericInterface(Type type, Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException($"The interface must be a generic interface definition: {TypeMetadataCache.GetShortName(interfaceType)}",
                    nameof(interfaceType));

            // our contract states that we will not return generic interface definitions without generic type arguments
            if (type == interfaceType)
                return null;
            if (type.GetTypeInfo().IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == interfaceType)
                    return type;
            }

            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();

            return interfaces.Where(t => t.GetTypeInfo().IsGenericType)
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == interfaceType);
        }

        public Type Get(Type type, Type interfaceType)
        {
            ConcurrentDictionary<Type, Type> typeCache = _cache.GetOrAdd(type, x => new ConcurrentDictionary<Type, Type>());

            return typeCache.GetOrAdd(interfaceType, x => GetInterfaceInternal(type, interfaceType));
        }

        Type GetInterfaceInternal(Type type, Type interfaceType)
        {
            if (interfaceType.GetTypeInfo().IsGenericTypeDefinition)
                return GetGenericInterface(type, interfaceType);

            Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray();

            return interfaces.FirstOrDefault(t => t == interfaceType);
        }
    }
}
