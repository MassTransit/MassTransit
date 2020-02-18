namespace MassTransit.Saga.InMemoryRepository
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;


    public class SagaInstance<TSaga> :
        IEquatable<SagaInstance<TSaga>>
        where TSaga : class, ISaga
    {
        readonly SemaphoreSlim _inUse;
        readonly TSaga _instance;
        bool _isRemoved;

        public SagaInstance(TSaga instance)
        {
            _instance = instance;
            _inUse = new SemaphoreSlim(1);
        }

        public TSaga Instance => _instance;

        public bool IsRemoved => _isRemoved;

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
            if (_isRemoved)
                throw new InvalidOperationException($"The saga instance was removed: {TypeMetadataCache<TSaga>.ShortName}: {_instance.CorrelationId}");

            return _inUse.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            if (_isRemoved)
                return;

            _inUse.Release();
        }

        public void Remove()
        {
            _isRemoved = true;
            _inUse.Release();
        }
    }
}
