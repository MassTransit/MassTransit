namespace MassTransit.Internals.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class PendingValue<TKey, TValue> :
        IPendingValue<TValue>
        where TValue : class
    {
        readonly MissingValueFactory<TKey, TValue> _factory;
        readonly TKey _key;
        readonly TaskCompletionSource<TValue> _value;

        public PendingValue(TKey key, MissingValueFactory<TKey, TValue> factory)
        {
            _key = key;
            _factory = factory ?? DefaultMissingValueFactory;

            _value = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task<TValue> Value => _value.Task;

        public Task<TValue> CreateValue()
        {
            try
            {
                Task<TValue> factory = _factory(_key);
                if (factory.Status == TaskStatus.RanToCompletion)
                {
                    _value.TrySetResult(factory.Result);
                    return factory;
                }

                async Task<TValue> CreateValueAsync()
                {
                    try
                    {
                        var value = await factory.ConfigureAwait(false);

                        _value.TrySetResult(value);

                        return value;
                    }
                    catch (Exception exception)
                    {
                        _value.TrySetException(exception);

                        throw;
                    }
                }

                return CreateValueAsync();
            }
            catch (Exception exception)
            {
                _value.TrySetException(exception);

                throw;
            }
        }

        public void SetValue(Task<TValue> value)
        {
            switch (value.Status)
            {
                case TaskStatus.RanToCompletion:
                    _value.TrySetResult(value.Result);
                    break;
                case TaskStatus.Faulted:
                    _value.TrySetException(value.Exception?.GetBaseException() ??
                        new ValueFactoryException("The missing value factory faulted, but no exception was found"));
                    break;
                case TaskStatus.Canceled:
                    _value.TrySetCanceled();
                    break;
                default:
                    throw new ArgumentException("A completed, faulted, or cancelled task was expected");
            }
        }

        static Task<TValue> DefaultMissingValueFactory(TKey key)
        {
            throw new KeyNotFoundException($"Key not found: {key}");
        }
    }
}
