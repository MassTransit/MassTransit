using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProcessorContextFactory :
        IPipeContextFactory<ProcessorContext>
    {
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ProcessorContextFactory(IConnectionContextSupervisor contextSupervisor, IHostConfiguration hostConfiguration,
            ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer)
        {
            _contextSupervisor = contextSupervisor;
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
        }

        public IActivePipeContextAgent<ProcessorContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProcessorContext> context, CancellationToken cancellationToken = default)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }


        IPipeContextAgent<ProcessorContext> IPipeContextFactory<ProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProcessorContext> asyncContext = supervisor.AddAsyncContext<ProcessorContext>();

            CreateProcessor(asyncContext, supervisor.Stopped);

            return asyncContext;
        }


        static async Task<ProcessorContext> CreateSharedConnection(Task<ProcessorContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProcessorContext(context.Result, cancellationToken)
                : new SharedProcessorContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProcessor(IAsyncPipeContextAgent<ProcessorContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProcessorContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                //TODO: Handler checkpoint store instead of null
                var client = connectionContext.CreateEventStoreDbClient();
                ProcessorContext context = new EventStoreDbProcessorContext(_hostConfiguration, _receiveSettings, client, null,
                    _headersDeserializer, createCancellationToken);
                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
