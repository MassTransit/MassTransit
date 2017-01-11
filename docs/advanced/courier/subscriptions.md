# Subscriptions

By default, routing slip events are published -- which means that any subscribed consumers will receive the events. 
While this is useful getting started, it can quickly get out of control as applications grow and multiple unrelated 
routing slips are used. To handle this, subscriptions were added (yes, added, because they weren't though of until 
we experienced this ourselves).

Subscriptions are added to the routing slip at the time it is built using the `RoutingSlipBuilder`.

```csharp
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), 
    RoutingSlipEvents.All);
```

This subscription would send all routing slip events to the specified endpoint. If the application only wanted 
specified events, the events can be selected by specifying the enumeration values for those events. For example, 
to only get the `RoutingSlipCompleted` and `RoutingSlipFaulted` events, the following code would be used.

```csharp
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), 
    RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);
```

It is also possible to tweak the content of the events to cut down on message size. For instance, 
by default, the `RoutingSlipCompleted` event includes the variables from the routing slip. If the variables 
contained a large document, that document would be copied to the event. Eliminating the variables from the 
event would reduce the message size, thereby reducing the traffic on the message broker. To specify the contents 
of a routing slip event subscription, an additional argument is specified.

```csharp
builder.AddSubscription(new Uri("rabbitmq://localhost/log-events"), 
    RoutingSlipEvents.Completed, RoutingSlipEventContents.None);
```

This would send the `RoutingSlipCompleted` event to the endpoint, without any of the variables be included 
(only the main properties of the event would be present).

> Once a subscription is added to a routing slip, events are no longer published -- they are only sent to 
> the addresses specified in the subscriptions. However, multiple subscriptions can be specified -- the endpoints 
> just need to be known at the time the routing slip is built.

## Custom events

It is also possible to specify a subscription with a custom event, a message that is created by the 
application developer. This makes it possible to create your own event types and publish them in response 
to routing slip events occurring. And this includes having the full context of a regular endpoint `Send` 
so that any headers or context settings can be applied.

To create a custom event subscription, use the overload shown below.

```csharp
// first, define the event type in your assembly
public interface OrderProcessingCompleted
{
    Guid TrackingNumber { get; }
    DateTime Timestamp { get; }

    string OrderId { get; }
    string OrderApproval { get; }
}

// then, add the subscription with the custom properties
builder.AddSubscription(new Uri("rabbitmq://localhost/order-events"), 
    RoutingSlipEvents.Completed, 
    x => x.Send<OrderProcessingCompleted>(new
    {
        OrderId = "BFG-9000",
        OrderApproval = "ComeGetSome"
    }));
```

In the message contract above, there are four properties, but only two of them are specified. 
By default, the base `RoutingSlipCompleted` event is created, and then the content of that event is *merged* 
into the message created in the subscription. This ensures that the dynamic values, such as the `TrackingNumber` 
and the `Timestamp`, which are present in the default event, are available in the custom event.

Custom events can also select with contents are merged with the custom event, using an additional method overload.
