# SignalR Backplane

MassTransit offers a package which provides an easy option to get a SignalR Backplane up and running in with just a few lines of configuration. We won't go over the concept of a SignalR Backplane, more details can be found out about it [here](https://docs.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-in-signalr). This page is old, and references the .NET Framework SignalR, but the concepts of scaleout are the same for the newer .NET Core SignalR.

**.NET Framework SignalR _(which MassTransit does not support)_ Backplane Options:**
* SQLServer
* Redis
* Azure Service Bus

**.NET Core SignalR (which MassTransit _WILL_ work for) Backplane Options:**
* Redis (official)
* Azure SignalR Service (official)
* MassTransit (unofficial)
  * RabbitMq
  * ActiveMq
  * Azure Service Bus

* [Quick Start](quickstart.md)
* [Hub Endpoints](hub_endpoints.md)
* [Interop](interop.md)
* [Sample](sample.md)
* [Considerations](considerations.md)
