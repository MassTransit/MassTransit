namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Util;


    public class AsyncInactivityObserver :
        IInactivityObserver
    {
        readonly Lazy<Task> _inactivityTask;
        readonly TaskCompletionSource<bool> _inactivityTaskSource;
        readonly CancellationTokenSource _inactivityTokenSource;
        readonly HashSet<IInactivityObservationSource> _sources;

        public AsyncInactivityObserver(TimeSpan timeout, CancellationToken cancellationToken)
        {
            _inactivityTaskSource = TaskUtil.GetTask();
            _inactivityTask = new Lazy<Task>(() => TimeoutTask(timeout, cancellationToken));

            _sources = new HashSet<IInactivityObservationSource>();
            _inactivityTokenSource = new CancellationTokenSource();
        }

        public Task InactivityTask => _inactivityTask.Value;

        public CancellationToken InactivityToken => _inactivityTokenSource.Token;

        public void Connected(IInactivityObservationSource source)
        {
            _sources.Add(source);
        }

        public Task NoActivity()
        {
            return CheckSourceActivity();
        }

        public void ForceInactive()
        {
            _inactivityTaskSource.TrySetResult(true);
            _inactivityTokenSource.Cancel();
        }

        Task<bool> CheckSourceActivity()
        {
            if (_sources.All(x => x.IsInactive))
            {
                _inactivityTaskSource.TrySetResult(true);
                _inactivityTokenSource.Cancel();

                return TaskUtil.True;
            }

            return TaskUtil.False;
        }

        async Task TimeoutTask(TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                var inActive = false;
                do
                {
                    await Task.Delay(timeout, cancellationToken).ConfigureAwait(false);

                    inActive = await CheckSourceActivity().ConfigureAwait(false);
                }
                while (!inActive);

                await _inactivityTaskSource.Task.OrCanceled(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }
    }
}
