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
namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Concurrent;
    using Util;


    public class ConventionTypeCache<TValue> :
        IConventionTypeCache<TValue>
        where TValue : class
    {
        readonly ConcurrentDictionary<Type, Cached> _dictionary;
        readonly IConventionTypeCacheFactory<TValue> _typeFactory;

        public ConventionTypeCache(IConventionTypeCacheFactory<TValue> typeFactory)
        {
            _typeFactory = typeFactory ?? throw new ArgumentNullException(nameof(typeFactory));

            _dictionary = new ConcurrentDictionary<Type, Cached>();
        }

        TResult IConventionTypeCache<TValue>.GetOrAdd<T, TResult>()
        {
            var result = _dictionary.GetOrAdd(typeof(T), add => new CachedValue(() => _typeFactory.Create<T>())).Value as TResult;
            if (result == null)
                throw new ArgumentException($"The specified result type was invalid: {TypeMetadataCache<TResult>.ShortName}");

            return result;
        }


        interface Cached
        {
            TValue Value { get; }
        }


        class CachedValue :
            Cached
        {
            readonly Lazy<TValue> _value;

            public CachedValue(Func<TValue> valueFactory)
            {
                _value = new Lazy<TValue>(valueFactory);
            }

            public TValue Value => _value.Value;
        }
    }
}