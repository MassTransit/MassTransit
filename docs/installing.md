---
layout: page
title: Installing MassTransit
---
This section of the online docs will explain how to get MassTransit into your project. It will also show you where to get help, how to report bugs, etc. Hopefully, you will find it useful as you explore the MassTransit framework.

MassTransit is written in C# and requires a compatible .NET framework. MassTransit is built and tested with .NET 4.5, and the shipped assemblies target this version. Visual Studio 2015 is used, and the new C# 6 syntax is used.

_For comparison, my development machines is a MacBook Pro hosting a Windows Server 2012 R2 virtual machine (via VMware Fusion), with Visual Studio 2015 and Resharper 9.x installed. My secondary rig is a Razer Blade running Windows 10, which also works just fine._

> **Note**
> With the release of Mono 4.0, and the upcoming release of .NET Core, MassTransit will be soon target both Windows and Mac/Linux applications. At this time, however, this has not been fully tested by the primary contributors.

## Transports
MassTransit leverages existing message transports, so you will need to have a supported transport installed.

### In Memory
The in memory transport is included with MassTransit. No additional software is required.

### RabbitMQ
To use RabbitMQ, download and install the version appropriate for your operating system. Once the broker is installed, enable some additional plug-ins for management and message tracking.

Then, install the `MassTransit.RabbitMQ` package in your project and follow the RabbitMQ configuration guide.

### Azure Service Bus
Azure Service Bus is a generic, cloud-based messaging system for connecting just about anything—applications, services, and devices—wherever they are. Connect apps running on Azure, on-premises systems, or both. You can even use Service Bus to connect household appliances, sensors, and other devices like tablets or phones to a central application or to each other.

To use Azure Service Bus with MassTransit, install the `MassTransit.AzureServiceBus` package in your project and follow the Service Bus configuration guide.
