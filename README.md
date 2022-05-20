MassTransit
===========

MassTransit is a _free, open-source_ distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

![Mass Transit](https://avatars2.githubusercontent.com/u/317796?s=200&v=4 "Mass Transit")

MassTransit is Apache 2.0 licensed.

## Documentation

Get started by [reading through the documentation](https://masstransit-project.com/).

Build Status
------------

Branch | Status
--- | :---:
master | [![master](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=master&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml)
develop | [![develop](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=develop&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml)
documentation | [![documentation](https://github.com/MassTransit/MassTransit/actions/workflows/docs.yml/badge.svg?branch=develop&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/docs.yml)

MassTransit NuGet Packages
---------------------------

| Package Name | .NET Standard | .NET Core App |
| ------------ | :-----------: | :----------: |
| **Main** |
| [MassTransit][MassTransit.nuget] | 2.0 |
| [MassTransit.Abstractions][MassTransitAbstractions.nuget] | 2.0 |
| **Other** |
| [MassTransit.Analyzers][Analyzers.nuget] | 2.0 |
| [MassTransit.Templates][Templates.nuget] | 5.0 |
| [MassTransit.SignalR][SignalR.nuget] | 2.0 |
| [MassTransit.TestFramework][TestFramework.nuget] | 2.0 |
| **Monitoring** |
| [MassTransit.Prometheus][Prometheus.nuget] | 2.0 |
| **Persistence** |
| [MassTransit.Azure.Cosmos][Cosmos.nuget] | 2.0 |
| [MassTransit.Azure.Storage][AzureStorage.nuget] | 2.0 |
| [MassTransit.Dapper][Dapper.nuget] | 2.0 |
| [MassTransit.EntityFrameworkCore][EFCore.nuget] | 2.0 or 6.0 |
| [MassTransit.EntityFramework][EF.nuget] | 2.1 |
| [MassTransit.Marten][Marten.nuget] | 2.0 |
| [MassTransit.MongoDb][MongoDb.nuget] | 2.0 |
| [MassTransit.NHibernate][NHibernate.nuget] | 2.0 |
| [MassTransit.Redis][Redis.nuget] | 2.0 |
| **Scheduling** |
| [MassTransit.Hangfire][Hangfire.nuget] | 2.0 |
| [MassTransit.Quartz][Quartz.nuget] | 2.0 |
| **Transports** |
| [MassTransit.ActiveMQ][ActiveMQ.nuget] | 2.0 |
| [MassTransit.AmazonSQS][AmazonSQS.nuget] | 2.0 |
| [MassTransit.Azure.ServiceBus.Core][AzureSbCore.nuget] | 2.0 |
| [MassTransit.Grpc][Grpc.nuget] | 2.0 |
| [MassTransit.RabbitMQ][RabbitMQ.nuget] | 2.0 |
| [MassTransit.WebJobs.EventHubs][EventHubs.nuget] | 2.0 |
| [MassTransit.WebJobs.ServiceBus][AzureFunc.nuget] | 2.0 |
| **Riders** |
| [MassTransit.Kafka][Kafka.nuget] | 2.0 |
| [MassTransit.EventHub][EventHub.nuget] | 2.0 |

## Discord 

Get help live at the MassTransit Discord server.

[![alt Join the conversation](https://img.shields.io/discord/682238261753675864.svg "Discord")](https://discord.gg/rNpQgYn)

## GitHub Issues

**Pay attention**

Please do not open an issue on GitHub, unless you have spotted an actual bug in MassTransit. 

Use [GitHub Discussions](https://github.com/MassTransit/MassTransit/discussions) to ask questions, bring up ideas, or other general items. Issues are not the place for questions, and will either be converted to a discussion or closed.

This policy is in place to avoid bugs being drowned out in a pile of sensible suggestions for future 
enhancements and calls for help from people who forget to check back if they get it and so on.

## Building from Source

 1. Install the latest [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
 1. Clone the source down to your machine<br/>
    ```bash
    git clone git://github.com/MassTransit/MassTransit.git
    ```
 1. Run `dotnet build`

## Contributing

 1. Turn off `autocrlf`
    ```bash
    git config core.autocrlf false
    ```
 1. Hack!
 1. Make a pull request
 
# REQUIREMENTS
* .NET 6 SDK

# CREDITS
Logo Design by _The Agile Badger_

[MassTransit.nuget]: https://www.nuget.org/packages/MassTransit
[MassTransitAbstractions.nuget]: https://www.nuget.org/packages/MassTransit.Abstractions
[Analyzers.nuget]: https://www.nuget.org/packages/MassTransit.Analyzers
[Templates.nuget]: https://www.nuget.org/packages/MassTransit.Templates
[SignalR.nuget]: https://www.nuget.org/packages/MassTransit.SignalR
[TestFramework.nuget]: https://www.nuget.org/packages/MassTransit.TestFramework

[Prometheus.nuget]: https://www.nuget.org/packages/MassTransit.Prometheus

[Cosmos.nuget]: https://www.nuget.org/packages/MassTransit.Azure.Cosmos
[AzureStorage.nuget]: https://www.nuget.org/packages/MassTransit.Azure.Storage
[Dapper.nuget]: https://www.nuget.org/packages/MassTransit.DapperIntegration
[EFCore.nuget]: https://www.nuget.org/packages/MassTransit.EntityFrameworkCore
[EF.nuget]: https://www.nuget.org/packages/MassTransit.EntityFramework
[Marten.nuget]: https://www.nuget.org/packages/MassTransit.Marten
[MongoDb.nuget]: https://www.nuget.org/packages/MassTransit.MongoDb
[NHibernate.nuget]: https://www.nuget.org/packages/MassTransit.NHibernate
[Redis.nuget]: https://www.nuget.org/packages/MassTransit.Redis

[Hangfire.nuget]: https://www.nuget.org/packages/MassTransit.Hangfire
[Quartz.nuget]: https://www.nuget.org/packages/MassTransit.Quartz

[ActiveMQ.nuget]: https://www.nuget.org/packages/MassTransit.ActiveMQ
[AmazonSQS.nuget]: https://www.nuget.org/packages/MassTransit.AmazonSQS
[AzureSbCore.nuget]: https://www.nuget.org/packages/MassTransit.Azure.ServiceBus.Core
[Grpc.nuget]: https://www.nuget.org/packages/MassTransit.Grpc
[RabbitMQ.nuget]: https://www.nuget.org/packages/MassTransit.RabbitMQ
[EventHubs.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.EventHubs
[AzureFunc.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.ServiceBus

[Kafka.nuget]: https://www.nuget.org/packages/MassTransit.Kafka
[EventHub.nuget]: https://www.nuget.org/packages/MassTransit.EventHub
