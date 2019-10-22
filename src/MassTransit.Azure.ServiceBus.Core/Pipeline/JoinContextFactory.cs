namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;


    public abstract class JoinContextFactory<TLeft, TRight, TContext> :
        IPipeContextFactory<TContext>
        where TLeft : class, PipeContext
        where TRight : class, PipeContext
        where TContext : class, PipeContext
    {
        readonly IPipe<TLeft> _leftPipe;
        readonly IPipeContextSource<TLeft> _leftSource;
        readonly IPipe<TRight> _rightPipe;
        readonly IPipeContextSource<TRight> _rightSource;

        protected JoinContextFactory(IPipeContextSource<TLeft> leftSource, IPipe<TLeft> leftPipe, IPipeContextSource<TRight> rightSource,
            IPipe<TRight> rightPipe)
        {
            _rightSource = rightSource;
            _leftSource = leftSource;
            _rightPipe = rightPipe;
            _leftPipe = leftPipe;
        }

        IPipeContextAgent<TContext> IPipeContextFactory<TContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<TContext> asyncContext = supervisor.AddAsyncContext<TContext>();

            Task<TContext> context = CreateJoinContext(asyncContext, supervisor.Stopped);
            context.ContinueWith(task =>
            {
                try
                {
                    if (task.IsCanceled)
                        asyncContext.CreateCanceled();
                    else if (task.IsFaulted)
                        asyncContext.CreateFaulted(task.Exception);
                }
                catch
                {
                }
            });

            return asyncContext;
        }

        IActivePipeContextAgent<TContext> IPipeContextFactory<TContext>.CreateActiveContext(ISupervisor supervisor, PipeContextHandle<TContext> context,
            CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedContext(context.Context, cancellationToken));
        }

        async Task<TContext> CreateJoinContext(IAsyncPipeContextAgent<TContext> asyncContext, CancellationToken cancellationToken)
        {
            IAsyncPipeContextAgent<TLeft> leftAgent = new AsyncPipeContextAgent<TLeft>();
            IAsyncPipeContextAgent<TRight> rightAgent = new AsyncPipeContextAgent<TRight>();

            var leftPipe = new AsyncPipeContextPipe<TLeft>(leftAgent, _leftPipe);
            var leftTask = _leftSource.Send(leftPipe, cancellationToken);

            var rightPipe = new AsyncPipeContextPipe<TRight>(rightAgent, _rightPipe);
            var rightTask = _rightSource.Send(rightPipe, cancellationToken);

            async Task Join()
            {
                try
                {
                    var leftAny = await Task.WhenAny(leftAgent.Context, leftTask).ConfigureAwait(false);
                    if (leftAny == leftTask)
                        await leftTask.ConfigureAwait(false);

                    var rightAny = await Task.WhenAny(rightAgent.Context, rightTask).ConfigureAwait(false);
                    if (rightAny == rightTask)
                        await rightTask.ConfigureAwait(false);

                    var leftContext = await leftAgent.Context.ConfigureAwait(false);
                    var rightContext = await rightAgent.Context.ConfigureAwait(false);

                    var clientContext = CreateClientContext(leftContext, rightContext);

                    clientContext.GetOrAddPayload(() => rightContext);
                    clientContext.GetOrAddPayload(() => leftContext);

                    await asyncContext.Created(clientContext).ConfigureAwait(false);

                    await asyncContext.Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
                finally
                {
                    await Task.WhenAll(leftAgent.Stop("Complete", cancellationToken), rightAgent.Stop("Complete", cancellationToken)).ConfigureAwait(false);
                }

                try
                {
                    await Task.WhenAll(leftTask, rightTask).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Warning?.Log(exception, "Join faulted");
                }
            }

            await Task.WhenAny(asyncContext.Context, Join()).ConfigureAwait(false);

            return await asyncContext.Context.ConfigureAwait(false);
        }

        protected abstract TContext CreateClientContext(TLeft leftContext, TRight rightContext);

        protected abstract Task<TContext> CreateSharedContext(Task<TContext> context, CancellationToken cancellationToken);
    }
}
