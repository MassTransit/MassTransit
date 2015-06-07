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
namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;
    using Internals.Extensions;
    using Internals.Reflection;


    /// <summary>
    /// Factory methods for decorating a saga repository so that properties of the saga
    /// can be set using the value provider specified.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public static class InjectingSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Creates a delegate saga repository that sets the designated property using the
        /// value returned by the value provider specified.
        /// </summary>
        /// <typeparam name="T1">The property type</typeparam>
        /// <param name="repository">The repository to decorate</param>
        /// <param name="propertyExpression">The property to set</param>
        /// <param name="valueProvider">The value provider for the property</param>
        /// <returns></returns>
        public static ISagaRepository<TSaga> Create<T1>(ISagaRepository<TSaga> repository,
            Expression<Func<TSaga, T1>> propertyExpression,
            Func<TSaga, T1> valueProvider)
        {
            var property = new ReadWriteProperty(propertyExpression.GetPropertyInfo());

            return new DelegatingSagaRepository<TSaga>(repository, context =>
            {
                T1 value = valueProvider(context.Saga);
                property.Set(context.Saga, value);
            });
        }

        /// <summary>
        /// Creates a delegate saga repository that sets the designated property using the
        /// value returned by the value provider specified.
        /// </summary>
        /// <typeparam name="T1">The property type</typeparam>
        /// <typeparam name="T2">The second property type</typeparam>
        /// <param name="repository">The repository to decorate</param>
        /// <param name="propertyExpression1">The first property to set</param>
        /// <param name="valueProvider1">The first property value provider</param>
        /// <param name="propertyExpression2">The second property to set</param>
        /// <param name="valueProvider2">The second value provider</param>
        /// <returns></returns>
        public static ISagaRepository<TSaga> Create<T1, T2>(ISagaRepository<TSaga> repository,
            Expression<Func<TSaga, T1>> propertyExpression1,
            Func<TSaga, T1> valueProvider1,
            Expression<Func<TSaga, T2>> propertyExpression2,
            Func<TSaga, T2> valueProvider2)
        {
            var property1 = new ReadWriteProperty<TSaga, T1>(propertyExpression1.GetPropertyInfo(), true);
            var property2 = new ReadWriteProperty<TSaga, T2>(propertyExpression2.GetPropertyInfo(), true);

            return new DelegatingSagaRepository<TSaga>(repository, context =>
            {
                T1 value = valueProvider1(context.Saga);
                property1.Set(context.Saga, value);

                T2 value2 = valueProvider2(context.Saga);
                property2.Set(context.Saga, value2);
            });
        }
    }
}