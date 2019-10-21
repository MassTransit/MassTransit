# Topology

Topology is how MassTransit configures the broker capabilities (exchanges and queues for RabbitMQ, topics, subscriptions, and queues for Azure Service Bus) to support sending, publishing, and consuming messages. MassTransit's topology provides access to advanced broker features, making it easy to configure and use them in applications. Topology is also used to define conventions which can be applied to each message type, reducing the amount of code written to handle things like message correlation identifiers and setting broker message properties.

Topology also defines how things are named, including RabbitMQ exchanges and Azure Service Bus topics -- both of which are used for publishing messages.

* [Message](message.md)
* [Publish](publish.md)
* [Send](send.md)
* [Consume](consume.md)
* [Conventions](conventions.md)
* [RabbitMQ](rabbitmq/README.md)
  * [Exchange Binding](rabbitmq/binding.md)
  * [RoutingKey](rabbitmq/routingkey.md)
* [Azure Service Bus](servicebus/README.md)
  * [SessionId](servicebus/sessionid.md)
  * [PartitionKey](servicebus/partitionkey.md)
  * [Topic Subscription](servicebus/topicsub.md)

You can also view a visual representation of the [default broker topologies](../../understand/default-topology.md) created.