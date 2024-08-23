namespace MassTransit.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// An Agent Provocateur that simply exists, out of context
    /// </summary>
    public class Agent :
        IAgent
    {
        readonly TaskCompletionSource<bool> _completed;
        readonly TaskCompletionSource<bool> _ready;
        readonly Lazy<CancellationTokenSource> _stopped;
        readonly Lazy<CancellationTokenSource> _stopping;

        TaskCompletionSource<bool>? _setCompleted;
        CancellationTokenSource? _setCompletedCancel;

        TaskCompletionSource<bool>? _setReady;
        CancellationTokenSource? _setReadyCancel;

        /// <summary>
        /// Creates the Agent
        /// </summary>
        public Agent()
        {
            _ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _stopped = new Lazy<CancellationTokenSource>(() =>
            {
                var source = new CancellationTokenSource();
                if (IsStopped)
                    source.Cancel();

                return source;
            });
            _stopping = new Lazy<CancellationTokenSource>(() =>
            {
                var source = new CancellationTokenSource();
                if (IsStopping)
                    source.Cancel();

                return source;
            });
        }

        /// <summary>
        /// True if the agent is in the process of stopping or is stopped
        /// </summary>
        protected bool IsStopping { get; private set; }

        /// <summary>
        /// True if the agent is stopped
        /// </summary>
        protected bool IsStopped { get; private set; }

        protected bool IsAlreadyReady => _ready.Task.IsCompleted;

        protected bool IsAlreadyCompleted => _completed.Task.IsCompleted;

        /// <inheritdoc />
        public Task Ready => _ready.Task;

        /// <inheritdoc />
        public Task Completed => _completed.Task;

        /// <inheritdoc />
        public CancellationToken Stopping => _stopping.Value.Token;

        /// <inheritdoc />
        public CancellationToken Stopped => _stopped.Value.Token;

        /// <inheritdoc />
        public async Task Stop(StopContext context)
        {
            if (IsStopping)
                return;

            IsStopping = true;
            if (_stopping.IsValueCreated)
                _stopping.Value.Cancel();

            await StopAgent(context).ConfigureAwait(false);

            IsStopped = true;
            if (_stopped.IsValueCreated)
                _stopped.Value.Cancel();
        }

        /// <summary>
        /// Stops the agent
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task StopAgent(StopContext context)
        {
            _completed.TrySetResult(true);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Puts the agent in a ready state, explicitly
        /// </summary>
        public virtual void SetReady()
        {
            _ready.TrySetResult(true);
        }

        /// <summary>
        /// Puts the agent in a faulted state where it will never be ready
        /// </summary>
        /// <param name="exception"></param>
        public virtual void SetNotReady(Exception exception)
        {
            _ready.TrySetException(exception);
        }

        /// <summary>
        /// Set the agent ready for duty
        /// </summary>
        /// <param name="readyTask"></param>
        protected void SetReady(Task readyTask)
        {
            lock (_ready)
            {
                if (_setReady != null)
                {
                    // if a previous readyTask is already completed, no sense in trying
                    if (_setReady.Task.IsCompleted)
                    {
                        if (_setReady.Task.IsFaulted)
                        {
                            var _ = _setReady.Task.Exception;
                        }

                        return;
                    }

                    _setReadyCancel?.Cancel();

                    _setReady = null;
                    _setReadyCancel = null;
                }

                if (_ready.Task.IsCompleted)
                {
                    if (_ready.Task.IsFaulted)
                    {
                        var _ = _ready.Task.Exception;
                    }

                    return;
                }

                var setReadyCancel = _setReadyCancel = new CancellationTokenSource();

                void OnSetReady(Task<bool> task)
                {
                    if (setReadyCancel.IsCancellationRequested)
                        return;

                    _ready.TrySetFromTask(task);
                }

                TaskCompletionSource<bool> setReady = _setReady = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                setReady.Task.ContinueWith(OnSetReady, TaskScheduler.Default);

                void OnCompleted(Task task)
                {
                    if (setReadyCancel.IsCancellationRequested)
                        return;

                    setReady.TrySetFromTask(task, true);
                }

                readyTask.ContinueWith(OnCompleted, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Set the agent Completed for duty
        /// </summary>
        /// <param name="completedTask"></param>
        protected void SetCompleted(Task completedTask)
        {
            lock (_completed)
            {
                if (_setCompleted != null)
                {
                    // if a previous completedTask is already completed, no sense in trying
                    if (_setCompleted.Task.IsCompleted)
                        return;

                    _setCompletedCancel?.Cancel();

                    _setCompleted = null;
                    _setCompletedCancel = null;
                }

                if (_completed.Task.IsCompleted)
                    return;

                var setCompletedCancel = _setCompletedCancel = new CancellationTokenSource();

                void OnSetCompleted(Task<bool> task)
                {
                    if (setCompletedCancel.IsCancellationRequested)
                        return;

                    _completed.TrySetFromTask(task);
                }

                TaskCompletionSource<bool> setCompleted = _setCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                setCompleted.Task.ContinueWith(OnSetCompleted, TaskScheduler.Default);

                void OnCompleted(Task task)
                {
                    if (setCompletedCancel.IsCancellationRequested)
                        return;

                    setCompleted.TrySetFromTask(task, true);
                }

                completedTask.ContinueWith(OnCompleted, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Set the agent faulted, making it dead.
        /// </summary>
        /// <param name="task"></param>
        protected void SetFaulted(Task task)
        {
            switch (task)
            {
                case { IsCanceled: true }:
                    _ready.TrySetCanceled();
                    break;
                case { IsFaulted: true, Exception.InnerExceptions: not null }:
                    _ready.TrySetException(task.Exception.InnerExceptions);
                    break;
                case { IsFaulted: true, Exception: not null }:
                    _ready.TrySetException(task.Exception);
                    break;
                default:
                    _ready.TrySetException(new InvalidOperationException("The context faulted but no exception was present."));
                    break;
            }

            _completed.TrySetResult(true);
        }
    }
}
