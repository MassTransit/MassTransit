# Publish Topology

Topology is a key part of publishing messages, and is responsible for how the broker's facilities are configured.

The publish topology defines many aspects of broker configuration, including:

- RabbitMQ Exchange names or Azure Service Bus Topic names
  - Formatted, based upon the message type
  - Explicit, based upon the configuration
- RabbitMQ Exchange Bindings or Azure Service Bus Topic Subscriptions

When `Publish` is called, the topology is also used to:

- Populate the `RoutingKey` of the message sent to the RabbitMQ exchange
- Populate the `PartitionId` or `SessionId` of the message sent to the Azure Service Bus topic