#nullable enable
namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    public static class TaskUtil
    {
        internal static Task Canceled => Cached<bool>.CanceledTask;
        public static Task Completed => Cached.CompletedTask;
        public static Task<bool> False => Cached.FalseTask;
        public static Task<bool> True => Cached.TrueTask;

        /// <summary>
        /// Returns a completed task with the default value for <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T?> Default<T>()
        {
            return Cached<T>.DefaultValueTask;
        }

        /// <summary>
        /// Returns a faulted task with the specified exception (creating using a <see cref="TaskCompletionSource{T}" />)
        /// </summary>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Faulted<T>(Exception exception)
        {
            TaskCompletionSource<T> source = GetTask<T>();
            source.TrySetException(exception);

            return source.Task;
        }

        /// <summary>
        /// Returns a cancelled task for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Cancelled<T>()
        {
            return Cached<T>.CanceledTask;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCompletionSource{T}" />, and ensures the TaskCreationOptions.RunContinuationsAsynchronously
        /// flag is specified (if available).
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<T> GetTask<T>(TaskCreationOptions options = TaskCreationOptions.None)
        {
            return new TaskCompletionSource<T>(options | TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Creates a new TaskCompletionSource and ensures the TaskCreationOptions.RunContinuationsAsynchronously
        /// flag is specified (if available).
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TaskCompletionSource<bool> GetTask(TaskCreationOptions options = TaskCreationOptions.None)
        {
            return new TaskCompletionSource<bool>(options | TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Register a callback on the <paramref name="cancellationToken" /> which completes the resulting task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="cancelTask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static CancellationTokenRegistration RegisterTask(this CancellationToken cancellationToken, out Task cancelTask)
        {
            if (!cancellationToken.CanBeCanceled)
                throw new ArgumentException("The cancellationToken must support cancellation", nameof(cancellationToken));

            var source = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            cancelTask = source.Task;

            return cancellationToken.Register(SetCompleted, source);
        }

        static void SetCompleted(object? obj)
        {
            if (obj is TaskCompletionSource<bool> source)
                source.SetCompleted();
        }

        public static CancellationTokenRegistration RegisterIfCanBeCanceled(this CancellationToken cancellationToken, CancellationTokenSource source)
        {
            if (cancellationToken.CanBeCanceled)
                return cancellationToken.Register(Cancel, source);

            return default;
        }

        static void Cancel(object? obj)
        {
            if (obj is CancellationTokenSource source)
                source.Cancel();
        }

        /// <summary>
        /// Sets the source to completed using TrySetResult
        /// </summary>
        /// <param name="source"></param>
        public static void SetCompleted(this TaskCompletionSource<bool> source)
        {
            source.TrySetResult(true);
        }

        public static void Await(Func<Task> taskFactory, CancellationToken cancellationToken = default)
        {
            if (taskFactory == null)
                throw new ArgumentNullException(nameof(taskFactory));

            using (InitializeExecutionEnvironment())
            {
                var task = taskFactory();
                if (task == null)
                    throw new InvalidOperationException("The taskFactory must return a Task");

                if (cancellationToken.CanBeCanceled)
                    task = task.OrCanceled(cancellationToken);

                var awaiter = new TaskAwaitAdapter(task);
                if (!awaiter.IsCompleted)
                {
                    var dispatch = SynchronizationDispatcher.FromCurrentSynchronizationContext();
                    dispatch.WaitForCompletion(awaiter);
                }

                awaiter.GetResult();
            }
        }

        public static void Await(Task task, CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            using (InitializeExecutionEnvironment())
            {
                if (cancellationToken.CanBeCanceled)
                    task = task.OrCanceled(cancellationToken);

                var awaiter = new TaskAwaitAdapter(task);

                if (!awaiter.IsCompleted)
                {
                    var waitStrategy = SynchronizationDispatcher.FromCurrentSynchronizationContext();
                    waitStrategy.WaitForCompletion(awaiter);
                }

                awaiter.GetResult();
            }
        }

        public static T Await<T>(Func<Task<T>> taskFactory, CancellationToken cancellationToken = default)
        {
            if (taskFactory == null)
                throw new ArgumentNullException(nameof(taskFactory));

            using (InitializeExecutionEnvironment())
            {
                Task<T>? task = taskFactory();
                if (task == null)
                    throw new InvalidOperationException("The taskFactory must return a Task");

                if (cancellationToken.CanBeCanceled)
                    task = task.OrCanceled(cancellationToken);

                var awaiter = new TaskAwaitAdapter<T>(task);
                if (!awaiter.IsCompleted)
                {
                    var dispatch = SynchronizationDispatcher.FromCurrentSynchronizationContext();
                    dispatch.WaitForCompletion(awaiter);
                }

                return awaiter.GetResultOfT();
            }
        }

        static void ContinueOnSameSynchronizationContext(AwaitAdapter awaiter, Action continuation)
        {
            if (continuation is null)
                throw new ArgumentNullException(nameof(continuation));

            var context = SynchronizationContext.Current;

            awaiter.OnCompleted(() =>
            {
                if (context is null || SynchronizationContext.Current == context)
                    continuation.Invoke();
                else
                    context.Post(_ => continuation.Invoke(), continuation);
            });
        }

        static IDisposable? InitializeExecutionEnvironment()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                var context = SynchronizationContext.Current;
                if (context is null || context.GetType() == typeof(SynchronizationContext))
                {
                    var singleThreadedContext = new SingleThreadedSynchronizationContext(TimeSpan.FromSeconds(10));

                    SetSynchronizationContext(singleThreadedContext);

                    return new DisposableAction(() =>
                    {
                        SetSynchronizationContext(context);
                        singleThreadedContext.Dispose();
                    });
                }
            }

            return null;
        }

        static void SetSynchronizationContext(SynchronizationContext? syncContext)
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }


        static class Cached
        {
            public static readonly Task CompletedTask = Task.FromResult(true);
            public static readonly Task<bool> TrueTask = Task.FromResult(true);
            public static readonly Task<bool> FalseTask = Task.FromResult(false);
        }


        static class Cached<T>
        {
            public static readonly Task<T?> DefaultValueTask = Task.FromResult<T?>(default);
            public static readonly Task<T> CanceledTask = GetCanceledTask();

            static Task<T> GetCanceledTask()
            {
                TaskCompletionSource<T> source = GetTask<T>();
                source.SetCanceled();
                return source.Task;
            }
        }


        sealed class SingleThreadedSynchronizationContext :
            SynchronizationContext,
            IDisposable
        {
            const string ShutdownTimeoutMessage = "Work posted to the synchronization context did not complete within ten seconds.";

            readonly Queue<ScheduledWork> _queue = new();

            readonly TimeSpan _shutdownTimeout;
            Status _status;
            Stopwatch? _timeSinceShutdown;

            public SingleThreadedSynchronizationContext(TimeSpan shutdownTimeout)
            {
                _shutdownTimeout = shutdownTimeout;
            }

            public void Dispose()
            {
                ShutDown();
            }

            public override void Post(SendOrPostCallback d, object? state)
            {
                if (d == null)
                    throw new ArgumentNullException(nameof(d));

                AddWork(new ScheduledWork(d, state, null));
            }

            public override void Send(SendOrPostCallback d, object? state)
            {
                if (d == null)
                    throw new ArgumentNullException(nameof(d));

                if (Current == this)
                    d.Invoke(state);
                else
                {
                    using var finished = new ManualResetEventSlim();

                    AddWork(new ScheduledWork(d, state, finished));
                    finished.Wait();
                }
            }

            void AddWork(ScheduledWork work)
            {
                lock (_queue)
                {
                    switch (_status)
                    {
                        case Status.ShuttingDown:
                            if (_timeSinceShutdown!.Elapsed < _shutdownTimeout)
                                break;
                            goto case Status.ShutDown;

                        case Status.ShutDown:
                            throw ErrorAndGetExceptionForShutdownTimeout();
                    }

                    _queue.Enqueue(work);
                    Monitor.Pulse(_queue);
                }
            }

            public void ShutDown()
            {
                lock (_queue)
                {
                    switch (_status)
                    {
                        case Status.ShuttingDown:
                        case Status.ShutDown:
                            return;
                    }

                    _timeSinceShutdown = Stopwatch.StartNew();
                    _status = Status.ShuttingDown;
                    Monitor.Pulse(_queue);
                }
            }

            public void Run()
            {
                lock (_queue)
                {
                    switch (_status)
                    {
                        case Status.Running:
                            throw new InvalidOperationException("SingleThreadedSynchronizationContext.Run may not be reentered.");

                        case Status.ShuttingDown:
                        case Status.ShutDown:
                            throw new InvalidOperationException("This SingleThreadedSynchronizationContext has been shut down.");
                    }

                    _status = Status.Running;
                }

                while (TryTake(out var scheduledWork))
                    scheduledWork.Execute();
            }

            bool TryTake(out ScheduledWork scheduledWork)
            {
                lock (_queue)
                {
                    while (_queue.Count == 0)
                    {
                        if (_status == Status.ShuttingDown)
                        {
                            _status = Status.ShutDown;
                            scheduledWork = default;
                            return false;
                        }

                        Monitor.Wait(_queue);
                    }

                    if (_status == Status.ShuttingDown && _timeSinceShutdown!.Elapsed > _shutdownTimeout)
                    {
                        _status = Status.ShutDown;
                        throw ErrorAndGetExceptionForShutdownTimeout();
                    }

                    scheduledWork = _queue.Dequeue();
                }

                return true;
            }

            static Exception ErrorAndGetExceptionForShutdownTimeout()
            {
                return new InvalidOperationException(ShutdownTimeoutMessage);
            }


            struct ScheduledWork
            {
                readonly SendOrPostCallback _callback;
                readonly object? _state;
                readonly ManualResetEventSlim? _finished;

                public ScheduledWork(SendOrPostCallback callback, object? state, ManualResetEventSlim? finished)
                {
                    _callback = callback;
                    _state = state;
                    _finished = finished;
                }

                public void Execute()
                {
                    _callback.Invoke(_state);
                    _finished?.Set();
                }
            }


            enum Status
            {
                NotStarted,
                Running,
                ShuttingDown,
                ShutDown
            }
        }


        abstract class AwaitAdapter
        {
            public abstract bool IsCompleted { get; }
            public abstract void OnCompleted(Action action);
            public abstract void GetResult();
        }


        sealed class TaskAwaitAdapter :
            AwaitAdapter
        {
            readonly TaskAwaiter _awaiter;

            public TaskAwaitAdapter(Task task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action)
            {
                _awaiter.UnsafeOnCompleted(action);
            }

            public override void GetResult()
            {
                _awaiter.GetResult();
            }
        }


        sealed class TaskAwaitAdapter<T> :
            AwaitAdapter
        {
            readonly TaskAwaiter<T> _awaiter;

            public TaskAwaitAdapter(Task<T> task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action)
            {
                _awaiter.UnsafeOnCompleted(action);
            }

            public override void GetResult()
            {
                _awaiter.GetResult();
            }

            public T GetResultOfT()
            {
                return _awaiter.GetResult();
            }
        }


        abstract class SynchronizationDispatcher
        {
            public abstract void WaitForCompletion(AwaitAdapter awaiter);

            public static SynchronizationDispatcher FromCurrentSynchronizationContext()
            {
                var context = SynchronizationContext.Current;

                if (context is SingleThreadedSynchronizationContext)
                    return SingleThreadedSynchronizationDispatcher.Instance;

                return WindowsFormsSynchronizationDispatcher.GetIfApplicable()
                    ?? WpfSynchronizationDispatcher.GetIfApplicable()
                    ?? NoSynchronizationDispatcher.Instance;
            }


            sealed class NoSynchronizationDispatcher :
                SynchronizationDispatcher
            {
                public static readonly NoSynchronizationDispatcher Instance = new();

                NoSynchronizationDispatcher()
                {
                }

                public override void WaitForCompletion(AwaitAdapter awaiter)
                {
                    awaiter.GetResult();
                }
            }


            sealed class WindowsFormsSynchronizationDispatcher :
                SynchronizationDispatcher
            {
                static WindowsFormsSynchronizationDispatcher? _instance;
                readonly Action _applicationExit;

                readonly Action _applicationRun;

                WindowsFormsSynchronizationDispatcher(Action applicationRun, Action applicationExit)
                {
                    _applicationRun = applicationRun;
                    _applicationExit = applicationExit;
                }

                public static SynchronizationDispatcher? GetIfApplicable()
                {
                    if (!IsApplicable(SynchronizationContext.Current))
                        return null;

                    if (_instance is null)
                    {
                        var applicationType =
                            SynchronizationContext.Current.GetType().Assembly.GetType("System.Windows.Forms.Application", true)!;

                        var applicationRun = (Action)applicationType
                            .GetMethod("Run", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)!
                            .CreateDelegate(typeof(Action));

                        var applicationExit = (Action)applicationType
                            .GetMethod("Exit", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)!
                            .CreateDelegate(typeof(Action));

                        _instance = new WindowsFormsSynchronizationDispatcher(applicationRun, applicationExit);
                    }

                    return _instance;
                }

                static bool IsApplicable([NotNullWhen(true)] SynchronizationContext? context)
                {
                    return context?.GetType().FullName == "System.Windows.Forms.WindowsFormsSynchronizationContext";
                }

                public override void WaitForCompletion(AwaitAdapter awaiter)
                {
                    var context = SynchronizationContext.Current;

                    if (!IsApplicable(context))
                        throw new InvalidOperationException("This dispatch must only be used from a WindowsFormsSynchronizationContext.");

                    if (awaiter.IsCompleted)
                        return;

                    context.Post(_ => ContinueOnSameSynchronizationContext(awaiter, _applicationExit), awaiter);

                    try
                    {
                        _applicationRun.Invoke();
                    }
                    finally
                    {
                        SynchronizationContext.SetSynchronizationContext(context);
                    }
                }
            }


            sealed class WpfSynchronizationDispatcher :
                SynchronizationDispatcher
            {
                static WpfSynchronizationDispatcher? _instance;
                readonly MethodInfo _dispatcherFrameSetContinueProperty;
                readonly Type _dispatcherFrameType;

                readonly MethodInfo _dispatcherPushFrame;

                WpfSynchronizationDispatcher(MethodInfo dispatcherPushFrame,
                    MethodInfo dispatcherFrameSetContinueProperty,
                    Type dispatcherFrameType)
                {
                    _dispatcherPushFrame = dispatcherPushFrame;
                    _dispatcherFrameSetContinueProperty = dispatcherFrameSetContinueProperty;
                    _dispatcherFrameType = dispatcherFrameType;
                }

                public static SynchronizationDispatcher? GetIfApplicable()
                {
                    var context = SynchronizationContext.Current;

                    if (!IsApplicable(context))
                        return null;

                    if (_instance is null)
                    {
                        var assemblyType = context.GetType().Assembly;
                        var dispatcherType = assemblyType.GetType("System.Windows.Threading.Dispatcher", true)!;
                        var dispatcherFrameType = assemblyType.GetType("System.Windows.Threading.DispatcherFrame", true)!;

                        var dispatcherPushFrame = dispatcherType
                            .GetMethod("PushFrame", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { dispatcherFrameType },
                                null)!;

                        var dispatcherSetFrameContinue = dispatcherFrameType
                            .GetProperty("Continue")?
                            .GetSetMethod()!;

                        _instance = new WpfSynchronizationDispatcher(
                            dispatcherPushFrame,
                            dispatcherSetFrameContinue,
                            dispatcherFrameType);
                    }

                    return _instance;
                }

                static bool IsApplicable([NotNullWhen(true)] SynchronizationContext? context)
                {
                    return context?.GetType().FullName == "System.Windows.Threading.DispatcherSynchronizationContext";
                }

                public override void WaitForCompletion(AwaitAdapter awaiter)
                {
                    var context = SynchronizationContext.Current;

                    if (!IsApplicable(context))
                        throw new InvalidOperationException("This dispatch must only be used from a DispatcherSynchronizationContext.");

                    if (awaiter.IsCompleted)
                        return;

                    var frame = Activator.CreateInstance(_dispatcherFrameType, true);

                    context.Post(_ => ContinueOnSameSynchronizationContext(awaiter, () => _dispatcherFrameSetContinueProperty.Invoke(frame, [false])), awaiter);

                    _dispatcherPushFrame.Invoke(null, [frame]);
                }
            }
        }


        sealed class SingleThreadedSynchronizationDispatcher :
            SynchronizationDispatcher
        {
            public static readonly SingleThreadedSynchronizationDispatcher Instance = new();

            SingleThreadedSynchronizationDispatcher()
            {
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current as SingleThreadedSynchronizationContext
                    ?? throw new InvalidOperationException("This dispatch must only be used from a SingleThreadedSynchronizationContext.");

                if (awaiter.IsCompleted)
                    return;

                context.Post(_ => ContinueOnSameSynchronizationContext(awaiter, context.ShutDown), awaiter);

                context.Run();
            }
        }


        sealed class DisposableAction :
            IDisposable
        {
            Action? _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref _action, null)?.Invoke();
            }
        }
    }
}
