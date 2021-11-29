namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;


    public static class SupervisorExtensions
    {
        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="context">The context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IPipeContextAgent<T> AddContext<T>(this ISupervisor supervisor, T context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IPipeContextAgent<T> contextAgent = new PipeContextAgent<T>(context);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="context">The context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IPipeContextAgent<T> AddContext<T>(this ISupervisor supervisor, Task<T> context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IPipeContextAgent<T> contextAgent = new PipeContextAgent<T>(context);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="contextHandle">The actual context handle</param>
        /// <param name="context">The active context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IActivePipeContextAgent<T> AddActiveContext<T>(this ISupervisor supervisor, PipeContextHandle<T> contextHandle, Task<T> context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var activeContext = new ActivePipeContext<T>(contextHandle, context);

            var contextAgent = new ActivePipeContextAgent<T>(activeContext);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="contextHandle">The actual context handle</param>
        /// <param name="context">The active context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IActivePipeContextAgent<T> AddActiveContext<T>(this ISupervisor supervisor, PipeContextHandle<T> contextHandle, T context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var activeContext = new ActivePipeContext<T>(contextHandle, context);

            var contextAgent = new ActivePipeContextAgent<T>(activeContext);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IAsyncPipeContextAgent<T> AddAsyncContext<T>(this ISupervisor supervisor)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));

            IAsyncPipeContextAgent<T> contextAgent = new AsyncPipeContextAgent<T>();

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        public static async Task<TAgent> CreateAgent<T, TAgent>(this ISupervisor<T> supervisor, IAsyncPipeContextAgent<TAgent> asyncContext,
            Func<T, CancellationToken, Task<TAgent>> agentFactory, CancellationToken cancellationToken)
            where T : class, PipeContext
            where TAgent : class, PipeContext
        {
            var createAgentPipe = new CreateAgentPipe<T, TAgent>(asyncContext, agentFactory, cancellationToken);

            var supervisorTask = supervisor.Send(createAgentPipe, cancellationToken);

            await Task.WhenAny(supervisorTask, asyncContext.Context).ConfigureAwait(false);

            async Task HandleSupervisorTask()
            {
                try
                {
                    await supervisorTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
            }

        #pragma warning disable 4014
            // ReSharper disable once MethodSupportsCancellation
            HandleSupervisorTask().ContinueWith(_ =>
            {
            });
        #pragma warning restore 4014

            return await asyncContext.Context.ConfigureAwait(false);
        }


        class CreateAgentPipe<T, TAgent> :
            IPipe<T>
            where T : class, PipeContext
            where TAgent : class, PipeContext
        {
            readonly Func<T, CancellationToken, Task<TAgent>> _agentFactory;
            readonly IAsyncPipeContextAgent<TAgent> _asyncContext;
            readonly CancellationToken _cancellationToken;

            public CreateAgentPipe(IAsyncPipeContextAgent<TAgent> asyncContext, Func<T, CancellationToken, Task<TAgent>> agentFactory,
                CancellationToken cancellationToken)
            {
                _asyncContext = asyncContext;
                _agentFactory = agentFactory;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(T context)
            {
                try
                {
                    var agent = await _agentFactory(context, _cancellationToken).ConfigureAwait(false);

                    await _asyncContext.Created(agent).ConfigureAwait(false);

                    await _asyncContext.Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await _asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await _asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("createAgent");
            }
        }
    }
}
