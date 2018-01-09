// Copyright 2012-2018 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace GreenPipes.Agents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An Agent Provocateur that simply exists, out of context
    /// </summary>
    public class Agent :
        IAgent
    {
        readonly TaskCompletionSource<DateTime> _completed;
        readonly object _lock = new object();
        readonly TaskCompletionSource<DateTime> _ready;
        readonly Lazy<CancellationTokenSource> _stopped;
        readonly Lazy<CancellationTokenSource> _stopping;

        bool _isStopping;
        bool _isStopped;

        TaskCompletionSource<DateTime> _setCompleted;
        CancellationTokenSource _setCompletedCancel;

        TaskCompletionSource<DateTime> _setReady;
        CancellationTokenSource _setReadyCancel;

        /// <summary>
        /// Creates the Agent
        /// </summary>
        public Agent()
        {
            _ready = new TaskCompletionSource<DateTime>();
            _completed = new TaskCompletionSource<DateTime>();

            _stopped = new Lazy<CancellationTokenSource>(() =>
            {
                var source = new CancellationTokenSource();
                if (_isStopped)
                    source.Cancel();

                return source;
            });
            _stopping = new Lazy<CancellationTokenSource>(() =>
            {
                var source = new CancellationTokenSource();
                if (_isStopping)
                    source.Cancel();

                return source;
            });
        }

        /// <summary>
        /// True if the agent is in the process of stopping or is stopped
        /// </summary>
        protected bool IsStopping => _isStopping;

        /// <summary>
        /// True if the agent is stopped
        /// </summary>
        protected bool IsStopped => _isStopped;

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
            _isStopping = true;
            if (_stopping.IsValueCreated)
                _stopping.Value.Cancel();

            await StopAgent(context).ConfigureAwait(false);

            _isStopped = true;
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
            _completed.TrySetResult(DateTime.UtcNow);

            return TaskUtil.Completed;
        }

        /// <summary>
        /// Puts the agent in a ready state, explicitly
        /// </summary>
        public virtual void SetReady()
        {
            _ready.TrySetResult(DateTime.UtcNow);
        }

        protected bool IsAlreadyReady => _ready.Task.IsCompleted;

        protected bool IsAlreadyCompleted => _completed.Task.IsCompleted;

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
            lock (_lock)
            {
                if (_setReady != null)
                {
                    // if a previous readyTask is already completed, no sense in trying
                    if (_setReady.Task.IsCompleted)
                        return;

                    _setReadyCancel.Cancel();

                    _setReady = null;
                    _setReadyCancel = null;
                }

                if (_ready.Task.IsCompleted)
                    return;

                var setReady = _setReady = new TaskCompletionSource<DateTime>();
                setReady.Task.ContinueWith(SetReadyInternal, TaskScheduler.Default);

                var setReadyCancel = _setReadyCancel = new CancellationTokenSource();

                void OnCompleted(Task task)
                {
                    if (setReadyCancel.IsCancellationRequested)
                        return;

                    if (task.IsCanceled)
                        setReady.TrySetCanceled();
                    else if (task.IsFaulted)

                        // ReSharper disable once AssignNullToNotNullAttribute
                        setReady.TrySetException(task.Exception);
                    else
                        setReady.TrySetResult(DateTime.UtcNow);
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
            lock (_lock)
            {
                if (_setCompleted != null)
                {
                    // if a previous completedTask is already completed, no sense in trying
                    if (_setCompleted.Task.IsCompleted)
                        return;

                    _setCompletedCancel.Cancel();

                    _setCompleted = null;
                    _setCompletedCancel = null;
                }

                if (_completed.Task.IsCompleted)
                    return;

                var setCompleted = _setCompleted = new TaskCompletionSource<DateTime>();
                setCompleted.Task.ContinueWith(SetCompletedInternal, TaskScheduler.Default);

                var setCompletedCancel = _setCompletedCancel = new CancellationTokenSource();

                void OnCompleted(Task task)
                {
                    if (setCompletedCancel.IsCancellationRequested)
                        return;

                    if (task.IsCanceled)
                        setCompleted.TrySetCanceled();
                    else if (task.IsFaulted)

                        // ReSharper disable once AssignNullToNotNullAttribute
                        setCompleted.TrySetException(task.Exception);
                    else
                        setCompleted.TrySetResult(DateTime.UtcNow);
                }

                completedTask.ContinueWith(OnCompleted, TaskScheduler.Default);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Agent";
        }

        void SetReadyInternal(Task<DateTime> task)
        {
            if (task.IsCanceled)
                _ready.TrySetCanceled();
            else if (task.IsFaulted)

                // ReSharper disable once AssignNullToNotNullAttribute
                _ready.TrySetException(task.Exception);
            else
                _ready.TrySetResult(task.Result);
        }

        void SetCompletedInternal(Task<DateTime> task)
        {
            if (task.IsCanceled)
                _completed.TrySetCanceled();
            else if (task.IsFaulted)

                // ReSharper disable once AssignNullToNotNullAttribute
                _completed.TrySetException(task.Exception);
            else
                _completed.TrySetResult(task.Result);
        }

        /// <summary>
        /// Set the agent faulted, making it dead.
        /// </summary>
        /// <param name="task"></param>
        protected void SetFaulted(Task task)
        {
            if (task.IsCanceled)
                _ready.TrySetCanceled();
            else if (task.IsFaulted && task.Exception != null)
                _ready.TrySetException(task.Exception.InnerExceptions);
            else
                _ready.TrySetException(new InvalidOperationException("The context faulted but no exception was present."));

            _completed.TrySetResult(DateTime.UtcNow);
        }
    }
}