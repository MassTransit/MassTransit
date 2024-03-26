# MassTransit

MassTransit provides a developer-focused, modern platform for creating distributed applications without complexity.

- First class testing support
- Write once, then deploy using RabbitMQ, Azure Service Bus, and Amazon SQS
- Observability via Open Telemetry (OTEL)
- Fully-supported, widely-adopted, a complete end-to-end solution

## Documentation

Get started by [reading through the documentation](https://masstransit-project.com/).

## Build Status

| Branch  |                                                                                              Status                                                                                              |
|---------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| master  |  [![master](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=master&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml)  |
| develop | [![develop](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=develop&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml) |

## MassTransit NuGet Packages

The following NuGet packages are the currently supported.

[![alt MassTransit](https://img.shields.io/nuget/v/MassTransit.svg "MassTransit")](https://nuget.org/packages/MassTransit/)

* [MassTransit](https://nuget.org/packages/MassTransit/)
* [MassTransit.Abstractions](https://www.nuget.org/packages/MassTransit.Abstractions/)

### Transports

* [MassTransit.ActiveMQ](https://nuget.org/packages/MassTransit.ActiveMQ/)
* [MassTransit.AmazonSQS](https://nuget.org/packages/MassTransit.AmazonSQS/)
* [MassTransit.Azure.ServiceBus.Core](https://nuget.org/packages/MassTransit.Azure.ServiceBus.Core/)
    * [MassTransit.WebJobs.ServiceBus](https://nuget.org/packages/MassTransit.WebJobs.ServiceBus/)
    * [MassTransit.WebJobs.EventHubs](https://nuget.org/packages/MassTransit.WebJobs.EventHubs/)
* [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)
* **Riders**
    * [MassTransit.EventHub](https://nuget.org/packages/MassTransit.EventHub/)
    * [MassTransit.Kafka](https://nuget.org/packages/MassTransit.Kafka/)

### Saga Persistence

* [MassTransit.AmazonS3](https://nuget.org/packages/MassTransit.AmazonS3/)
* [MassTransit.Azure.Cosmos](https://nuget.org/packages/MassTransit.Azure.Cosmos/)
* [MassTransit.Azure.Cosmos.Table](https://nuget.org/packages/MassTransit.Azure.Cosmos.Table/)
* [MassTransit.DapperIntegration](https://nuget.org/packages/MassTransit.DapperIntegration/)
* [MassTransit.DynamoDb](https://nuget.org/packages/MassTransit.DynamoDb/)
* [MassTransit.EntityFramework](https://nuget.org/packages/MassTransit.EntityFramework/)
* [MassTransit.EntityFrameworkCore](https://nuget.org/packages/MassTransit.EntityFrameworkCore/)
* [MassTransit.Marten](https://nuget.org/packages/MassTransit.Marten/)
* [MassTransit.MongoDb](https://nuget.org/packages/MassTransit.MongoDb/)
* [MassTransit.NHibernate](https://nuget.org/packages/MassTransit.NHibernate/)
* [MassTransit.Redis](https://nuget.org/packages/MassTransit.Redis/)

### Message Data

* [MassTransit.Azure.Storage](https://nuget.org/packages/MassTransit.Azure.Storage/)

### Scheduling

* [MassTransit.Hangfire](https://nuget.org/packages/MassTransit.Hangfire/)
* [MassTransit.Quartz](https://nuget.org/packages/MassTransit.Quartz/)

### Interoperability

* [MassTransit.Interop.NServiceBus](https://nuget.org/packages/MassTransit.Interop.NServiceBus/)
* [MassTransit.Newtonsoft](https://nuget.org/packages/MassTransit.Newtonsoft/)

### Other

* [MassTransit.Analyzers](https://nuget.org/packages/MassTransit.Analyzers/)
* [MassTransit.SignalR](https://nuget.org/packages/MassTransit.SignalR/)
* [MassTransit.Prometheus](https://nuget.org/packages/MassTransit.Prometheus/)
* [MassTransit.StateMachineVisualizer](https://nuget.org/packages/MassTransit.StateMachineVisualizer/)
* [MassTransit.TestFramework](https://nuget.org/packages/MassTransit.TestFramework/)

## Deprecated Packages

The following packages from earlier versions of MassTransit are no longer supported.

* Automatonymous
* Automatonymous.NHibernate
* Automatonymous.Visualizer
* GreenPipes
* MassTransit.ApplicationInsights
* MassTransit.AspNetCore
* MassTransit.Autofac
* MassTransit.Automatonymous
* MassTransit.Automatonymous.Autofac
* MassTransit.Automatonymous.Extensions.DependencyInjection
* MassTransit.Automatonymous.Lamar
* MassTransit.Automatonymous.SimpleInjector
* MassTransit.Automatonymous.StructureMap
* MassTransit.Automatonymous.Windsor
* MassTransit.AzureServiceBus
* MassTransit.CastleWindsor
* MassTransit.Extensions.DependencyInjection
* MassTransit.Extensions.Logging
* MassTransit.Host
* MassTransit.Http
* MassTransit.Lamar
* MassTransit.Log4Net
* MassTransit.MSMQ
* MassTransit.Ninject
* MassTransit.NLog
* MassTransit.Platform.Abstractions
* MassTransit.Reactive
* MassTransit.SerilogIntegration
* MassTransit.SimpleInjector
* MassTransit.StructureMap
* MassTransit.StructureMapSigned
* MassTransit.Unity

## Discord

Get help live at the MassTransit Discord server.

[![alt Join the conversation](https://img.shields.io/discord/682238261753675864.svg "Discord")](https://discord.gg/rNpQgYn)

## GitHub Issues

> Please do not open an issue on GitHub, unless you have spotted an actual bug in MassTransit.

Use [GitHub Discussions](https://github.com/MassTransit/MassTransit/discussions) to ask questions, bring up ideas, or other general items. Issues are not the
place for questions, and will either be converted to a discussion or closed.
