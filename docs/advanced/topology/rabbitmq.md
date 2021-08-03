# RabbitMQ

The send and publish topologies are extended to support RabbitMQ features, and make it possible to configure how exchanged are created.

## Exchanges

When a message is published, MassTransit sends it to an exchange that is named based upon the message type. Using topology, the exchange name, as well as the exchange properties can be configured to support a custom behavior.

To configure the properties used when an exchange is created, the publish topology can be configured during bus creation:

<<< @/docs/code/topology/TopologyRabbitMqPublish.cs

### Exchange Layout

In versions of MassTransit prior to 4.x, every implemented type was connected directly to the top-level exchange for the published message type. Starting with v4.0, the broker topology for inherited types can be configured to maintain the type hierarchy, which can significantly reduce the number of exchange bindings in some cases. To configure this new behavior, the publish topology is used to specify the broker topology option.

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;
});
```

### Exchange Binding

To bind an exchange to a receive endpoint:

```csharp
cfg.ReceiveEndpoint("input-queue", e =>
{
    e.Bind("exchange-name");
    e.Bind<MessageType>();
})
```

The above will create two exchange bindings, one between the `exchange-name` exchange and the `input-queue` exchange and a second between the exchange name matching the `MessageType` and the same `input-queue` exchange.

The properties of the exchange binding may also be configured:

```csharp
cfg.ReceiveEndpoint("input-queue", e =>
{
    e.Bind("exchange-name", x =>
    {
        x.Durable = false;
        x.AutoDelete = true;
        x.ExchangeType = "direct";
        x.RoutingKey = "8675309";
    });
})
```

The above will create an exchange binding between the `exchange-name` and the `input-queue` exchange, using the configured properties.

## RoutingKey

The routing key on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```csharp
public interface SubmitOrder
{
    string CustomerType { get; }
    Guid TransactionId { get; }
    // ...
}

Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Send<SubmitOrder>(x =>
    {
        // use customerType for the routing key
        x.UseRoutingKeyFormatter(context => context.Message.CustomerType);

        // multiple conventions can be set, in this case also CorrelationId
        x.UseCorrelationId(context => context.Message.TransactionId);
    });
    //Keeping in mind that the default exchange config for your published type will be the full typename of your message
    //we explicitly specify which exchange the message will be published to. So it lines up with the exchange we are binding our
    //consumers too.
    cfg.Message<SubmitOrder>(x => x.SetEntityName("submitorder"));
    //Also if your publishing your message: because publishing a message will, by default, send it to a fanout queue. 
    //We specify that we are sending it to a direct queue instead. In order for the routingkeys to take effect.
    cfg.Publish<SubmitOrder>(x => x.ExchangeType = ExchangeType.Direct);
});
```

The consumer could then be created:

```csharp
public class OrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {

    }
}
```

And then connected to a receive endpoint:

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("priority-orders", x =>
    {
        x.ConfigureConsumeTopology = false;

        x.Consumer<OrderConsumer>();

        x.Bind("submitorder", s => 
        {
            s.RoutingKey = "PRIORITY";
            s.ExchangeType = ExchangeType.Direct;
        });
    });

    cfg.ReceiveEndpoint("regular-orders", x =>
    {
        x.ConfigureConsumeTopology = false;

        x.Consumer<OrderConsumer>();

        x.Bind("submitorder", s => 
        {
            s.RoutingKey = "REGULAR";
            s.ExchangeType = ExchangeType.Direct;
        });
    });
});
```

This would split the messages sent to the exchange, by routing key, to the proper endpoint, using the CustomerType property.

## Addressing

Query string parameters supported:

### RabbitMQ Query Parameters

| Parameter        | Type  | Description                | Implies |
| -------------    |-------|----------                  |---------|
| temporary        | bool  | Temporary endpoint         | durable = false, autodelete = true
| durable          | bool  | Save messages to disk      |
| autodelete       | bool  | Delete when bus is stopped |
| bind             | bool  | Bind exchange to queue     |
| queue            | string| Bind to queue name         | bind = true


## Broker Topology

In this example topology, two commands and events are used.

First, the event contracts that are supported by an endpoint that receives files from a customer.

```csharp
interface FileReceived
{
    Guid FileId { get; }
    DateTime Timestamp { get; }
    Uri Location { get; }
}

interface CustomerDataReceived
{
    DateTime Timestamp { get; }
    string CustomerId { get; }
    string SourceAddress { get; }
    Uri Location { get; }
}
```

Second, the command contract for processing a file that was received.

```csharp
interface ProcessFile
{
    Guid FileId { get; }
    Uri Location { get; }
}
```

The above contracts are used by the consumers to receive messages. From a publishing or sending perspective, two classes are created by the event producer and the command sender which implement these interfaces.

```csharp
class FileReceivedEvent :
    FileReceived,
    CustomerDataReceived
{
    public Guid FileId { get; set; }
    public DateTime Timestamp { get; set; }
    public Uri Location { get; set; }
    public string CustomerId { get; set; }
    public string SourceAddress { get; set; }
}
```

And the command class.

```csharp
class ProcessFileCommand :
    ProcessFile
{
    public Guid FileId { get; set; }   
    public Uri Location { get; set; }
}
```

The consumers for these message contracts are as below.

```csharp
class FileReceivedConsumer :
    IConsumer<FileReceived>
{
}

class CustomerAuditConsumer :
    IConsumer<CustomerDataReceived>
{
}

class ProcessFileConsumer :
    IConsumer<ProcessFile>
{
}
```

### Publish

The exchanges and queues configures for the event example are as shown below.

> MassTransit publishes messages to the message type exchange, and copies are routed to all the subscribers by RabbitMQ. This approach was [based on an article][2] on how to maximize routing performance in RabbitMQ.

[2]: http://spring.io/blog/2011/04/01/routing-topologies-for-performance-and-scalability-with-rabbitmq/

![rabbitmq-publish-topology](/rabbitmq-publish-topology.png)

### Send

The exchanges and queues for the send example are shown.

![rabbitmq-send-topology](/rabbitmq-send-topology.png)

> Note that the broker topology can now be configured using the [topology](../topology/README.md) API.

