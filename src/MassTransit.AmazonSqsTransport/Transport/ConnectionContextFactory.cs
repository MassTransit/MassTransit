namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration.Configuration;
    using Context;
    using Contexts;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Policies;
    using Topology;
    using Transports;


    public class ConnectionContextFactory :
        IPipeContextFactory<ConnectionContext>
    {
        readonly IAmazonSqsHostConfiguration _configuration;
        readonly IAmazonSqsHostTopology _hostTopology;
        readonly IRetryPolicy _connectionRetryPolicy;

        public ConnectionContextFactory(IAmazonSqsHostConfiguration configuration, IAmazonSqsHostTopology hostTopology)
        {
            _configuration = configuration;
            _hostTopology = hostTopology;

            _connectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsConnectException>();

                x.Exponential(1000, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });
        }

        IPipeContextAgent<ConnectionContext> IPipeContextFactory<ConnectionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConnectionContext> asyncContext = supervisor.AddAsyncContext<ConnectionContext>();

            Task.Run(() => CreateConnection(asyncContext, supervisor), supervisor.Stopping);

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
            return await _connectionRetryPolicy.Retry(async () =>
            {
                if (supervisor.Stopping.IsCancellationRequested)
                    throw new OperationCanceledException($"The connection is stopping and cannot be used: {_configuration.HostAddress}");


                IConnection connection = null;
                try
                {
                    TransportLogMessages.ConnectHost(_configuration.Settings.ToString());

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
                catch (Exception ex)
                {
                    LogContext.Error?.Log(ex, "Amazon SQS connection failed");

                    await asyncContext.CreateFaulted(ex).ConfigureAwait(false);

                    throw new AmazonSqsConnectException("Connect failed: " + _configuration.Settings, ex);
                }
            });
        }
    }
}
