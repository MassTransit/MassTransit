namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration.Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IAmazonSqsHostConfiguration _configuration;
        readonly IAmazonSqsHostTopology _hostTopology;

        public ConnectionContextFactory(IAmazonSqsHostConfiguration configuration, IAmazonSqsHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConnectionContext> asyncContext = supervisor.AddAsyncContext<ConnectionContext>();

            var context = Task.Factory.StartNew(() => CreateConnection(asyncContext, supervisor), supervisor.Stopping, TaskCreationOptions.None,
                    TaskScheduler.Default)
                .Unwrap();

            return asyncContext;
        }

        IActivePipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        async Task<ConnectionContext> CreateSharedConnection(Task<ConnectionContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            var sharedConnection = new SharedConnectionContext(connectionContext, cancellationToken);

            return sharedConnection;
        }

        async Task<ConnectionContext> CreateConnection(IAsyncPipeContextAgent<ConnectionContext> asyncContext, IAgent supervisor)
        {
            IConnection connection = null;
            try
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_configuration.HostAddress}");

                connection = _configuration.Settings.CreateConnection();

                var connectionContext = new AmazonSqsConnectionContext(connection, _configuration, _hostTopology, supervisor.Stopped);

                await asyncContext.Created(connectionContext).ConfigureAwait(false);

                return connectionContext;
            }
            catch (OperationCanceledException)
            {
                await asyncContext.CreateCanceled().ConfigureAwait(false);
                throw;
            }
        }
    }
}
