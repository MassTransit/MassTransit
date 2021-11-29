namespace MassTransit.Caching.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Internals;


    /// <summary>
    /// This is used to store a pending value as a node, which will eventually be published to the indices
    /// once it resolves. If the pending value faults, it will be removed from the index, unless a subsequent
    /// factory completes successfully.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public class PendingValue<TKey, TValue> :
        IPendingValue<TValue>
        where TValue : class
    {
        readonly MissingValueFactory<TKey, TValue> _factory;
        readonly TKey _key;
        readonly TaskCompletionSource<TValue> _source;

        public PendingValue(TKey key, MissingValueFactory<TKey, TValue> factory)
        {
            _key = key;
            _factory = factory ?? DefaultMissingValueFactory;

            _source = new TaskCompletionSource<TValue>();
        }

        public Task<TValue> Value => _source.Task;

        public async Task<TValue> CreateValue()
        {
            try
            {
                var value = await _factory(_key).ConfigureAwait(false);

                _source.TrySetResult(value);

                return value;
            }
            catch (Exception exception)
            {
                _source.TrySetException(exception);

                throw;
            }
        }

        public void SetValue(Task<TValue> value)
        {
            if (value.IsCanceled)
                _source.TrySetCanceled();
            else if (value.IsFaulted)
                _source.TrySetException(value.Exception?.GetBaseException() ?? new ValueFactoryException("The value factory faulted, without exception"));
            else if (value.IsCompletedSuccessfully())
                _source.TrySetResult(value.Result);
            else
                throw new ArgumentException("A completed, faulted, or cancelled task was expected");
        }

        static Task<TValue> DefaultMissingValueFactory(TKey key)
        {
            throw new KeyNotFoundException($"Key not found: {key}");
        }
    }
}
