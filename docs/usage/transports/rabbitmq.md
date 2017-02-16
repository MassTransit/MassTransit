# RabbitMQ configuration options

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

The code below configures one bus instance and one host with the specified base address.
This bus instance can be used to send and publish messages.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(new Uri("rabbitmq://a-machine-name/a-virtual-host"), host =>
    {
        host.Username("username");
        host.Password("password");
    });
});
await busControl.StartAsync();
```

In order to consume messages, you must configure one or more receive endpoints.
To do this, include the endpoint configuration inside the configuration delegate:

```csharp
cfg.Host(new Uri("rabbitmq://a-machine-name/a-virtual-host"), host =>
{
    host.Username("username");
    host.Password("password");

    cfg.ReceiveEndpoint("queue-name", ec => {
        // endpoint configuration

        // for example, MyMessageConsumer consumes MyMessage
        ec.Consumer<MyMessageConsumer>(); 
    });
});
```

RabbitMQ transport will then set up necessary infrastructure elements, such as:
 * Endpoint queue `queue-name`
 * Endpoint exchange `queue-name`
 * Message type exchange `MyMessage`
 * Binding between `MyMessage`and `queue-name` exchanges
 * Binding between `queue-name` exchange and `queue-name` queue

This will result in all messages that will be published to the `MyMessage` exchange to be
also delivered to the `queue-name` queue.

The infrastructure elements are only created if they do not exist yet. All elements
are by default durable. MassTransit will also create a number of elements that are
not durable and these will be removed as soon as the service stops.

All durable elements remain running on RabbitMQ and this means that even if the
service is down and not consuming messages, messages will still be accumulated in the queue
so when the service comes online, all queued messages will be consumed.

There are additional configuration options for RabbitMQ transport, that can be applied 
when the bus is being configured:

| Level | Property                | Type   | Default | Description 
|-------|-------------------------|--------|---------|------------------
| Bus   | `PrefetchCount`         | `ushort` | Processor count multiplied by 4 | The number of messages to fetch in the buffer
| Bus   | `PurgeOnStartup`        | `bool`   | `false` | Forces the bus to clean up messages from the queue when starting
| Host  | `Username`              | `string` |       | User name for RabbitMQ
| Host  | `Password`              | `string` |       | Password for RabbitMQ
| Host  | `ClusterMembers`        | `string[]` |     | List of cluster member addresses
| Host  | `PublisherConfirmation` | `bool` | `true` | instructs if MassTransit should wait for a confirmation when publishing or sending messages. 

