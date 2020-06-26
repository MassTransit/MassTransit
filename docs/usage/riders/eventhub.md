# Event Hub

Azure Event Hub is included as a [Rider](/usage/riders/), and supports consuming and producing messages from/to Azure event hubs.

> Uses [MassTransit.Azure.ServiceBus.Core](https://nuget.org/packages/MassTransit.Azure.ServiceBus.Core/), [MassTransit.EventHub](https://nuget.org/packages/MassTransit.EventHub/), [MassTransit.Extensions.DependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

To consume messages from an event hub, configure a Rider within the bus configuration as shown.

<<< @/docs/code/riders/EventHubConsumer.cs

The familiar _ReceiveEndpoint_ syntax is used to configure an event hub. The consumer group specified should be unique to the application, and shared by a cluster of service instances for load balancing. Consumers and sagas can be configured on the receive endpoint, which should be registered in the rider configuration. While the configuration for event hubs is the same as a receive endpoint, there is no implicit binding of consumer message types to event hubs (there is no pub-sub using event hub).

### Producers

Producing messages to event hubs uses a producer. In the example below, a messages is produced to the event hub.

<<< @/docs/code/riders/EventHubProducer.cs

