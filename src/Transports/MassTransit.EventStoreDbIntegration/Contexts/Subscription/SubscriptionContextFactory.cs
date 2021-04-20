using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class SubscriptionContextFactory :
        IPipeContextFactory<SubscriptionContext>
    {
        readonly CheckpointStoreFactory _checkpointStoreFactory;
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IHostConfiguration _hostConfiguration;
        readonly SubscriptionSettings _subscriptionSettings;

        public SubscriptionContextFactory(IConnectionContextSupervisor contextSupervisor, IHostConfiguration hostConfiguration,
            SubscriptionSettings subscriptionSettings, IHeadersDeserializer headersDeserializer,
            CheckpointStoreFactory checkpointStoreFactory)
        {
            _contextSupervisor = contextSupervisor;
            _hostConfiguration = hostConfiguration;
            _subscriptionSettings = subscriptionSettings;
            _headersDeserializer = headersDeserializer;
            _checkpointStoreFactory = checkpointStoreFactory;
        }

        public IActivePipeContextAgent<SubscriptionContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<SubscriptionContext> context, CancellationToken cancellationToken = default)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }


        IPipeContextAgent<SubscriptionContext> IPipeContextFactory<SubscriptionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<SubscriptionContext> asyncContext = supervisor.AddAsyncContext<SubscriptionContext>();

            CreateSubscription(asyncContext, supervisor.Stopped);

            return asyncContext;
        }


        static async Task<SubscriptionContext> CreateSharedConnection(Task<SubscriptionContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedSubscriptionContext(context.Result, cancellationToken)
                : new SharedSubscriptionContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateSubscription(IAsyncPipeContextAgent<SubscriptionContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<SubscriptionContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                SubscriptionContext context;

                if (_subscriptionSettings.IsCatchupSubscription)
                {
                    var client = connectionContext.CreateEventStoreDbClient();
                    var checkpointStore = _checkpointStoreFactory(client);

                    context = _subscriptionSettings.StreamName.IsAllStream
                        ? new EventStoreDbAllStreamCatchupSubscriptionContext(_hostConfiguration, _subscriptionSettings, client, _headersDeserializer,
                            checkpointStore, createCancellationToken)
                        : (SubscriptionContext)new EventStoreDbCatchupSubscriptionContext(_hostConfiguration, _subscriptionSettings, client, _headersDeserializer,
                            checkpointStore, createCancellationToken);
                }
                else {
                    throw new NotSupportedException("Persistent subscriptions are not supported at this time.");
                }

                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
