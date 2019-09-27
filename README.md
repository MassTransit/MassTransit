MassTransit
===========

MassTransit is a _free, open-source_ distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

![Mass Transit](https://avatars2.githubusercontent.com/u/317796?s=200&v=4 "Mass Transit")

MassTransit is Apache 2.0 licensed.

Build Status
------------

Branch | Status
--- | :---:
master | [![master](https://ci.appveyor.com/api/projects/status/hox8dhh5eyy7jsf4/branch/master?svg=true)](https://ci.appveyor.com/project/phatboyg/masstransit/branch/master)
develop | [![develop](https://ci.appveyor.com/api/projects/status/hox8dhh5eyy7jsf4/branch/develop?svg=true)](https://ci.appveyor.com/project/phatboyg/masstransit/branch/develop)

MassTransit Nuget Packages
---------------------------

| Package Name | FullFramework | .NET Standard |
| ------------ | :-----------: | :-----------: |
| **Main** |
| [MassTransit][MassTransit.nuget] | 4.5.2 | 2.0 |
| **Other** |
| [MassTransit.Host][Host.nuget] | 4.5.2 | 2.0 |
| [MassTransit.Reactive][Reactive.nuget] | 4.5.2 | 2.0 |
| [MassTransit.SignalR][SignalR.nuget] | - | 2.0 |
| [MassTransit.TestFramework][TestFramework.nuget] | 4.5.2 | 2.0 |
| **Containers** |
| [MassTransit.Autofac][Autofac.nuget] | 4.5.2 | 2.0 |
| [MassTransit.Extensions.DependencyInjection][CoreDI.nuget] | 4.6.1 | 2.0 |
| [MassTransit.Lamar][Lamar.nuget] | 4.6.1 | 2.0 |
| [MassTransit.Ninject][Ninject.nuget] | 4.5.2 | 2.0 |
| [MassTransit.SimpleInjector][SimpleInjector.nuget] | 4.5.2 | 2.0 |
| [MassTransit.StructureMap][StructureMap.nuget] | 4.5.2 | 2.0 |
| [MassTransit.Unity][Unity.nuget] | 4.5.2 | 2.0 |
| [MassTransit.CastleWindsor][Windsor.nuget] | 4.5.2 | 2.0 |
| **Loggers** |
| [MassTransit.Extensions.Logging][CoreLogger.nuget] | 4.6.1 | 2.0 |
| [MassTransit.Log4Net][Log4Net.nuget] | 4.5.2 | 2.0 |
| [MassTransit.NLog][NLog.nuget] | 4.5.2 | 2.0 |
| [MassTransit.SerilogIntegration][Serilog.nuget] | 4.5.2 | 2.0 |
| **Monitoring** |
| [MassTransit.ApplicationInsights][AppInsights.nuget] | 4.5.2 | 2.0 |
| [MassTransit.AspNetCore][AspNetCore.nuget] | 4.5 | 2.0 |
| **Persistence** |
| [MassTransit.Dapper][Dapper.nuget] | 4.6.1 | - |
| [MassTransit.DocumentDb][DocumentDb.nuget] | 4.5.2 | 2.0 |
| [MassTransit.EntityFrameworkCore][EFCore.nuget] | 4.6.1 | 2.0 |
| [MassTransit.EntityFramework][EF.nuget] | 4.5.2 | - |
| [MassTransit.Marten][Marten.nuget] | 4.6.1 | 2.0 |
| [MassTransit.MongoDb][MongoDb.nuget] | 4.5.2 | 2.0 |
| [MassTransit.NHibernate][NHibernate.nuget] | 4.6.1 | 2.0 |
| [MassTransit.Redis][Redis.nuget] | 4.5.2 | 2.0 |
| **Transports** |
| [MassTransit.ActiveMQ][ActiveMQ.nuget] | 4.5 | 2.0 |
| [MassTransit.AmazonSQS][AmazonSQS.nuget] | 4.5 | 2.0 |
| [MassTransit.Azure.ServiceBus.Core][AzureSbCore.nuget] | 4.6.1 | 2.0 |
| [MassTransit.AzureServiceBus][AzureSb.nuget] | 4.5.2 | - |
| [MassTransit.Http][Http.nuget] | 4.5 | 2.0 |
| [MassTransit.RabbitMQ][RabbitMQ.nuget] | 4.5 | 2.0 |
| [MassTransit.WebJobs.EventHubs][EventHubs.nuget] | - | 2.0 |
| [MassTransit.WebJobs.ServiceBus][AzureFunc.nuget] | - | 2.0 |

## Getting started with MassTransit

In order to get started with MassTransit, you can have a look at the
documentation, which is located at [http://masstransit-project.com/MassTransit](http://masstransit-project.com/MassTransit).

### Simplest possible thing:

`install-package MassTransit.RabbitMq`

and then:

```csharp
// Message Definition
class MyMessage
{
	public string Value { get; set; }
}

// Code Snippet for Console Application
async Task Main()
{
	var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
	{
		var host = sbc.Host(new Uri("rabbitmq://localhost:/"), h =>
		{
			h.Username("guest");
			h.Password("guest");
		});

		sbc.ReceiveEndpoint(host, "my_queue", endpoint =>
		{
			endpoint.Handler<MyMessage>(async context =>
			{
				await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
			});
		});
	});

	await bus.StartAsync();

	await bus.Publish(new MyMessage { Value = "Hello, World." });

	Console.ReadLine();

	await bus.StopAsync();
}
```

You will also need to set up RabbitMQ; 

 1. **Install Erlang** using the [installer](http://www.erlang.org/download.html). (Next -> Next ...)
 2. **Install RabbitMQ** using the [installer](http://www.rabbitmq.com/download.html). (Next -> Next ...) You now have a RabbitMQ broker (look in `services.msc` for it) that you can [log into](http://localhost:15672/#/) using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 
 
**Please note** 

You need to add the management interface before you can login. First, from an elevated command prompt, change directory to the sbin folder within the RabbitMQ Server installation directory e.g. `%PROGRAMFILES%\RabbitMQ Server\rabbitmq_server_3.5.3\sbin\`

Next, run the following command to enable the rabbitmq management plugin:

`rabbitmq-plugins.bat enable rabbitmq_management`

### Downloads

Download from NuGet 'MassTransit' [Search NuGet for MassTransit](http://nuget.org/packages?q=masstransit)

Download the continuously integrated Nuget packages from [AppVeyor](https://ci.appveyor.com/project/phatboyg/masstransit/build/artifacts).

### Supported transports

We support RabbitMQ and Azure Service Bus message brokers.

## Mailing list

[MassTransit Discuss](http://groups.google.com/group/masstransit-discuss)

## Gitter Chat 

While attendance is pretty light, there is a Gitter chat room available:

[![Join the chat at https://gitter.im/MassTransit/MassTransit](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MassTransit/MassTransit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## GitHub Issues

**Pay attention**

Please do not open an issue on github, unless you have spotted an actual bug in MassTransit. 
If you are unsure, ask on the mailing list, and if we confirm it's a bug, we'll ask you to create the issue. 
Issues are not the place for questions, and they'll likely be closed.

This policy is in place to avoid bugs being drowned out in a pile of sensible suggestions for future 
enhancements and calls for help from people who forget to check back if they get it and so on.

## Building from Source

 1. Install the latest [.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) preview.
 1. Clone the source down to your machine.<br/>
    ```bash
    git clone git://github.com/MassTransit/MassTransit.git
    ```
 1. Run `build.ps1` or `build.sh`.

## Contributing

 1. Turn off `autocrlf`.
    ```bash
    git config core.autocrlf false
    ```
 1. Hack!
 1. Make a pull request.

## Builds

MassTransit is built on [AppVeyor](https://ci.appveyor.com/project/phatboyg/masstransit)
 
# REQUIREMENTS
* .Net 4.5.2 or .NET Standard 2.0

# CREDITS
Logo Design by [The Agile Badger](http://www.theagilebadger.com)

[MassTransit.nuget]: https://www.nuget.org/packages/MassTransit
[Host.nuget]: https://www.nuget.org/packages/MassTransit.Host
[Reactive.nuget]: https://www.nuget.org/packages/MassTransit.Reactive
[SignalR.nuget]: https://www.nuget.org/packages/MassTransit.SignalR
[TestFramework.nuget]: https://www.nuget.org/packages/MassTransit.TestFramework

[Autofac.nuget]: https://www.nuget.org/packages/MassTransit.Autofac
[CoreDI.nuget]: https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection
[Lamar.nuget]: https://www.nuget.org/packages/MassTransit.Lamar
[Ninject.nuget]: https://www.nuget.org/packages/MassTransit.Ninject
[SimpleInjector.nuget]: https://www.nuget.org/packages/MassTransit.SimpleInjector
[StructureMap.nuget]: https://www.nuget.org/packages/MassTransit.StructureMap
[Unity.nuget]: https://www.nuget.org/packages/MassTransit.Unity
[Windsor.nuget]: https://www.nuget.org/packages/MassTransit.CastleWindsor

[CoreLogger.nuget]: https://www.nuget.org/packages/MassTransit.Extensions.Logging
[Log4Net.nuget]: https://www.nuget.org/packages/MassTransit.Log4Net
[NLog.nuget]: https://www.nuget.org/packages/MassTransit.NLog
[Serilog.nuget]: https://www.nuget.org/packages/MassTransit.SerilogIntegration

[AppInsights.nuget]: https://www.nuget.org/packages/MassTransit.ApplicationInsights
[AspNetCore.nuget]: https://www.nuget.org/packages/MassTransit.AspNetCore

[Dapper.nuget]: https://www.nuget.org/packages/MassTransit.Dapper
[DocumentDb.nuget]: https://www.nuget.org/packages/MassTransit.DocumentDb
[EFCore.nuget]: https://www.nuget.org/packages/MassTransit.EntityFrameworkCore
[EF.nuget]: https://www.nuget.org/packages/MassTransit.EntityFramework
[Marten.nuget]: https://www.nuget.org/packages/MassTransit.Marten
[MongoDb.nuget]: https://www.nuget.org/packages/MassTransit.MongoDb
[NHibernate.nuget]: https://www.nuget.org/packages/MassTransit.NHibernate
[Redis.nuget]: https://www.nuget.org/packages/MassTransit.Redis

[ActiveMQ.nuget]: https://www.nuget.org/packages/MassTransit.ActiveMQ
[AmazonSQS.nuget]: https://www.nuget.org/packages/MassTransit.AmazonSQS
[AzureSbCore.nuget]: https://www.nuget.org/packages/MassTransit.Azure.ServiceBus.Core
[AzureSb.nuget]: https://www.nuget.org/packages/MassTransit.AzureServiceBus
[Http.nuget]: https://www.nuget.org/packages/MassTransit.Http
[RabbitMQ.nuget]: https://www.nuget.org/packages/MassTransit.RabbitMQ
[EventHubs.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.EventHubs
[AzureFunc.nuget]: https://www.nuget.org/packages/MassTransit.WebJobs.ServiceBus
