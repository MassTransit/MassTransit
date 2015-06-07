// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using Reflection;


    public class ValueValueDictionaryDictionaryMapper<T, TKey, TValue> :
        IDictionaryMapper<T>
    {
        readonly ReadOnlyProperty<T> _property;

        public ValueValueDictionaryDictionaryMapper(ReadOnlyProperty<T> property)
        {
            _property = property;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            object value = _property.Get(obj);
            if (value == null)
                return;

            var values = value as IDictionary<TKey, TValue>;
            if (values == null)
                return;

            object[] elementArray = values.Select(element => new object[] {element.Key, element.Value})
                .ToArray<object>();

            dictionary.Add(_property.Property.Name, elementArray);
        }
    }
}