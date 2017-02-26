namespace MassTransit.Util.Caching
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A bucket node has been stored in a bucket, and is a fully resolved value.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public class BucketNode<TValue> :
        IBucketNode<TValue>
        where TValue : class
    {
        Task<TValue> _value;

        Bucket<TValue> _bucket;
        IBucketNode<TValue> _next;

        public BucketNode(TValue value)
        {
            _value = Task.FromResult(value);

            var notify = value as INotifyValueTouched;
            if (notify != null)
                notify.Touched += Touch;
        }

        public Task<TValue> Value
        {
            get
            {
                Touch();
                return _value;
            }
        }

        public bool HasValue => true;
        public Bucket<TValue> Bucket => _bucket;
        public IBucketNode<TValue> Next => _next;

        public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
        {
            return _value;
        }

        void Touch()
        {
            _bucket?.Touch(this);
        }

        public void SetBucket(Bucket<TValue> bucket, IBucketNode<TValue> next)
        {
            _bucket = bucket;
            _next = next;
        }

        public void SetBucket(Bucket<TValue> bucket)
        {
            _bucket = bucket;
        }

        public void Evict()
        {
            _bucket = null;
            _next = null;

            var notify = _value.Result as INotifyValueTouched;
            if (notify != null)
                notify.Touched -= Touch;

            Interlocked.Exchange(ref _value, Cached.Removed);
        }

        public IBucketNode<TValue> Pop()
        {
            var next = _next;

            _next = null;

            return next;
        }


        static class Cached
        {
            internal static readonly Task<TValue> Removed;

            static Cached()
            {
                var source = new TaskCompletionSource<TValue>();
                source.TrySetException(new InvalidOperationException("The cached value has been removed"));

                Removed = source.Task;
            }
        }
    }
}