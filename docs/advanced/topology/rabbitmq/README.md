# RabbitMQ Topology

The send and publish topologies are extended to support RabbitMQ features, and make it possible to configure how exchanged are created.

## Exchange Properties

To specify properties used when an exchange is created, the publish topology can be configured during bus creation:

```
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Publish<OrderSubmitted>(x =>
    {
        x.Durable = false;
    });
});
```