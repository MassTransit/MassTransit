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
namespace MassTransit.Internals.Mapping
{
    using System;
    using System.Collections.Concurrent;
    using GreenPipes.Internals.Reflection;


    /// <summary>
    /// Caches dictionary to object converters for the types requested, including the implementation
    /// builder for interfaces that are dynamically proxied
    /// </summary>
    public class DynamicObjectConverterCache :
        IObjectConverterCache
    {
        readonly ConcurrentDictionary<Type, IObjectConverter> _cache;
        readonly IImplementationBuilder _implementationBuilder;

        public DynamicObjectConverterCache(IImplementationBuilder implementationBuilder)
        {
            _implementationBuilder = implementationBuilder;
            _cache = new ConcurrentDictionary<Type, IObjectConverter>();
        }

        public IObjectConverter GetConverter(Type type)
        {
            return _cache.GetOrAdd(type, CreateMissingConverter);
        }

        IObjectConverter CreateMissingConverter(Type type)
        {
            Type implementationType = type.IsInterface ? _implementationBuilder.GetImplementationType(type) : type;
            Type converterType = typeof(DynamicObjectConverter<,>).MakeGenericType(type, implementationType);

            return (IObjectConverter)Activator.CreateInstance(converterType, this);
        }
    }
}