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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;


    public class ReadOnlyPropertyCache<T> :
        IEnumerable<ReadOnlyProperty<T>>
    {
        readonly IDictionary<string, ReadOnlyProperty<T>> _properties;

        public ReadOnlyPropertyCache()
        {
            _properties = CreatePropertyCache();
        }

        public IEnumerator<ReadOnlyProperty<T>> GetEnumerator()
        {
            return _properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(string key, out ReadOnlyProperty<T> value)
        {
            try
            {
                return _properties.TryGetValue(key, out value);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"The read only property {key} was not found.");
            }
        }

        static IDictionary<string, ReadOnlyProperty<T>> CreatePropertyCache()
        {
            return new Dictionary<string, ReadOnlyProperty<T>>(typeof(T).GetAllProperties()
                .Where(x => x.CanRead)
                .Select(x => new ReadOnlyProperty<T>(x))
                .ToDictionary(x => x.Property.Name));
        }

        public object Get(Expression<Func<T, object>> propertyExpression, T instance)
        {
            return _properties[propertyExpression.GetMemberName()].Get(instance);
        }
    }
}