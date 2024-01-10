namespace MassTransit;

using System;
using System.Threading.Tasks;
using Internals;


class OneTimeSetupMethod
{
    readonly OneTimeSetupCallback _callback;
    readonly TaskCompletionSource<bool> _value;

    public OneTimeSetupMethod(OneTimeSetupCallback callback)
    {
        _callback = callback;
        _value = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public Task<bool> Value => _value.Task;

    public Task SetupPayload()
    {
        try
        {
            var task = _callback();
            if (task.Status == TaskStatus.RanToCompletion)
            {
                _value.TrySetResult(true);

                return Task.CompletedTask;
            }

            async Task SetupAsync()
            {
                try
                {
                    await task.ConfigureAwait(false);

                    _value.TrySetResult(true);
                }
                catch (Exception exception)
                {
                    _value.TrySetException(exception);

                    throw;
                }
            }

            return SetupAsync();
        }
        catch (Exception exception)
        {
            _value.TrySetException(exception);

            throw;
        }
    }

    public void SetPayload(Task<bool> value)
    {
        _value.TrySetFromTask(value);
    }
}


public interface OneTimeContext
{
    void Evict();
}
