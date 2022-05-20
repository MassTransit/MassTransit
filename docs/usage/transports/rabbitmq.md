# RabbitMQ

> [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)

With tens of thousands of users, RabbitMQ is one of the most popular open source message brokers. RabbitMQ is lightweight and easy to deploy on premises and in the cloud. RabbitMQ can be deployed in distributed and federated configurations to meet high-scale, high-availability requirements.

MassTransit fully supports RabbitMQ, including many of the advanced features and capabilities. 

::: tip Getting Started
To get started with RabbitMQ, refer to the [configuration](/usage/configuration) section which uses RabbitMQ in the examples.
:::

## Minimal Example

In the example below, which configures a receive endpoint, consumer, and message type, the bus is configured to use RabbitMQ.

<<< @/docs/code/transports/RabbitMqConsoleListener.cs

## Broker Topology

With RabbitMQ, which supports exchanges and queues, messages are _sent_ or _published_ to exchanges and RabbitMQ routes those messages through exchanges to the appropriate queues.

When the bus is started, MassTransit will create exchanges and queues on the virtual host for the receive endpoint. MassTransit creates durable, _fanout_ exchanges by default, and queues are also durable by default.

## Configuration

The configuration includes:

* The RabbitMQ host
  - Host name: `localhost`
  - Virtual host: `/`
  - User name and password used to connect to the virtual host (credentials are virtual-host specific)
* The receive endpoint
  - Queue name: `order-events-listener`
  - Consumer: `OrderSubmittedEventConsumer`
    - Message type: `OrderSystem.Events.OrderSubmitted`

| Name | Description |
|:-----|:------------|
| order-events-listener | Queue for the receive endpoint
| order-events-listener | An exchange, bound to the queue, used to _send_ messages
| OrderSystem.Events:OrderSubmitted | An exchange, named by the message-type, bound to the _order-events-listener_ exchange, used to _publish_ messages

When a message is sent, the endpoint address can be one of two values:

`exchange:order-events-listener`

Send the message to the _order-events-listener_ exchange. If the exchange does not exist, it will be created. _MassTransit translates topic: to exchange: when using RabbitMQ, so that topic: addresses can be resolved – since RabbitMQ is the only supported transport that doesn't have topics._

`queue:order-events-listener`

Send the message to the _order-events-listener_ exchange. If the exchange or queue does not exist, they will be created and the exchange will be bound to the queue.

With either address, RabbitMQ will route the message from the _order-events-listener_ exchange to the _order-events-listener_ queue.

When a message is published, the message is sent to the _OrderSystem.Events:OrderSubmitted_ exchange. If the exchange does not exist, it will be created. RabbitMQ will route the message from the _OrderSystem.Events:OrderSubmitted_ exchange to the _order-events-listener_ exchange, and subsequently to the _order-events-listener_ queue. If other receive endpoints connected to the same virtual host include consumers that consume the _OrderSubmitted_ message, a copy of the message would be routed to each of those endpoints as well.

::: warning
If a message is published before starting the bus, so that MassTransit can create the exchanges and queues, the exchange _OrderSystem.Events:OrderSubmitted_ will be created. However, until the bus has been started at least once, there won't be a queue bound to the exchange and any published messages will be lost. Once the bus has been started, the queue will remain bound to the exchange even when the bus is stopped.
:::

Durable exchanges and queues remain configured on the virtual host, so even if the bus is stopped messages will continue to be routed to the queue. When the bus is restarted, queued messages will be consumed.

MassTransit includes several host-level configuration options that control the behavior for the entire bus.

|  Property                      | Type   | Description 
|-------|------------------------|--------|---
| PublisherConfirmation        | bool | MassTransit will wait until RabbitMQ confirms messages when publishing or sending messages (default: true)
| Heartbeat                    | TimeSpan |The heartbeat interval used by the RabbitMQ client to keep the connection alive
| RequestedChannelMax          | ushort | The maximum number of channels allowed on the connection
| RequestedConnectionTimeout   | TimeSpan | The connection timeout

#### UseCluster

MassTransit can connect to a cluster of RabbitMQ virtual hosts and treat them as a single virtual host. To configure a cluster, call the `UseCluster` methods, and add the cluster nodes, each of which becomes part of the virtual host identified by the host name. Each cluster node can specify either a `host` or a `host:port` combination.

#### ConfigureBatch

MassTransit will briefly buffer messages before sending them to RabbitMQ, to increase message throughput. While use of the default values is recommended, the batch options can be configured.

|  Property               | Type   | Default |Description 
|-------|------------------------|-----|--------|---
| Enabled        | bool | true | Enable or disable batch sends to RabbitMQ
| MessageLimit        | int | 100 | Limit the number of messages per batch
| SizeLimit        | int | 64K | A rough limit of the total message size
| Timeout        | TimeSpan | 1ms | The time to wait for additional messages before sending

MassTransit includes several receive endpoint level configuration options that control receive endpoint behavior.

| Property                | Type   | Description 
|-------------------------|--------|------------------
| PrefetchCount         | ushort | The number of unacknowledged messages that can be processed concurrently (default based on CPU count)
| PurgeOnStartup        | bool   | Removes all messages from the queue when the bus is started (default: false)
| AutoDelete         | bool | If true, the queue will be automatically deleted when the bus is stopped (default: false)
| Durable        | bool   | If true, messages are persisted to disk before being acknowledged (default: true)

## Additional Examples

### CloudAMQP

MassTransit can be used with CloudAMQP, which is a great SaaS-based solution to host your RabbitMQ broker. To configure MassTransit, the host and virtual host must be specified, and _UseSsl_ must be configured. 

<<< @/docs/code/transports/CloudAmqpConsoleListener.cs

### AmazonMQ - RabbitMQ

AmazonMQ now includes [RabbitMQ support](https://us-east-2.console.aws.amazon.com/amazon-mq/home), which means the best message broker can now be used easily on AWS. To configure MassTransit, the AMQPS endpoint address can be used to configure the host as shown below. 

<<< @/docs/code/transports/AmazonRabbitMqConsoleListener.cs
