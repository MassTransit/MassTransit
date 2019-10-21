# Azure Service Bus Topology

The send and publish topologies are extended to support the Azure Service Bus features, and make it possible to configure how topics are created.


## Topic Properties

To specify properties used when a topic is created, the publish topology can be configured during bus creation:

```csharp
Bus.Factory.CreateUsingAzureServiceBus(..., cfg =>
{
    cfg.Publish<OrderSubmitted>(x =>
    {
        x.EnablePartitioning = true;
    });
});
```