# Topology Conventions

Conventions are used to apply topology to messages without requiring explicit configuration of every message type.

A basic example of a convention is the default `CorrelationId` convention, which is automatically applied to all sent messages. As message types are sent, the convention is used to determine if the message contains a property that could be considered a CorrelationId, and uses that property to set the `CorrelationId` header on the message envelope.

For example, the following message contains a property named `CorrelationId`, which is an obvious choice. Note that the `CorrelatedBy<Guid>` interface is not part of the message contract.

```csharp
public interface OrderCreated
{
    Guid CorrelationId { get; }
}
```

If there isn't a property named `CorrelationId`, the convention also checks for `CommandId` and `EventId` and uses that property to set the header value (the type must be a Guid, or a Guid?, no magic type conversion happening here). 

If the message implements the `CorrelatedBy<Guid>` interface, that would be used before referencing any properties by name.

During bus creation, it is possible to explicitly configure a message type (or any of the message type's inherited interfaces) to use a specific property for the `CorrelationId`. In the example below, the OrderId property is specified as the CorrelationId.

```csharp
public interface OrderSubmitted
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
}

Bus.Factory.CreateUsingRabbitMq(..., cfg =>
{
    cfg.Send<OrderSubmitted>(x =>
    {
        x.UseCorrelationId(context => context.Message.OrderId);
    });
});
```

The CorrelationId topology convention is [implemented here](https://github.com/MassTransit/MassTransit/tree/develop/src/MassTransit/Topology/Conventions/CorrelationId), which can be used as an example of how to create your own conventions, or add additional CorrelationId detectors to the existing convention.

> Send topologies are applied to all outbound messages, regardless of whether they are _sent_ or _published_.
