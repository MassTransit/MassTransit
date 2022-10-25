# Azure Service Bus

The send and publish topologies are extended to support the Azure Service Bus features, and make it possible to configure how topics are created.

## Topics

To specify properties used when a topic is created, the publish topology can be configured during bus creation:

```csharp
Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.Publish<OrderSubmitted>(x =>
    {
        x.EnablePartitioning = true;
    });
});
```

## PartitionKey

The PartitionKey on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```csharp
public record SubmitOrder
{
    public string CustomerId { get; init; }
    public Guid TransactionId { get; init; }
}

Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.Send<SubmitOrder>(x =>
    {
        x.UsePartitionKeyFormatter(context => context.Message.CustomerId);
    });
});
```

## SessionId

The SessionId on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```csharp
public record UpdateUserStatus
{
    public Guid UserId { get; init; }
    public string Status { get; init; }
}

Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.Send<UpdateUserStatus>(x =>
    {
        x.UseSessionIdFormatter(context => context.Message.UserId);
    });
});
```

## Subscriptions

In Azure, topics and topic subscriptions provide a mechanism for one-to-many communication (versus queues that are designed for one-to-one). A topic subscription acts as a virtual queue. To subscribe to a topic subscription directly the `SubscriptionEndpoint` should be used:

```csharp
cfg.SubscriptionEndpoint<MessageType>("subscription-name", e =>
{
    e.ConfigureConsumer<MyConsumer>(provider);
})
```

Note that a topic subscription's messages can be forwarded to a receive endpoint (an Azure Service Bus queue), in the following way. Behind the scenes MassTransit is setting up [Service Bus Auto-forwarding](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-auto-forwarding) between a topic subscription and a queue.

```csharp
cfg.ReceiveEndpoint("input-queue", e =>
{
    e.Subscribe("topic-name");
    e.Subscribe<MessageType>();
})
```

The properties of the topic subscription may also be configured:

```csharp
cfg.ReceiveEndpoint("input-queue", e =>
{
    e.Subscribe("topic-name", x =>
    {
        x.AutoDeleteOnIdle = TimeSpan.FromMinutes(60);
    });
})
```

### Broker Topology

The topics, queues, and subscriptions configured on Azure Service Bus are shown below.

![azure-topology](/azure-topology.png)

