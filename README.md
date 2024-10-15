MassTransit
===========

MassTransit is a _free, open-source_ distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

![Mass Transit](https://avatars2.githubusercontent.com/u/317796?s=200&v=4 "Mass Transit")

MassTransit is Apache 2.0 licensed.

## Documentation

Get started by [reading through the documentation](https://masstransit-project.com/).

Build Status
------------

| Branch        |                                                                                                Status                                                                                                |
|---------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| master        |    [![master](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=master&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml)    |
| develop       |   [![develop](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml/badge.svg?branch=develop&event=push)](https://github.com/MassTransit/MassTransit/actions/workflows/build.yml)   |

MassTransit NuGet Packages
---------------------------

| Package Name                                                    |   .NET   | .NET Standard | .NET Framework |
|-----------------------------------------------------------------|:--------:|:-------------:|:--------------:|
| **Main**                                                        |          |               |                |
| [MassTransit][MassTransit.nuget]                                | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Abstractions][MassTransitAbstractions.nuget]       | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Newtonsoft][MassTransitNewtonsoft.nuget]           | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.MessagePack][MassTransitMessagePack.nuget]         | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Other**                                                       |          |               |                |
| [MassTransit.Analyzers][Analyzers.nuget]                        |          |      2.0      |                |
| [MassTransit.Templates][Templates.nuget]                        |   6.0    |               |                |
| [MassTransit.SignalR][SignalR.nuget]                            | 6.0, 8.0 |               |     4.7.2      |
| [MassTransit.Interop.NServiceBus][MassTransitNServiceBus.nuget] | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.TestFramework][TestFramework.nuget]                | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Monitoring**                                                  |          |               |                |
| [MassTransit.Prometheus][Prometheus.nuget]                      | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Persistence**                                                 |          |               |                |
| [MassTransit.AmazonS3][AmazonS3.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Azure.Cosmos][Cosmos.nuget]                        | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Azure.Storage][AzureStorage.nuget]                 | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Azure.Table][AzureTable.nuget]                     | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Dapper][Dapper.nuget]                              | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.DynamoDb][DynamoDb.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.EntityFrameworkCore][EFCore.nuget]                 | 6.0, 8.0 |      2.0      |                |
| [MassTransit.EntityFramework][EF.nuget]                         |          |      2.1      |     4.7.2      |     
| [MassTransit.Marten][Marten.nuget]                              | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.MongoDb][MongoDb.nuget]                            | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.NHibernate][NHibernate.nuget]                      | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Redis][Redis.nuget]                                | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Scheduling**                                                  |          |               |                |
| [MassTransit.Hangfire][Hangfire.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Quartz][Quartz.nuget]                              | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Transports**                                                  |          |               |                |
| [MassTransit.ActiveMQ][ActiveMQ.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.AmazonSQS][AmazonSQS.nuget]                        | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.Azure.ServiceBus.Core][AzureSbCore.nuget]          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.RabbitMQ][RabbitMQ.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.SqlTransport.PostgreSQL][PostgreSQL.nuget]         | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.SqlTransport.SqlServer][SqlServer.nuget]           | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.WebJobs.EventHubs][EventHubs.nuget]                | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.WebJobs.ServiceBus][AzureFunc.nuget]               | 6.0, 8.0 |      2.0      |     4.7.2      |
| **Riders**                                                      |          |               |                |
| [MassTransit.Kafka][Kafka.nuget]                                | 6.0, 8.0 |      2.0      |     4.7.2      |
| [MassTransit.EventHub][EventHub.nuget]                          | 6.0, 8.0 |      2.0      |     4.7.2      |

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

 1. Install the latest [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
 2. Clone the source down to your machine<br/>
    ```bash
    git clone https://github.com/MassTransit/MassTransit.git
    ```
 3. Run `dotnet build`

## Contributing

 1. Turn off `autocrlf`
    ```bash
    git config core.autocrlf false
    ```
 2. Hack!
 3. Make a pull request
 
# REQUIREMENTS
* .NET 8 SDK

# CREDITS
Logo Design by _The Agile Badger_

[MassTransit.nuget]: https://www.nuget.org/packages/MassTransit
[MassTransitAbstractions.nuget]: https://www.nuget.org/packages/MassTransit.Abstractions
[MassTransitNewtonsoft.nuget]: https://www.nuget.org/packages/MassTransit.Newtonsoft
[MassTransitMessagePack.nuget]: https://www.nuget.org/packages/MassTransit.MessagePack
[MassTransitNServiceBus.nuget]: https://www.nuget.org/packages/MassTransit.Interop.NServiceBus
[Analyzers.nuget]: https://www.nuget.org/packages/MassTransit.Analyzers
[Templates.nuget]: https://www.nuget.org/packages/MassTransit.Templates
[SignalR.nuget]: https://www.nuget.org/packages/MassTransit.SignalR
[TestFramework.nuget]: https://www.nuget.org/packages/MassTransit.TestFramework

[Prometheus.nuget]: https://www.nuget.org/packages/MassTransit.Prometheus

[Cosmos.nuget]: https://www.nuget.org/packages/MassTransit.Azure.Cosmos
[AzureStorage.nuget]: https://www.nuget.org/packages/MassTransit.Azure.Storage
[AzureTable.nuget]: https://www.nuget.org/packages/MassTransit.Azure.Table
[Dapper.nuget]: https://www.nuget.org/packages/MassTransit.DapperIntegration
[DynamoDb.nuget]: https://www.nuget.org/packages/MassTransit.DynamoDb
[EFCore.nuget]: https://www.nuget.org/packages/MassTransit.EntityFrameworkCore
[EF.nuget]: https://www.nuget.org/packages/MassTransit.EntityFramework
[Marten.nuget]: https://www.nuget.org/packages/MassTransit.Marten
[MongoDb.nuget]: https://www.nuget.org/packages/MassTransit.MongoDb
[NHibernate.nuget]: https://www.nuget.org/packages/MassTransit.NHibernate
[Redis.nuget]: https://www.nuget.org/packages/MassTransit.Redis

[Hangfire.nuget]: https://www.nuget.org/packages/MassTransit.Hangfire
[Quartz.nuget]: https://www.nuget.org/packages/MassTransit.Quartz

[ActiveMQ.nuget]: https://www.nuget.org/packages/MassTransit.ActiveMQ
[AmazonS3.nuget]: https://www.nuget.org/packages/MassTransit.AmazonS3
[AmazonSQS.nuget]: https://www.nuget.org/packages/MassTransit.AmazonSQS
[AzureSbCore.nuget]: https://www.nuget.org/packages/MassTransit.Azure.ServiceBus.Core
[RabbitMQ.nuget]: https://www.nuget.org/packages/MassTransit.RabbitMQ
[PostgreSQL.nuget]: https://nuget.org/packages/MassTransit.SqlTransport.PostgreSQL/
[SqlServer.nuget]: https://nuget.org/packages/MassTransit.SqlTransport.SqlServer/
[EventHubs.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.EventHubs
[AzureFunc.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.ServiceBus

[Kafka.nuget]: https://www.nuget.org/packages/MassTransit.Kafka
[EventHub.nuget]: https://www.nuget.org/packages/MassTransit.EventHub
