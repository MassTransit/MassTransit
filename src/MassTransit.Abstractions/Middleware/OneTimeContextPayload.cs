namespace MassTransit;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;


class OneTimeContextPayload<TPayload> :
    OneTimeContext<TPayload>
    where TPayload : class
{
    Task<bool>? _createValue;
    Queue<OneTimeSetupMethod>? _pending;
    TaskCompletionSource<bool> _value;

    public OneTimeContextPayload()
    {
        _value = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public Task Value => _value.Task;

    public bool HasValue => _value.Task.Status == TaskStatus.RanToCompletion;

    public bool IsFaultedOrCanceled => _value.Task.IsFaulted || _value.Task.IsCanceled;

    public void Evict()
    {
        lock (this)
        {
            _value = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _createValue = null;
        }
    }

    public Task RunOneTime(Func<OneTimeSetupMethod> oneTimeSetupMethodFactory)
    {
        lock (this)
        {
            if (HasValue)
                return _value.Task;

            var pendingValue = oneTimeSetupMethodFactory();

            if (_createValue == null)
                _createValue = RunOnce(pendingValue);
            else
                (_pending ??= new Queue<OneTimeSetupMethod>(1)).Enqueue(pendingValue);

            return pendingValue.Value;
        }
    }

    async Task<bool> RunOnce(OneTimeSetupMethod oneTimeSetupMethod)
    {
        ExceptionDispatchInfo dispatchInfo;

        var setupPayload = oneTimeSetupMethod;
        do
        {
            try
            {
                await setupPayload.SetupPayload().ConfigureAwait(false);

                SetResult(true);

                return true;
            }
            catch (Exception ex)
            {
                dispatchInfo = ExceptionDispatchInfo.Capture(ex.GetBaseException());
            }
        }
        while (TryTake(out setupPayload));

        SetException(dispatchInfo);

        dispatchInfo.Throw();

        throw dispatchInfo.SourceException;
    }

    bool TryTake([NotNullWhen(true)] out OneTimeSetupMethod? pendingValue)
    {
        lock (this)
        {
            if (_pending is { Count: > 0 })
            {
                pendingValue = _pending.Dequeue();
                return true;
            }
        }

        pendingValue = default;
        return false;
    }

    void SetResult(bool value)
    {
        lock (this)
        {
            _value.TrySetResult(value);

            while (_pending is { Count: > 0 })
                _pending.Dequeue().SetPayload(_value.Task);

            _pending = null;
        }
    }

    void SetException(ExceptionDispatchInfo dispatchInfo)
    {
        lock (this)
        {
            _value.TrySetException(dispatchInfo.SourceException);

            while (_pending is { Count: > 0 })
                _pending.Dequeue().SetPayload(_value.Task);

            _pending = null;
        }
    }
}
