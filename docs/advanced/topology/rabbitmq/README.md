# RabbitMQ Topology

The send and publish topologies are extended to support RabbitMQ features, and make it possible to configure how exchanged are created.

## Configuring Exchange Properties

When a message is published, MassTransit sends it to an exchange that is named based upon the message type. Using topology, the exchange name, as well as the exchange properties can be configured to support a custom behavior.

To configure the properties used when an exchange is created, the publish topology can be configured during bus creation:

```csharp
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Publish<OrderSubmitted>(x =>
    {
        x.Durable = false;
    });
});
```

## Hierarchical Exchange Layout

In versions of MassTransit prior to 4.x, every implemented type was connected directly to the top-level exchange for the published message type. Starting with v4.0, the broker topology for inherited types can be configured to maintain the type hierarchy, which can significantly reduce the number of exchange bindings in some cases. To configure this new behavior, the publish topology is used to specify the broker topology option.

```csharp
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.PublishTopology(x =>
    {
        x.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;
    });
});
```
