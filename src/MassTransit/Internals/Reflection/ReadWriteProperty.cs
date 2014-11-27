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
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;


    public class ReadWriteProperty :
        ReadOnlyProperty
    {
        public readonly Action<object, object> SetProperty;

        public ReadWriteProperty(PropertyInfo property)
            : this(property, false)
        {
        }

        public ReadWriteProperty(PropertyInfo property, bool includeReadOnly)
            : base(property)
        {
            SetProperty = GetSetMethod(Property, includeReadOnly);
        }

        public void Set(object instance, object value)
        {
            SetProperty(instance, value);
        }

        static Action<object, object> GetSetMethod(PropertyInfo property, bool includeNonPublic)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            UnaryExpression instanceCast;
            if (property.DeclaringType.GetTypeInfo().IsValueType)
                instanceCast = Expression.Convert(instance, property.DeclaringType);
            else
                instanceCast = Expression.TypeAs(instance, property.DeclaringType);

            UnaryExpression valueCast;
            if (property.PropertyType.GetTypeInfo().IsValueType)
                valueCast = Expression.Convert(value, property.PropertyType);
            else
                valueCast = Expression.TypeAs(value, property.PropertyType);

            MethodCallExpression call = Expression.Call(instanceCast, property.SetMethod, valueCast);

            return Expression.Lambda<Action<object, object>>(call, new[] {instance, value}).Compile();
        }
    }


    public class ReadWriteProperty<T> :
        ReadOnlyProperty<T>
    {
        public readonly Action<T, object> SetProperty;

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo(), false)
        {
        }

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression, bool includeNonPublic)
            : this(propertyExpression.GetPropertyInfo(), includeNonPublic)
        {
        }

        public ReadWriteProperty(PropertyInfo property)
            : this(property, false)
        {
        }

        public ReadWriteProperty(PropertyInfo property, bool includeNonPublic)
            : base(property)
        {
            SetProperty = GetSetMethod(Property, includeNonPublic);
        }

        public void Set(T instance, object value)
        {
            SetProperty(instance, value);
        }

        static Action<T, object> GetSetMethod(PropertyInfo property, bool includeNonPublic)
        {
            if (!property.CanWrite)
            {
                return (x, i) =>
                {
                    throw new InvalidOperationException("No setter available on " + property.Name);
                };
            }

            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            UnaryExpression valueCast;
            if (property.PropertyType.GetTypeInfo().IsValueType)
                valueCast = Expression.Convert(value, property.PropertyType);
            else
                valueCast = Expression.TypeAs(value, property.PropertyType);
            MethodCallExpression call = Expression.Call(instance, property.SetMethod, valueCast);

            return Expression.Lambda<Action<T, object>>(call, new[] {instance, value}).Compile();
        }
    }


    public class ReadWriteProperty<T, TProperty> :
        ReadOnlyProperty<T, TProperty>
    {
        public readonly Action<T, TProperty> SetProperty;

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo(), false)
        {
        }

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression, bool includeNonPublic)
            : this(propertyExpression.GetPropertyInfo(), includeNonPublic)
        {
        }

        public ReadWriteProperty(PropertyInfo property)
            : this(property, false)
        {
        }

        public ReadWriteProperty(PropertyInfo property, bool includeNonPublic)
            : base(property)
        {
            SetProperty = GetSetMethod(Property, includeNonPublic);
        }

        public void Set(T instance, TProperty value)
        {
            SetProperty(instance, value);
        }

        static Action<T, TProperty> GetSetMethod(PropertyInfo property, bool includeNonPublic)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(TProperty), "value");
            MethodCallExpression call = Expression.Call(instance, property.SetMethod, value);
            return Expression.Lambda<Action<T, TProperty>>(call, new[] {instance, value}).Compile();
        }
    }
}