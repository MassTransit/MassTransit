namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;


    public static class Pipe
    {
        /// <summary>
        /// Create a new pipe using the pipe configurator to add filters, etc.
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="callback">The configuration callback</param>
        /// <returns>An initialized pipe ready for use</returns>
        public static IPipe<T> New<T>(Action<IPipeConfigurator<T>> callback)
            where T : class, PipeContext
        {
            var configurator = new PipeConfigurator<T>();

            callback(configurator);

            configurator.Validate().ThrowIfContainsFailure("The pipe configuration is invalid:");

            return configurator.Build();
        }

        /// <summary>
        /// Create a new pipe using the pipe configurator to add filters, etc.
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="callback">The configuration callback</param>
        /// <param name="validate">True if the pipe should be validated</param>
        /// <returns>An initialized pipe ready for use</returns>
        public static IPipe<T> New<T>(Action<IPipeConfigurator<T>> callback, bool validate)
            where T : class, PipeContext
        {
            var configurator = new PipeConfigurator<T>();

            callback(configurator);

            if (validate)
                configurator.Validate().ThrowIfContainsFailure("The pipe configuration is invalid:");

            return configurator.Build();
        }

        /// <summary>
        /// Constructs a simple pipe that executes the specified action
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="action">The method to execute</param>
        /// <returns>The constructed pipe</returns>
        public static IPipe<T> Execute<T>(Action<T> action)
            where T : class, PipeContext
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new DelegatePipe<T>(action);
        }

        /// <summary>
        /// Constructs a simple pipe that executes the specified action
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="pipe"></param>
        /// <param name="action">The method to execute</param>
        /// <returns>The constructed pipe</returns>
        public static IPipe<T> AddCallback<T>(this IPipe<T> pipe, Action<T> action)
            where T : class, PipeContext
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new PushPipe<T>(pipe, action);
        }

        /// <summary>
        /// Constructs a simple pipe that executes the specified action
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="action">The method to execute</param>
        /// <returns>The constructed pipe</returns>
        public static IPipe<T> ExecuteAsync<T>(Func<T, Task> action)
            where T : class, PipeContext
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new AsyncDelegatePipe<T>(action);
        }

        /// <summary>
        /// Returns an empty pipe of the specified context type
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns></returns>
        public static IPipe<T> Empty<T>()
            where T : class, PipeContext
        {
            return Cache<T>.EmptyPipe;
        }

        /// <summary>
        /// Returns a pipe for the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IPipe<T> ToPipe<T>(this IFilter<T> filter)
            where T : class, PipeContext
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return new LastPipe<T>(filter);
        }


        class DelegatePipe<T> :
            IPipe<T>
            where T : class, PipeContext
        {
            readonly Action<T> _callback;

            public DelegatePipe(Action<T> callback)
            {
                _callback = callback;
            }

            public Task Send(T context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("execute");
            }
        }


        class PushPipe<T> :
            IPipe<T>
            where T : class, PipeContext
        {
            readonly IPipe<T> _nextPipe;
            readonly Action<T> _callback;

            public PushPipe(IPipe<T> nextPipe, Action<T> callback)
            {
                _nextPipe = nextPipe;
                _callback = callback;
            }

            public async Task Send(T context)
            {
                await _nextPipe.Send(context).ConfigureAwait(false);

                _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("push");
            }
        }


        class AsyncDelegatePipe<T> :
            IPipe<T>
            where T : class, PipeContext
        {
            readonly Func<T, Task> _callback;

            public AsyncDelegatePipe(Func<T, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(T context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("executeAsync");
            }
        }


        static class Cache<TContext>
            where TContext : class, PipeContext
        {
            internal static readonly IPipe<TContext> EmptyPipe = new EmptyPipe<TContext>();
        }
    }
}
