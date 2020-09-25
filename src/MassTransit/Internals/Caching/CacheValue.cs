namespace MassTransit.Internals.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;


    public class CacheValue<TValue> :
        ICacheValue<TValue>
        where TValue : class
    {
        readonly Action _remove;
        readonly TaskCompletionSource<TValue> _value;
        Task<TValue> _createValue;
        Queue<IPendingValue<TValue>> _pending;
        int _usage;

        public CacheValue(Action remove)
        {
            _remove = remove;

            _value = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);

            _usage = -1;
        }

        public Task<TValue> Value
        {
            get
            {
                Interlocked.Increment(ref _usage);

                return _value.Task;
            }
        }

        public bool HasValue => _value.Task.Status == TaskStatus.RanToCompletion;

        public bool IsFaultedOrCanceled => _value.Task.IsFaulted || _value.Task.IsCanceled;

        public int Usage => Interlocked.Exchange(ref _usage, 0);

        public Task<TValue> GetValue(Func<IPendingValue<TValue>> pendingValueFactory)
        {
            Interlocked.Increment(ref _usage);

            lock (this)
            {
                if (HasValue)
                    return _value.Task;

                IPendingValue<TValue> pendingValue = pendingValueFactory();

                if (_createValue == null)
                    _createValue = CreateValue(pendingValue);
                else
                    (_pending ??= new Queue<IPendingValue<TValue>>(1)).Enqueue(pendingValue);

                return pendingValue.Value;
            }
        }

        public Task Evict()
        {
            lock (this)
            {
                if (_usage < int.MinValue / 2)
                    return Task.CompletedTask;

                _usage = int.MinValue;

                _remove();

                if (HasValue)
                {
                    switch (_value.Task.Result)
                    {
                        case IAsyncDisposable asyncDisposable:
                            var valueTask = asyncDisposable.DisposeAsync();
                            if (valueTask.IsCompletedSuccessfully)
                                return Task.CompletedTask;

                            async Task DisposeAsync()
                            {
                                await valueTask.ConfigureAwait(false);
                            }

                            return DisposeAsync();

                        case IDisposable disposable:
                            disposable.Dispose();
                            break;
                    }
                }
            }

            return Task.CompletedTask;
        }

        async Task<TValue> CreateValue(IPendingValue<TValue> pendingValue)
        {
            ExceptionDispatchInfo dispatchInfo;

            do
            {
                try
                {
                    var value = await pendingValue.CreateValue().ConfigureAwait(false);

                    SetResult(value);

                    return value;
                }
                catch (Exception ex)
                {
                    dispatchInfo = ExceptionDispatchInfo.Capture(ex.GetBaseException());
                }
            }
            while (TryTake(out pendingValue));

            SetException(dispatchInfo);

            dispatchInfo.Throw();

            throw dispatchInfo.SourceException;
        }

        bool TryTake(out IPendingValue<TValue> pendingValue)
        {
            lock (this)
            {
                if (_pending != null && _pending.Count > 0)
                {
                    pendingValue = _pending.Dequeue();
                    return true;
                }
            }

            pendingValue = default;
            return false;
        }

        void SetResult(TValue value)
        {
            lock (this)
            {
                _value.TrySetResult(value);

                while (_pending != null && _pending.Count > 0)
                    _pending.Dequeue().SetValue(_value.Task);

                _pending = null;
            }
        }

        void SetException(ExceptionDispatchInfo dispatchInfo)
        {
            lock (this)
            {
                _value.TrySetException(dispatchInfo.SourceException);

                while (_pending != null && _pending.Count > 0)
                    _pending.Dequeue().SetValue(_value.Task);

                _pending = null;
            }
        }
    }
}
