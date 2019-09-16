namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;


    public class SessionContextFactory :
        IPipeContextFactory<SessionContext>
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public SessionContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
        }

        IPipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<SessionContext> asyncContext = supervisor.AddAsyncContext<SessionContext>();

            CreateSession(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<SessionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedSession(context.Context, cancellationToken));
        }

        async Task<SessionContext> CreateSharedSession(Task<SessionContext> context, CancellationToken cancellationToken)
        {
            var sessionContext = await context.ConfigureAwait(false);

            return new SharedSessionContext(sessionContext, cancellationToken);
        }

        void CreateSession(IAsyncPipeContextAgent<SessionContext> asyncContext, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                try
                {
                    var session = await connectionContext.CreateSession(cancellationToken).ConfigureAwait(false);

                    LogContext.Debug?.Log("Created session: {Host}", connectionContext.Description);

                    var sessionContext = new ActiveMqSessionContext(connectionContext, session, cancellationToken);

                    void HandleException(Exception exception)
                    {
                    #pragma warning disable 4014
                        sessionContext.DisposeAsync(CancellationToken.None);
                    #pragma warning restore 4014
                    }

                    connectionContext.Connection.ExceptionListener += HandleException;

                #pragma warning disable 4014
                    // ReSharper disable once MethodSupportsCancellation
                    asyncContext.Completed.ContinueWith(task =>
                #pragma warning restore 4014
                    {
                        connectionContext.Connection.ExceptionListener -= HandleException;
                    });

                    await asyncContext.Created(sessionContext).ConfigureAwait(false);

                    await asyncContext.Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "Create session failed: {Host}", connectionContext.Description);

                    await asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
            });

            var connectionTask = _connectionContextSupervisor.Send(connectionPipe, cancellationToken);

            Task NotifyCreateCanceled(Task task) => asyncContext.CreateCanceled();

            connectionTask.ContinueWith(NotifyCreateCanceled, TaskContinuationOptions.OnlyOnCanceled);

            Task NotifyCreateFaulted(Task task) => asyncContext.CreateFaulted(task.Exception);

            connectionTask.ContinueWith(NotifyCreateFaulted, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
