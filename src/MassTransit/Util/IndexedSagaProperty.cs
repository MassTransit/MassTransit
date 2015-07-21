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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Saga;


    /// <summary>
    /// For the in-memory saga repository, this maintains an index of saga properties
    /// for fast searching
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public interface IndexedSagaProperty<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Returns the saga with the specified key
        /// </summary>
        /// <param name="key"></param>
        SagaInstance<TSaga> this[object key] { get; }

        int Count { get; }

        /// <summary>
        /// Adds a new saga to the index
        /// </summary>
        /// <param name="newItem"></param>
        void Add(TSaga newItem);

        /// <summary>
        /// Removes a saga from the index
        /// </summary>
        /// <param name="item"></param>
        void Remove(SagaInstance<TSaga> item);

        /// <summary>
        /// Returns sagas matching the filter function
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter);

        /// <summary>
        /// Returns sagas matching the filter function where the key also matches
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter);

        /// <summary>
        /// Selects sagas from the index, running the transformation function and returning the output type
        /// </summary>
        /// <param name="transformer"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer);
    }


    /// <summary>
    /// A dictionary index of the sagas
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class IndexedSagaProperty<TSaga, TProperty> :
        IndexedSagaProperty<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<TSaga, TProperty> _getProperty;
        readonly IDictionary<TProperty, HashSet<SagaInstance<TSaga> >> _values;

        /// <summary>
        /// Creates an index for the specified property of a saga
        /// </summary>
        /// <param name="propertyInfo"></param>
        public IndexedSagaProperty(PropertyInfo propertyInfo)
        {
            _values = new Dictionary<TProperty, HashSet<SagaInstance<TSaga>>>();
            _getProperty = GetGetMethod(propertyInfo);
        }

        public int Count => _values.Count;

        public SagaInstance<TSaga> this[object key]
        {
            get
            {
                var keyValue = (TProperty)key;

                HashSet<SagaInstance<TSaga>> result;
                if (_values.TryGetValue(keyValue, out result))
                    return result.Single();

                return null;
            }
        }

        public void Add(TSaga newItem)
        {
            TProperty key = _getProperty(newItem);

            HashSet<SagaInstance<TSaga>> hashSet;
            if (!_values.TryGetValue(key, out hashSet))
            {
                hashSet = new HashSet<SagaInstance<TSaga>>();
                _values.Add(key, hashSet);
            }

            hashSet.Add(new SagaInstance<TSaga>(newItem));
        }

        public void Remove(SagaInstance<TSaga> instance)
        {
            TProperty key = _getProperty(instance.Instance);

            HashSet<SagaInstance<TSaga>> hashSet;
            if (!_values.TryGetValue(key, out hashSet))
                return;

            hashSet.Remove(instance);
        }

        public IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter)
        {
            return _values.Values.SelectMany(x => x).Where(x => filter(x.Instance));
        }

        public IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter)
        {
            var keyValue = (TProperty)key;

            HashSet<SagaInstance<TSaga>> resultSet;
            if (_values.TryGetValue(keyValue, out resultSet))
                return resultSet.Where(x => filter(x.Instance));

            return Enumerable.Empty<SagaInstance<TSaga>>();
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            return _values.Values.SelectMany(x => x).Select(x => transformer(x.Instance));
        }

        static Func<TSaga, TProperty> GetGetMethod(PropertyInfo property)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSaga), "instance");
            return Expression.Lambda<Func<TSaga, TProperty>>(
                Expression.Call(parameterExpression, property.GetGetMethod()), new[] {parameterExpression}).Compile();
        }
    }
}