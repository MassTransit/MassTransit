namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class SagaInstance<TSaga> :
        IEquatable<SagaInstance<TSaga>>
        where TSaga : class, ISaga
    {
        readonly SemaphoreSlim _inUse;

        public SagaInstance(TSaga instance)
        {
            Instance = instance;
            _inUse = new SemaphoreSlim(1);
        }

        public TSaga Instance { get; }

        public bool IsRemoved { get; set; }

        public bool Equals(SagaInstance<TSaga> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return EqualityComparer<TSaga>.Default.Equals(Instance, other.Instance);
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
            return EqualityComparer<TSaga>.Default.GetHashCode(Instance);
        }

        public Task MarkInUse(CancellationToken cancellationToken)
        {
            if (IsRemoved)
                throw new InvalidOperationException($"The saga instance was removed: {TypeCache<TSaga>.ShortName}: {Instance.CorrelationId}");

            return _inUse.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            if (IsRemoved)
                return;

            _inUse.Release();
        }

        public void Remove()
        {
            IsRemoved = true;
            _inUse.Release();
        }
    }
}
