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


    public class ReadWritePropertyCache<T> :
        IEnumerable<ReadWriteProperty<T>>
    {
        readonly IDictionary<string, ReadWriteProperty<T>> _properties;

        public ReadWritePropertyCache()
        {
            _properties = CreatePropertyCache(false);
        }

        public ReadWritePropertyCache(bool includeNonPublic)
        {
            _properties = CreatePropertyCache(includeNonPublic);
        }

        public IEnumerator<ReadWriteProperty<T>> GetEnumerator()
        {
            return _properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        static IDictionary<string, ReadWriteProperty<T>> CreatePropertyCache(bool includeNonPublic)
        {
            return new Dictionary<string, ReadWriteProperty<T>>(typeof(T).GetAllProperties()
                .Where(x => x.CanRead && (includeNonPublic || x.CanWrite))
                .Where(x => x.SetMethod != null)
                .Select(x => new ReadWriteProperty<T>(x, includeNonPublic))
                .ToDictionary(x => x.Property.Name));
        }

        public void Set(Expression<Func<T, object>> propertyExpression, T instance, object value)
        {
            _properties[propertyExpression.GetMemberName()].Set(instance, value);
        }

        public object Get(Expression<Func<T, object>> propertyExpression, T instance)
        {
            return _properties[propertyExpression.GetMemberName()].Get(instance);
        }
    }
}