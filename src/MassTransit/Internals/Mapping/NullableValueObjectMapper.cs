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
    using System;
    using System.ComponentModel;
    using Reflection;


    public class NullableValueObjectMapper<T, TValue> :
        IObjectMapper<T>
        where TValue : struct
    {
        readonly ReadWriteProperty<T> _property;

        public NullableValueObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            object value;
            if (valueProvider.TryGetValue(_property.Property.Name, out value))
            {
                TValue? nullableValue = value as TValue?;
                if (!nullableValue.HasValue)
                {
                    var converter = TypeDescriptor.GetConverter(typeof(TValue));
                    nullableValue = converter.CanConvertFrom(value.GetType())
                        ? (TValue?)converter.ConvertFrom(value)
                        : null;
                }

                _property.Set(obj, nullableValue);
            }
        }
    }
}