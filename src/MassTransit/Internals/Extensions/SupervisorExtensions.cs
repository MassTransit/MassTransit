namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;


    public static class SupervisorExtensions
    {
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
            HandleSupervisorTask();
        #pragma warning restore 4014

            return await asyncContext.Context.ConfigureAwait(false);
        }


        class CreateAgentPipe<T, TAgent> :
            IPipe<T>
            where T : class, PipeContext
            where TAgent : class, PipeContext
        {
            readonly IAsyncPipeContextAgent<TAgent> _asyncContext;
            readonly Func<T, CancellationToken, Task<TAgent>> _agentFactory;
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
