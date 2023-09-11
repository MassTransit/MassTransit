import {H3Event} from "h3";

const mapping: { [key: string]: string } = {
    '/advanced/middleware/circuit-breaker.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/index.html': '/documentation/configuration/middleware',
    '/advanced/middleware/scoped.html': '/documentation/configuration/middleware/scoped',
    '/advanced/middleware/killswitch.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/concurrency-limit.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/transactions.html': '/documentation/configuration/middleware/transactions',
    '/advanced/middleware/custom.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/receive.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/rate-limiter.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware/latest.html': '/documentation/configuration/middleware/filters',
    '/advanced/middleware': '/documentation/configuration/middleware',

    '/advanced/topology/message.html': '/documentation/configuration/topology/message',
    '/advanced/topology/conventions.html': '/documentation/configuration/topology/conventions',
    '/advanced/topology/servicebus.html': '/documentation/concepts/messages',
    '/advanced/topology/index.html': '/documentation/configuration/topology',
    '/advanced/topology/publish.html': '/documentation/configuration/topology',
    '/advanced/topology/send.html': '/documentation/configuration/topology',
    '/advanced/topology/rabbitmq.html': '/documentation/concepts/messages',
    '/advanced/topology/deploy.html': '/documentation/configuration/topology/deploy',
    '/advanced/topology/consume.html': '/documentation/configuration/topology',

    '/advanced': '/documentation/concepts',
    '/advanced/index.html': '/documentation/concepts',

    '/advanced/courier': '/documentation/patterns/routing-slip',
    '/advanced/courier/activities.html': '/documentation/patterns/routing-slip#activities',
    '/advanced/courier/': '/documentation/patterns/routing-slip',
    '/advanced/courier/index.html': '/documentation/patterns/routing-slip',
    '/advanced/courier/builder.html': '/documentation/patterns/routing-slip#building',
    '/advanced/courier/subscriptions.html': '/documentation/patterns/routing-slip#subscriptions',
    '/advanced/courier/execute.html': '/documentation/patterns/routing-slip#executing',
    '/advanced/courier/events.html': '/documentation/patterns/routing-slip#routing-slip-events',

    '/advanced/scheduling/azure-sb-scheduler.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/redeliver.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling': '/documentation/configuration/scheduling',
    '/advanced/scheduling/': '/documentation/configuration/scheduling',
    '/advanced/scheduling/index.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/scheduling-api.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/amazonsqs-scheduler.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/hangfire.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/activemq-delayed.html': '/documentation/configuration/scheduling',
    '/advanced/scheduling/rabbitmq-delayed.html': '/documentation/configuration/scheduling',

    '/advanced/signalr/quickstart.html': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr/index.html': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr/considerations.html': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr/sample.html': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr/hub_endpoints.html': '/documentation/configuration/integrations/signalr',
    '/advanced/signalr/interop.html': '/documentation/configuration/integrations/signalr',

    '/advanced/connect-endpoint.html': '/documentation/concepts/messages',
    '/advanced/batching.html': '/documentation/concepts/messages',
    '/advanced/job-consumers.html': '/documentation/patterns/job-consumers',
    '/advanced/monitoring/applications-insights.html': '/documentation/configuration/observability',
    '/advanced/monitoring/prometheus.html': '/documentation/configuration/observability',
    '/advanced/monitoring/diagnostic-source.html': '/documentation/configuration/observability',
    '/advanced/monitoring/perfcounters.html': '/documentation/configuration/observability',
    '/advanced/audit.html': '/documentation/concepts/messages',
    '/advanced/transactional-outbox.html': '/documentation/configuration/middleware/outbox',
    '/advanced/observers.html': '/documentation/configuration/observability',

    '/releases/v7.1.8.html': '/documentation/concepts/messages',
    '/releases/v7.1.4.html': '/documentation/concepts/messages',
    '/releases/index.html': '/documentation/concepts/messages',
    '/releases/v7.1.5.html': '/documentation/concepts/messages',
    '/releases/v7.2.0.html': '/documentation/concepts/messages',
    '/releases/v7.0.6.html': '/documentation/concepts/messages',
    '/releases/v7.0.7.html': '/documentation/concepts/messages',
    '/releases/v7.1.3.html': '/documentation/concepts/messages',
    '/releases/v7.0.4.html': '/documentation/concepts/messages',
    '/releases/v7.1.0.html': '/documentation/concepts/messages',
    '/releases/v7.1.1.html': '/documentation/concepts/messages',
    '/releases/v7.1.6.html': '/documentation/concepts/messages',
    '/releases/v7.2.3.html': '/documentation/concepts/messages',
    '/releases/v8.0.0.html': '/documentation/concepts/messages',
    '/releases/v7.1.7.html': '/documentation/concepts/messages',

    '/learn/videos.html': '/support/support-channels',
    '/learn/index.html': '/support/support-channels',
    '/learn/support.html': '/support',
    '/learn/analyzers.html': '/documentation/configuration/integrations/roslyn-analyzer',
    '/learn/loving-the-community.html': '/support/support-channels',
    '/learn/contributing.html': '/support/support-channels',
    '/learn/samples.html': '/support/samples',
    '/learn/training.html': '/support/training',

    '/index.html': '/',
    '/support.html': '/support',

    '/understand/index.html': '/documentation/concepts/messages',
    '/understand/key-ideas.html': '/documentation/concepts/messages',
    '/understand/under-the-hood.html': '/documentation/concepts/messages',
    '/understand/publishing.html': '/documentation/concepts/messages',
    '/understand/additions-to-transport.html': '/introduction',

    '/platform': '/documentation/concepts/messages',
    '/platform/index.html': '/documentation/concepts/messages',
    '/platform/configuration.html': '/documentation/concepts/messages',

    '/articles/durable-futures.html': '/documentation/patterns/durable-futures',

    '/articles/outbox.html': '/documentation/concepts/messages',
    '/articles/net5.html': '/documentation/concepts/messages',
    '/articles/mediator.html': '/documentation/concepts/messages',

    '/usage': '/documentation/concepts',

    '/usage/guidance.html': '/documentation/concepts/messages',
    '/usage/index.html': '/documentation/concepts/messages',
    '/usage/monitoring.html': '/documentation/concepts/messages',
    '/usage/correlation.html': '/documentation/concepts/messages',

    '/usage/transports': '/documentation/transports',
    '/usage/transports/in-memory.html': '/documentation/transports',
    '/usage/transports/grpc.html': '/documentation/transports',
    '/usage/transports/index.html': '/documentation/transports',
    '/usage/transports/activemq.html': '/documentation/transports',
    '/usage/transports/rabbitmq.html': '/documentation/transports/rabbitmq',
    '/usage/transports/azure-sb.html': '/documentation/transports/azure-service-bus',
    '/usage/transports/amazonsqs.html': '/documentation/transports/amazon-sqs',

    '/usage/message-data.html': '/documentation/patterns/claim-check',

    '/usage/configuration.html': '/documentation/configuration',

    '/usage/requests.html': '/documentation/concepts/messages',

    '/usage/faas': '/documentation/configuration/transports/azure-functions',
    '/usage/faas/aws-lambda.html': '/documentation/configuration/transports/aws-lambda',
    '/usage/faas/index.html': '/documentation/configuration',
    '/usage/faas/azure-functions.html': '/documentation/configuration/transports/azure-functions',
    '/usage/exceptions.html': '/documentation/concepts/exceptions',
    '/usage/templates.html': '/quick-starts/templates',

    '/usage/sagas': '/documentation/patterns/saga',
    '/usage/sagas/azure-table.html': '/documentation/configuration/persistence/azure-table',
    '/usage/sagas/persistence.html': '/documentation/patterns/saga/persistence',
    '/usage/sagas/quickstart.html': '/documentation/configuration/persistence/messages',
    '/usage/sagas/ef.html': '/documentation/configuration/persistence/entity-framework',
    '/usage/sagas/guidance.html': '/documentation/patterns/saga/guidance',
    '/usage/sagas/index.html': '/documentation/patterns/saga',
    '/usage/sagas/redis.html': '/documentation/configuration/persistence/redis',
    '/usage/sagas/cosmos.html': '/documentation/configuration/persistence/azure-cosmos',
    '/usage/sagas/automatonymous.html': '/documentation/patterns/saga/state-machine',
    '/usage/sagas/consumer-saga.html': '/documentation/patterns/saga/consumer-sagas',
    '/usage/sagas/dapper.html': '/documentation/configuration/persistence/dapper',
    '/usage/sagas/marten.html': '/documentation/configuration/persistence/marten',
    '/usage/sagas/mongodb.html': '/documentation/configuration/persistence/mongodb',
    '/usage/sagas/efcore.html': '/documentation/configuration/persistence/entity-framework',
    '/usage/sagas/session.html': '/documentation/configuration/persistence/azure-service-bus',
    '/usage/sagas/nhibernate.html': '/documentation/configuration/persistence/nhibernate',

    '/usage/audit/azuretable.html': '/documentation/concepts/messages',

    '/usage/messages.html': '/documentation/concepts/messages',

    '/usage/riders': '/documentation/concepts/riders',
    '/usage/riders/index.html': '/documentation/concepts/riders',
    '/usage/riders/kafka.html': '/documentation/transports/kafka',
    '/usage/riders/eventhub.html': '/documentation/configuration/transports/azure-event-hub',

    '/usage/containers/multibus.html': '/documentation/configuration/multibus',

    '/usage/containers': '/documentation/configuration',
    '/usage/containers/index.html': '/documentation/configuration',
    '/usage/containers/msdi.html': '/documentation/configuration',
    '/usage/containers/definitions.html': '/documentation/concepts/consumers#definitions',
    '/usage/containers/simpleinjector.html': '/documentation/configuration',
    '/MassTransit/usage/containers/autofac.html': '/documentation/configuration',
    '/usage/containers/autofac.html': '/documentation/configuration',
    '/usage/containers/castlewindsor.html': '/documentation/configuration',
    '/usage/containers/structuremap.html': '/documentation/configuration',

    '/usage/producers.html': '/documentation/concepts/producers',
    '/usage/testing.html': '/documentation/concepts/testing',
    '/usage/lifecycle-observers.html': '/documentation/configuration/observability',
    '/usage/observers.html': '/documentation/configuration/observability',
    '/usage/mediator.html': '/documentation/concepts/mediator',
    '/usage/consumers.html': '/documentation/concepts/consumers',
    '/usage/logging.html': '/documentation/configuration/integrations/logging',

    '/troubleshooting/common-gotchas.html': '/support/common-mistakes',
    '/troubleshooting/show-config.html': '/support/show-configuration',

    '/quick-starts/in-memory.html': '/quick-starts/in-memory',
    '/quick-starts/azure-service-bus.html': '/quick-starts/azure-service-bus',
    '/quick-starts/index.html': '/quick-starts',
    '/quick-starts/sqs.html': '/quick-starts/amazon-sqs',
    '/quick-starts/rabbitmq.html': '/quick-starts/rabbitmq',

    '/architecture/versioning.html': '/documentation/concepts/messages',
    '/architecture/packages.html': '/support/packages',
    '/architecture/green-cache.html': '/documentation/concepts/messages',
    '/architecture/nservicebus.html': '/documentation/configuration/integrations/nsb',
    '/architecture/interoperability.html': '/documentation/configuration/serialization',
    '/advanced/interoperability.html': '/documentation/configuration/serialization',
    '/architecture/history.html': '/documentation/concepts/messages',
    '/architecture/encrypted-messages.html': '/documentation/concepts/messages',
    '/architecture/newid.html': '/documentation/patterns/newid',

    '/getting-started': '/quick-starts',
    '/getting-started/upgrade-v6.html': '/support/upgrade',
    '/getting-started/index.html': '/documentation/concepts/messages',
    '/getting-started/live-coding.html': '/documentation/concepts/messages',
    '/discord.html': '/support/support-channels',
    '/obsolete': '/documentation/configuration/obsolete',

    '/documentation/configuration/integrations/serialization': '/documentation/configuration/serialization',
    '/documentation/patterns/routing-slip': '/documentation/concepts/routing-slips',
    '/documentation/patterns/routing-slip/configuration': '/documentation/concepts/routing-slips'
}

// flip the map, so we ignore good routes
const reverseMapping = Object.assign({}, ...Object.entries(mapping).map(([k, v]) => ({[v]: k})))


export default defineEventHandler((evt: H3Event) => {
    let path = evt.node.req.url || ''

    // good endpoint, bail
    if (reverseMapping[path]) return;

    if (path.startsWith('/MassTransit/')) {
        path = path.replace('/MassTransit', '')
    }

    let dest = mapping[path]

    // try looking for it with html
    if (dest === undefined) {
        dest = mapping[path + '.html']
    }

    // if still undefined, but path ends with .md
    if (dest === undefined && path.endsWith('.md')) {
        // swap .md for .html
        path = path.replace('.md', '.html')
        dest = mapping[path]
    }

    if (dest) {
        sendRedirect(evt, dest, 302)
            .then(() => {
            })
    }
})
