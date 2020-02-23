# RabbitMQ

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

The code below configures one bus instance and one host with the specified base address. This bus instance can be used to send and publish messages.

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

In order to consume messages, you must configure one or more receive endpoints. To do this, include the endpoint configuration inside the configuration delegate:

```csharp
namespace MyApplication
{
    public class MyMessage
    {
    }

    public class MyMessageConsumer :
        IConsumer<MyMessage>
    {
        public async Task Consume(ConsumeContext<MyMessage> context)
        {            
        }
    }

    // configured in Program.cs, or wherever
    cfg.Host("localhost", "virtual-host");

    cfg.ReceiveEndpoint("queue-name", ec => 
    {
        // for example, MyMessageConsumer consumes MyMessage
        ec.Consumer<MyMessageConsumer>(); 
    });
}
```

RabbitMQ transport will then set up necessary infrastructure elements, such as:
 * Endpoint queue `queue-name`
 * Endpoint exchange `queue-name`
 * Message type exchange `MyApplication.MyMessage`
 * Binding between `MyApplication.MyMessage`and `queue-name` exchanges
 * Binding between `queue-name` exchange and `queue-name` queue

This will result in all messages that will be published to the `MyMessage` exchange to be also delivered to the `queue-name` queue.

When a message is published, the following happens under the hood:

- Application calls _Publish_ passing _MyMessage_.
- MassTransit serializes the message and sends it to the `MyApplication.MyMessage` exchange
- RabbitMQ routes the message to the `queue-name` exchange
- RabbitMQ delivers the message to the `queue-name` queue

The infrastructure elements are only created if they do not exist yet. All elements are by default durable. MassTransit will also create a number of elements that are not durable and these will be removed as soon as the service stops. By default, _Fanout_ exchanges are used.

::: warning
If a message is published prior to starting the receive endpoint (which configures the broker topology), the exchange
<i>MyApplication.MyMessage</i> will be created by _Publish_. However, it won't be bound to anything until the receive endpoint is started.
Until the message exchange is bound, published messages will just disappear.
:::

All durable elements remain running on RabbitMQ and this means that even if the service is down and not consuming messages, messages will still be accumulated in the queue so when the service comes online, all queued messages will be consumed.

There are additional configuration options for RabbitMQ transport, that can be applied when the bus is being configured:

| Level | Property                | Type   | Default | Description 
|-------|-------------------------|--------|---------|------------------
| Bus   | `PrefetchCount`         | `ushort` | Processor count multiplied by 4 | The number of messages to fetch in the buffer
| Bus   | `PurgeOnStartup`        | `bool`   | `false` | Forces the bus to clean up messages from the queue when starting
| Host  | `Username`              | `string` |       | User name for RabbitMQ
| Host  | `Password`              | `string` |       | Password for RabbitMQ
| Host  | `ClusterMembers`        | `string[]` |     | List of cluster member addresses
| Host  | `PublisherConfirmation` | `bool` | `true` | instructs if MassTransit should wait for a confirmation when publishing or sending messages. 


## CloudAMQP

MassTransit works great with CloudAMQP, and is an easy way to get started. It's highly recommended to use SSL, an example configuration is shown below. Note that the port number _may_ need to be specified, in addition to the `UseSsl` configuration.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
{
    var host = x.Host(new Uri("rabbitmq://wombat.rmq.cloudamqp.com:5671/your_vhost/"), h =>
    {
        h.Username("your_username");
        h.Password("your_password");

        h.UseSsl(s =>
        {
            s.Protocol = SslProtocols.Tls12;
        });
    });
});
```

















## Publishing a message

When you publish a message on the bus here is what happens:

- Publish `MySystem.Messages.SomeMessage`
- This message gets published by the publishing logic to the exchange `MySystem.Messages.SomeMessage`
- The message is routed by messaging infrastructure to the `my_endpoint` exchange
- The message is then routed to the `my_endpoint` queue

<div class="alert alert-info">
<b>Note:</b>
If you publish a message before the consumer has been started (and created its configuration), the exchange
<i>MySystem.Messages.SomeMessage</i> will be created. It will not be bound to anything until the consumer starts,
so if you publish to it, the message will just disappear.
</div>

## Queues

- Each application you write should use a unique queue name.
- If you run multiple copies  of your consumer service, they would listen to the same queue (as they are copies).
  This would mean you have multiple applications listening to `my_endpoint` queue
  This would result in a 'competing consumer' scenario.  (Which is what you want if you run same service multiple times)
- If there is an exception from your consumer, the message will be sent to `my_endpoint_error` queue.
- If a message is received in a queue that the consumer does not know how to handle, the message will be sent to `my_endpoint_skipped` queue.

## Design Benefits

- Any application can listen to any message and that will not affect any other application that may or may not be listening for that message
- Any application(s) that bind a group of messages to the same queue will result in the competing consumer pattern.
- You do not have to concern yourself with anything but what message type to produce and what message type to consume.

## Faq

- How many messages at a time will be simultaneously processed?
  - Each endpoint you create represents 1 queue.  That queue can receive any number of different message types (based on what you subscribe to it)
  - The configuration of each endpoint you can set the number of consumers with a call to `PrefetchCount(x)`.
  - This is the total number of consumers for all message types sent to this queue.
  - In MT2, you had to add ?prefetch=X to the Rabbit URL. This is handled automatically now.

- Can I have a set number of consumers per message type?
  - Yes. This uses middleware.

    `x.Consumer(new AutofacConsumerFactory<…>(), p => p.UseConcurrencyLimit(1));  x.PrefetchCount=16;`

     PrefetchCount should be relatively high, a multiple of your concurrency limit for all message types so that RabbitMQ doesn't choke delivery messages due to network delays. Always have a queue ready to receive the message.

- When my consumer is not running, I do not want the messages to wait in the queue.  How can I do this?
  - There are two ways.  Note that each of these imply you would never use a 'competing consumer' pattern, so make sure that is the case.
    1. Set `PurgeOnStartup=true` in the endpoint configuration. When the bus starts, it will empty the queue of all messages.
    1. Set `AutoDelete=true` in the endpoint configuration. This causes the queue to be removed when your application stops.

- How are Retries handled?
  - This is handled by [middleware](../../advanced/middleware/README.md). Each endpoint has a [retry policy](../exceptions.md#retry).

- Can I have a different retry policy per each message type?
  - No. This is set at an endpoint level. You would have to have a specific queue per consumer to achieve this.




