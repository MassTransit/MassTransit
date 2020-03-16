namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public ClientContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
        }

        IPipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ClientContext> asyncContext = supervisor.AddAsyncContext<ClientContext>();

            CreateClientContext(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ClientContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedClientContext(context.Context, cancellationToken));
        }

        async Task<ClientContext> CreateSharedClientContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            var modelContext = await context.ConfigureAwait(false);

            return new SharedClientContext(modelContext, cancellationToken);
        }

        void CreateClientContext(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                try
                {
                    var clientContext = connectionContext.CreateClientContext(cancellationToken);

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
            });

            var connectionTask = _connectionContextSupervisor.Send(connectionPipe, cancellationToken);

            Task NotifyCreateCanceled(Task task) => asyncContext.CreateCanceled();

            connectionTask.ContinueWith(NotifyCreateCanceled, TaskContinuationOptions.OnlyOnCanceled);

            Task NotifyCreateFaulted(Task task) => asyncContext.CreateFaulted(task.Exception);

            connectionTask.ContinueWith(NotifyCreateFaulted, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
