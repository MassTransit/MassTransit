namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;


    public class PendingReceiveLockContext :
        ReceiveLockContext
    {
        Lock? _lockContext;
        Queue<Lock> _pending;

        public bool IsEmpty => _lockContext == null && (_pending == null || _pending.Count == 0);

        public Task Complete()
        {
            return Execute(context => context.Complete(), true);
        }

        public Task Faulted(Exception exception)
        {
            return Execute(context => context.Faulted(exception), true);
        }

        public Task ValidateLockStatus()
        {
            return Execute(context => context.ValidateLockStatus());
        }

        public bool Enqueue(BaseReceiveContext receiveContext, ReceiveLockContext receiveLockContext)
        {
            var lockContext = new Lock(receiveContext, receiveLockContext);

            lock (this)
            {
                if (_lockContext == null && (_pending == null || _pending.Count == 0))
                {
                    _lockContext = lockContext;
                    return true;
                }

                (_pending ??= new Queue<Lock>(1)).Enqueue(lockContext);
                return false;
            }
        }

        async Task Execute(Func<ReceiveLockContext, Task> action, bool clearLockContext = false)
        {
            if (_lockContext == null)
            {
                lock (this)
                {
                    if (_lockContext == null)
                    {
                        if (_pending == null || _pending.Count == 0)
                            return;

                        _lockContext = _pending.Dequeue();
                    }
                }
            }

            ExceptionDispatchInfo dispatchInfo;

            do
            {
                try
                {
                    var lockContext = _lockContext.Value;

                    if (clearLockContext)
                        _lockContext = null;

                    await action(lockContext.ReceiveLockContext).ConfigureAwait(false);

                    return;
                }
                catch (Exception ex)
                {
                    dispatchInfo = ExceptionDispatchInfo.Capture(ex.GetBaseException());
                }
            }
            while (TryDequeue());

            if (dispatchInfo != null)
            {
                dispatchInfo.Throw();

                throw dispatchInfo.SourceException;
            }
        }

        bool TryDequeue()
        {
            lock (this)
            {
                if (_pending == null || _pending.Count == 0)
                {
                    _lockContext = null;
                    return false;
                }

                _lockContext = _pending.Dequeue();
                return true;
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                _lockContext?.ReceiveContext.Cancel();
                if (_pending != null)
                {
                    foreach (var pendingLock in _pending)
                        pendingLock.ReceiveContext.Cancel();
                }
            }
        }


        readonly struct Lock
        {
            public readonly BaseReceiveContext ReceiveContext;
            public readonly ReceiveLockContext ReceiveLockContext;

            public Lock(BaseReceiveContext receiveContext, ReceiveLockContext receiveLockContext)
            {
                ReceiveContext = receiveContext;
                ReceiveLockContext = receiveLockContext;
            }
        }
    }
}
