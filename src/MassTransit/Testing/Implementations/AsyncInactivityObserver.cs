namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
            _inactivityTask = new Lazy<Task>(() =>
            {
                SetupTimeout(timeout, cancellationToken);

                return _inactivityTaskSource.Task;
            });

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
            if (_sources.All(x => x.IsInactive))
            {
                _inactivityTaskSource.TrySetResult(true);
                _inactivityTokenSource.Cancel();
            }

            return Task.CompletedTask;
        }

        public void ForceInactive()
        {
            _inactivityTaskSource.TrySetResult(true);
            _inactivityTokenSource.Cancel();
        }

        async void SetupTimeout(TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(timeout, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }

            _inactivityTaskSource.TrySetResult(true);
        }
    }
}
