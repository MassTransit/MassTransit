namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    public class IndexedSagaDictionary<TSaga>
        where TSaga : class, ISaga
    {
        readonly IIndexedSagaProperty<TSaga> _indexById;
        readonly Dictionary<string, IIndexedSagaProperty<TSaga>> _indices;
        readonly SemaphoreSlim _inUse = new SemaphoreSlim(1);
        readonly object _lock = new object();

        public IndexedSagaDictionary()
        {
            _indices = new Dictionary<string, IIndexedSagaProperty<TSaga>>();

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
            {
                foreach (IIndexedSagaProperty<TSaga> index in _indices.Values)
                    index.Add(instance);
            }
        }

        public void Remove(SagaInstance<TSaga> item)
        {
            lock (_lock)
            {
                foreach (IIndexedSagaProperty<TSaga> index in _indices.Values)
                    index.Remove(item);
            }
        }

        public IEnumerable<SagaInstance<TSaga>> Where(ISagaQuery<TSaga> query)
        {
            lock (_lock)
            {
                IIndexedSagaProperty<TSaga> index = HasIndexFor(query.FilterExpression);
                if (index == null)
                    return _indexById.Where(query.GetFilter()).ToList();

                var rightValue = GetRightValue(query.FilterExpression);

                return index.Where(rightValue, query.GetFilter()).ToList();
            }
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            lock (_lock)
                return _indexById.Select(transformer);
        }

        IIndexedSagaProperty<TSaga> HasIndexFor(Expression<Func<TSaga, bool>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var propertyInfo = ((MemberExpression)expression.Body).Member as PropertyInfo;

                if (propertyInfo == null)
                    return null;

                if (_indices.TryGetValue(propertyInfo.Name, out IIndexedSagaProperty<TSaga> result))
                    return result;
            }

            return null;
        }

        void BuildIndices()
        {
            IEnumerable<PropertyInfo> indexProperties = typeof(TSaga).GetProperties()
                .Where(x => x.HasAttribute<IndexedAttribute>() || x.Name.Equals("CorrelationId"));

            foreach (var property in indexProperties)
            {
                var propertyType = typeof(IndexedSagaProperty<,>).MakeGenericType(typeof(TSaga), property.PropertyType);

                _indices.Add(property.Name, (IIndexedSagaProperty<TSaga>)Activator.CreateInstance(propertyType, property));
            }
        }

        static object GetRightValue(Expression<Func<TSaga, bool>> right)
        {
            switch (right.Body.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)right.Body).Value;

                default:
                    return right.CompileFast().DynamicInvoke(null);
            }
        }
    }
}
