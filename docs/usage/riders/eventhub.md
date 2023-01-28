# Event Hub

Azure Event Hub is included as a [Rider](/usage/riders/), and supports consuming and producing messages from/to Azure event hubs.

> Uses [MassTransit.Azure.ServiceBus.Core](https://nuget.org/packages/MassTransit.Azure.ServiceBus.Core/), [MassTransit.EventHub](https://nuget.org/packages/MassTransit.EventHub/), [MassTransit.Extensions.DependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

To consume messages from an event hub, configure a Rider within the bus configuration as shown.

<<< @/docs/code/riders/EventHubConsumer.cs

The familiar _ReceiveEndpoint_ syntax is used to configure an event hub. The consumer group specified should be unique to the application, and shared by a cluster of service instances for load balancing. Consumers and sagas can be configured on the receive endpoint, which should be registered in the rider configuration. While the configuration for event hubs is the same as a receive endpoint, there is no implicit binding of consumer message types to event hubs (there is no pub-sub using event hub).

### Configuration

#### Checkpoint

Rider implementation is taking full responsibility of Checkpointing, there is no ability to change it.
Checkpointer can be configured on topic bases through next properties:

| Name                   | Description                                           | Default |
|:-----------------------|:------------------------------------------------------|:-----|
| CheckpointInterval     | Checkpoint frequency based on time                    | 1 min
| CheckpointMessageCount | Checkpoint every X messages                           | 5000
| MessageLimit           | Checkpointer buffer size without blocking consumption | 10000

> Please note, each topic partition has it's own checkpointer and configuration is applied to partition and not to entire topic.

During graceful shutdown Checkpointer will try to "checkpoint" all already consumed messages. Force shutdown should be avoided to prevent multiple consumption for the same message.

#### Scalability
Riders are designed with performance in mind, handling each topic partition withing separate threadpool. As well, allowing to scale-up consumption within same partition by using PartitionKey, as long as keys are different they will be processed concurrently and all this **without** sacrificing ordering.

| Name                    | Description                                                                                                                                                                      | Default |
|:------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:-----|
| ConcurrentDeliveryLimit | Number of Messages delivered concurrently within same partition + PartitionKey. Increasing this value will **break ordering**, helpful for topics where ordering is not required | 1
| ConcurrentMessageLimit  | Number of Messages processed concurrently witin different keys (preserving ordering). When keys are the same for entire partition `ConcurrentDeliveryLimit` will be used instead | 1
| PrefetchCount           | Number of Messages to prefetch from kafka topic into memory                                                                                                                      | 1000

### Producers

Producing messages to event hubs uses a producer. In the example below, a messages is produced to the event hub.

<<< @/docs/code/riders/EventHubProducer.cs

