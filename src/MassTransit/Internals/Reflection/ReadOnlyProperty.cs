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
namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;


    public class ReadOnlyProperty
    {
        public readonly Func<object, object> GetProperty;

        public ReadOnlyProperty(PropertyInfo property)
        {
            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public object Get(object instance)
        {
            return GetProperty(instance);
        }

        static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            if (property.DeclaringType == null)
                throw new ArgumentException("DeclaringType is null", nameof(property));

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instanceCast = property.DeclaringType.GetTypeInfo().IsValueType
                ? Expression.Convert(instance, property.DeclaringType)
                : Expression.TypeAs(instance, property.DeclaringType);

            MethodCallExpression call = Expression.Call(instanceCast, property.GetMethod);
            UnaryExpression typeAs = Expression.TypeAs(call, typeof(object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
    }


    public class ReadOnlyProperty<T>
    {
        public readonly Func<T, object> GetProperty;

        public ReadOnlyProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadOnlyProperty(PropertyInfo property)
        {
            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public object Get(T instance)
        {
            return GetProperty(instance);
        }

        static Func<T, object> GetGetMethod(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            MethodCallExpression call = Expression.Call(instance, property.GetMethod);
            UnaryExpression typeAs = Expression.TypeAs(call, typeof(object));
            return Expression.Lambda<Func<T, object>>(typeAs, instance).Compile();
        }
    }


    public class ReadOnlyProperty<T, TProperty>
    {
        public readonly Func<T, TProperty> GetProperty;

        public ReadOnlyProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadOnlyProperty(PropertyInfo property)
        {
            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public TProperty Get(T instance)
        {
            return GetProperty(instance);
        }

        static Func<T, TProperty> GetGetMethod(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            MethodCallExpression call = Expression.Call(instance, property.GetMethod);

            return Expression.Lambda<Func<T, TProperty>>(call, instance).Compile();
        }
    }
}