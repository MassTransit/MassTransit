# Send Topology

Topology does not cover sending messages beyond delivering messages to a queue. MassTransit sends messages via a _send endpoint_, which is retrieved using the endpoint's address only.

The exception to this is when the transport supports additional capabilities on send, such as the partitioning of messages. With RabbitMQ this would include specifying the `RoutingKey`, and with Azure Service Bus this would include specifying the `PartitionId` or the `SessionId`.

> Topology cannot alter the destination of a message, only the properties of the message delivery itself. Determining the path of a message is routing, which is handled separately.
