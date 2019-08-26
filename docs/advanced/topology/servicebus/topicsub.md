# Topic Subscriptions

In Azure, topics and topic subscriptions provide a mechanism for one-to-many communication (versus queues that are designed for one-to-one). A topic subscription acts as a virtual queue. To subscribe to a topic subscription directly the `SubscriptionEndpoint` should be used:

```csharp
cfg.SubscriptionEndpoint<MessageType>(host, "subscription-name", e =>
{
    e.ConfigureConsumer<MyConsumer>(provider);
})
```

Note that a topic subscription's messages can be forwarded to a receive endpoint (an Azure Service Bus queue), in the following way. Behind the scenes MassTransit is setting up [Service Bus Autoforwarding](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-auto-forwarding) between a topic subscription and a queue.

```csharp
cfg.ReceiveEndpoint(host, "input-queue", e =>
{
    e.Subscribe("topic-name");
    e.Subscribe<MessageType>();
})
```

The properties of the topic subscription may also be configured:

```csharp
cfg.ReceiveEndpoint(host, "input-queue", e =>
{
    e.Subscribe("topic-name", x =>
    {
        x.AutoDeleteOnIdle = TimeSpan.FromMinutes(60);
    });
})
```
