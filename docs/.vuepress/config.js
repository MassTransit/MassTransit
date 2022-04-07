module.exports = {
  title: 'MassTransit',
  description: 'A free, open-source distributed application framework for .NET.',
  head: [
      ['link', { rel: "apple-touch-icon", sizes: "180x180", href: "/apple-touch-icon.png"}],
      ['link', { rel: "icon", type: "image/png", sizes: "32x32", href: "/favicon-32x32.png"}],
      ['link', { rel: "icon", type: "image/png", sizes: "16x16", href: "/favicon-16x16.png"}],
      ['link', { rel: "manifest", href: "/site.webmanifest"}],
      ['link', { rel: "mask-icon", href: "/safari-pinned-tab.svg", color: "#3a0839"}],
      ['link', { rel: "shortcut icon", href: "/favicon.ico"}],
      ['meta', { name: "msapplication-TileColor", content: "#3a0839"}],
      ['meta', { name: "msapplication-config", content: "/browserconfig.xml"}],
      ['meta', { name: "theme-color", content: "#ffffff"}],
      ['meta', { name: "viewport", content: "width=device-width, initial-scale=1"}],
    ],
  plugins: [
    '@vuepress/back-to-top',
    [
      '@vuepress/google-analytics', {
        'ga': 'UA-156512132-1'
      }
    ]
  ],
  themeConfig: {
    logo: '/mt-logo-small.png',
    algolia: {
      apiKey: 'e458b7be70837c0e85b6b229c4e26664',
      indexName: 'masstransit'
    },
    nav: [
      { text: "Discord", link: "/discord" },
      { text: 'NuGet', link: 'https://nuget.org/packages/MassTransit' }
    ],
    sidebarDepth: 1,
    sidebar: [
      {
        title: 'Getting Started',
        path: '/getting-started/',
        collapsable: false,
        children: [
          {
            title: 'Quick Starts',
            path: '/quick-starts/',
            collapsable: true,
            children: [
              '/quick-starts/in-memory',
              '/quick-starts/rabbitmq',
              '/quick-starts/azure-service-bus',
              '/quick-starts/sqs'
            ]
          },
          '/getting-started/upgrade-v6',
          {
            title: 'Release Notes',
            path: '/releases/',
            collapsable: true,
            children: [
              '/releases/v8.0.0',
              '/releases/v7.2.3',
              '/releases/v7.2.0',
              '/releases/v7.1.8',
              '/releases/v7.1.7',
              '/releases/v7.1.6',
              '/releases/v7.1.5',
              '/releases/v7.1.4',
              '/releases/v7.1.3',
              '/releases/v7.1.1',
              '/releases/v7.1.0',
              '/releases/v7.0.7',
              '/releases/v7.0.6',
              '/releases/v7.0.4'
            ]
          }
        ]
      },
      {
        title: 'Using MassTransit',
        path: '/usage/',
        collapsable: false,
        children: [
          '/usage/templates',
          '/usage/configuration',
          {
            title: 'Transports',
            path: '/usage/transports/',
            collapsable: true,
            children: [
              '/usage/transports/rabbitmq',
              '/usage/transports/azure-sb',
              '/usage/transports/activemq',
              '/usage/transports/amazonsqs',
              '/usage/transports/grpc',
              '/usage/transports/in-memory'
            ]
          },
          {
            title: 'Riders',
            path: '/usage/riders/',
            collapsable: true,
            children: [
              '/usage/riders/kafka',
              '/usage/riders/eventhub'
            ]
          },
          {
            title: 'FaaS',
            path: '/usage/faas/',
            collapsable: true,
            children: [
              '/usage/faas/azure-functions',
              '/usage/faas/aws-lambda'
            ]
          },
          '/usage/guidance',
          '/usage/mediator',
          '/usage/messages',
          '/usage/consumers',
          '/usage/producers',
          '/usage/exceptions',
          '/usage/requests',
          {
            title: 'Sagas',
            path: '/usage/sagas/',
            collapsable: true,
            children: [
              '/usage/sagas/automatonymous',
              '/usage/sagas/consumer-saga',
              {
                title: 'Persistence',
                path: '/usage/sagas/persistence',
                collapsable: false,
                children: [
                  '/usage/sagas/azure-table',
                  '/usage/sagas/cosmos',
                  '/usage/sagas/dapper',
                  '/usage/sagas/ef',
                  '/usage/sagas/efcore',
                  '/usage/sagas/marten',
                  '/usage/sagas/mongodb',
                  '/usage/sagas/nhibernate',
                  '/usage/sagas/redis',
                  '/usage/sagas/session'
                ]
              }
            ]
          },
          {
            title: 'Containers',
            path: '/usage/containers/',
            collapsable: true,
            children: [
              ['/usage/containers/definitions', 'Definitions'],
              ['/usage/containers/msdi', 'Microsoft'],
              '/usage/containers/multibus'
            ]
          },
          ['/usage/testing', 'Testing'],
          ['/usage/logging', 'Logging'],
          {
            title: 'Advanced',
            collapsable: true,
            sidebarDepth: 2,
            children: [
              {
                title: 'Scheduling',
                path: '/advanced/scheduling/',
                collapsable: true,
                children: [
                  '/advanced/scheduling/scheduling-api',
                  '/advanced/scheduling/activemq-delayed',
                  '/advanced/scheduling/amazonsqs-scheduler',
                  '/advanced/scheduling/azure-sb-scheduler',
                  '/advanced/scheduling/rabbitmq-delayed',
                  '/advanced/scheduling/hangfire'
                ]
              },
              {
                title: 'Courier',
                path: '/advanced/courier/',
                collapsable: true,
                children: [
                  '/advanced/courier/activities',
                  '/advanced/courier/builder',
                  '/advanced/courier/execute',
                  '/advanced/courier/events',
                  '/advanced/courier/subscriptions'
                ]
              },
              {
                title: 'Middleware',
                path: '/advanced/middleware/',
                collapsable: true,
                children: [
                  '/advanced/middleware/receive',
                  '/advanced/middleware/killswitch',
                  '/advanced/middleware/circuit-breaker',
                  '/advanced/middleware/rate-limiter',
                  '/advanced/middleware/transactions',
                  '/advanced/middleware/custom',
                  '/advanced/middleware/scoped'
                ]
              },
              '/usage/message-data',
              {
                title: 'Monitoring',
                collapsable: true,
                children: [
                  '/advanced/monitoring/diagnostic-source',
                  '/advanced/monitoring/prometheus',
                  '/advanced/monitoring/applications-insights',
                  '/advanced/monitoring/perfcounters'
                ]
              },
              '/advanced/connect-endpoint',
              '/advanced/observers',
              {
                title: 'Topology',
                path: '/advanced/topology/',
                collapsable: true,
                children: [
                  '/advanced/topology/message',
                  '/advanced/topology/publish',
                  '/advanced/topology/send',
                  '/advanced/topology/consume',
                  '/advanced/topology/conventions',
                  '/advanced/topology/rabbitmq',
                  '/advanced/topology/servicebus',
                  '/advanced/topology/deploy'
                ]
              },
              {
                title: 'SignalR',
                path: '/advanced/signalr/',
                collapsable: true,
                children: [
                  '/advanced/signalr/quickstart',
                  '/advanced/signalr/hub_endpoints',
                  '/advanced/signalr/interop',
                  '/advanced/signalr/sample',
                  '/advanced/signalr/considerations'
                ]
              },
              'advanced/audit',
              'advanced/batching',
              'advanced/job-consumers'
            ]
          }
        ]
      },
      {
        title: 'Getting Help',
        path: '/learn/',
        collapsable: true,
        children: [
          '/troubleshooting/common-gotchas',
          '/troubleshooting/show-config',
          '/learn/analyzers',
          '/learn/samples',
          '/learn/videos',
          '/learn/training',
          '/learn/support',
          '/learn/loving-the-community',
          '/learn/contributing',
          '/getting-started/live-coding'
        ]
      },
      {
        title: 'Articles',
        collapsable: true,
        children: [
          '/articles/mediator',
          '/articles/outbox',
          '/articles/durable-futures',
          '/articles/net5'
        ]
      },
      {
        title: "Platform",
        path: '/platform/',
        collapsable: true,
        children: [
          '/platform/configuration'
        ]
      },
      {
        title: "Reference",
        children: [
          '/architecture/packages',
          '/architecture/interoperability',
          '/architecture/nservicebus',
          '/architecture/versioning',
          '/architecture/newid',
          '/architecture/encrypted-messages',
          '/architecture/green-cache',
          '/architecture/history'
        ]
      }
    ],
    searchPlaceholder: 'Search...',
    lastUpdated: 'Last Updated',
    repo: 'MassTransit/MassTransit',

    docsRepo: 'MassTransit/MassTransit',
    docsDir: 'docs',
    docsBranch: 'develop',
    editLinks: true,
    editLinkText: 'Help us by improving this page!'
  }
}
