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
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Saga;


    public class SagaInstance<TSaga> :
        IEquatable<SagaInstance<TSaga>>
    {
        readonly SemaphoreSlim _inUse;
        readonly TSaga _instance;

        public SagaInstance(TSaga instance)
        {
            _instance = instance;
            _inUse = new SemaphoreSlim(1);
        }

        public TSaga Instance => _instance;

        public bool IsRemoved { get; set; }

        public bool Equals(SagaInstance<TSaga> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return EqualityComparer<TSaga>.Default.Equals(_instance, other._instance);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((SagaInstance<TSaga>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TSaga>.Default.GetHashCode(_instance);
        }

        public Task MarkInUse(CancellationToken cancellationToken)
        {
            return _inUse.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            _inUse.Release();
        }

        public void Remove()
        {
            IsRemoved = true;
            _inUse.Release();
        }
    }


    public class IndexedSagaDictionary<TSaga>
        where TSaga : class, ISaga
    {
        readonly SemaphoreSlim _inUse = new SemaphoreSlim(1);
        readonly IndexedSagaProperty<TSaga> _indexById;
        readonly Dictionary<string, IndexedSagaProperty<TSaga>> _indices;
        readonly object _lock = new object();

        public IndexedSagaDictionary()
        {
            _indices = new Dictionary<string, IndexedSagaProperty<TSaga>>();

            BuildIndices();

            _indexById = _indices["CorrelationId"];
        }

        public SagaInstance<TSaga> this[Guid sagaId]
        {
            get
            {
                lock (_lock)
                    return _indexById[sagaId];
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                    return _indexById.Count;
            }
        }

        public Task MarkInUse(CancellationToken cancellationToken)
        {
            return _inUse.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            _inUse.Release();
        }

        public void Add(SagaInstance<TSaga> instance)
        {
            lock (_lock)
                foreach (var index in _indices.Values)
                    index.Add(instance);
        }

        public void Remove(SagaInstance<TSaga> item)
        {
            lock (_lock)
                foreach (var index in _indices.Values)
                    index.Remove(item);
        }

        public IEnumerable<SagaInstance<TSaga>> Where(ISagaQuery<TSaga> query)
        {
            lock (_lock)
            {
                IndexedSagaProperty<TSaga> index = HasIndexFor(query.FilterExpression);
                if (index == null)
                    return _indexById.Where(query.GetFilter()).ToList();

                object rightValue = GetRightValue(query.FilterExpression);

                return index.Where(rightValue, query.GetFilter()).ToList();
            }
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            lock (_lock)
                return _indexById.Select(transformer);
        }

        IndexedSagaProperty<TSaga> HasIndexFor(Expression<Func<TSaga, bool>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var propertyInfo = ((MemberExpression)expression.Body).Member as PropertyInfo;

                if (propertyInfo == null)
                    return null;

                IndexedSagaProperty<TSaga> result;
                if (_indices.TryGetValue(propertyInfo.Name, out result))
                    return result;
            }

            return null;
        }

        void BuildIndices()
        {
            IEnumerable<PropertyInfo> indexProperties = typeof(TSaga).GetProperties()
                .Where(x => x.HasAttribute<IndexedAttribute>() || x.Name.Equals("CorrelationId"));

            foreach (PropertyInfo property in indexProperties)
            {
                _indices.Add(property.Name, (IndexedSagaProperty<TSaga>)Activator.CreateInstance(
                    typeof(IndexedSagaProperty<,>).MakeGenericType(typeof(TSaga), property.PropertyType), property));
            }
        }

        static object GetRightValue(Expression<Func<TSaga, bool>> right)
        {
            switch (right.Body.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)right.Body).Value;

                default:
                    return right.Compile().DynamicInvoke(null);
            }
        }
    }
}