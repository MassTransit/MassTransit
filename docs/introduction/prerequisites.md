# Prerequisites

MassTransit is written in C\# and requires a compatible .NET framework. MassTransit is built and tested with
.NET 4.5.2 and .NET Standard 2.0. The development team uses both Visual Studio 2017 and JetBrains Rider, either of which
can be used with the supplied NuGet packages.

_For comparison, my development machines is a MacBook Pro using JetBrains Rider. I test using both Mono (for the .NET
4.5.2 builds) and .NET Core Standard 2.0 (natively on Mac OS X). My secondary rig is a Razer Blade running Windows 10, which also works just fine with both Visual Studio 2017 and JetBrains Rider._

## Transports

MassTransit leverages existing message transports, so you will need to have a supported transport installed.

### In Memory

The [in memory](https://github.com/MassTransit/MassTransit/blob/develop/doc/source/configuration/transports/in_memory.html)
transport is included with MassTransit. No additional software is required.

### RabbitMQ

To use RabbitMQ, [download](http://www.rabbitmq.com/download.html) and
[install](http://www.rabbitmq.com/install-windows.html) the version appropriate for your operating system.
Once the broker is installed, enable some additional plug-ins for management and message tracking.

```text
rabbitmq-plugins.bat enable rabbitmq_management
```

Then, install the `MassTransit.RabbitMQ` package in your project and follow the [RabbitMQ](https://github.com/MassTransit/MassTransit/blob/develop/doc/source/configuration/transports/rabbitmq.html) configuration guide.

### Azure Service Bus

[Azure Service Bus](http://azure.microsoft.com/en-us/services/service-bus/)is a generic,
cloud-based messaging system for connecting just about anything—applications, services, and devices—wherever they are.
Connect apps running on Azure, on-premises systems, or both. You can even use Service Bus to connect
household appliances, sensors, and other devices like tablets or phones to a central application or to each other.

To use Azure Service Bus with MassTransit, install the `MassTransit.AzureServiceBus` package in your project
and follow the [Service Bus](https://github.com/MassTransit/MassTransit/blob/develop/doc/source/configuration/transports/azure.html)
configuration guide.

### ActiveMQ

ActiveMQ is supported, both running as a service (either IAAS, or on-premise), or hosted via Amazon MQ. Support for ActiveMQ
requires the `MassTransit.ActiveMQ` NuGet package.