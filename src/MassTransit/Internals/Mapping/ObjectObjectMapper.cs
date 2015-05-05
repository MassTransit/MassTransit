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


    public class ObjectObjectMapper<T> :
        IObjectMapper<T>
    {
        readonly IObjectConverter _converter;
        readonly ReadWriteProperty<T> _property;
        readonly TypeConverter _typeConverter;

        public ObjectObjectMapper(ReadWriteProperty<T> property, IObjectConverter converter)
        {
            _property = property;
            _converter = converter;
            _typeConverter = TypeDescriptor.GetConverter(property.Property.PropertyType);
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            IObjectValueProvider propertyProvider;
            if (valueProvider.TryGetValue(_property.Property.Name, out propertyProvider))
            {
                object value = _converter.GetObject(propertyProvider);
                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (!valueType.IsInstanceOfType(_property.Property.PropertyType))
                    {
                        if (_typeConverter.IsValid(value))
                        {
                            if (_typeConverter.CanConvertFrom(valueType))
                                value = _typeConverter.ConvertFrom(value);
                        }
                    }
                }

                _property.Set(obj, value);
            }
        }
    }
}