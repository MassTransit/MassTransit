namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;


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

        static async Task<SessionContext> CreateSharedSession(Task<SessionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedSessionContext(context.Result, cancellationToken)
                : new SharedSessionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateSession(IAsyncPipeContextAgent<SessionContext> asyncContext, CancellationToken cancellationToken)
        {
            async Task<SessionContext> CreateSessionContext(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                var session = await connectionContext.CreateSession(createCancellationToken).ConfigureAwait(false);

                void HandleConnectionException(Exception exception)
                {
                    // ReSharper disable once MethodSupportsCancellation
                    asyncContext.Stop($"Connection Exception: {exception}");
                }

                connectionContext.Connection.ExceptionListener += HandleConnectionException;

            #pragma warning disable 4014
                // ReSharper disable once MethodSupportsCancellation
                asyncContext.Completed.ContinueWith(_ => connectionContext.Connection.ExceptionListener -= HandleConnectionException);
            #pragma warning restore 4014

                return new ActiveMqSessionContext(connectionContext, session, createCancellationToken);
            }

            _connectionContextSupervisor.CreateAgent(asyncContext, CreateSessionContext, cancellationToken);
        }
    }
}
