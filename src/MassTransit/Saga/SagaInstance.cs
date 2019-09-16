namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


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
}
