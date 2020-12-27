# Connect Endpoint

Receive endpoints should be configured with the bus, so that they are all started at the same time. In some situations, however, a receive endpoint may be needed after the bus has been started. For instance, a request client may want to use a separate queue from responses instead of using the bus's queue. Or it may be that a new consumer is now available, and needs to be connected to the bus.

To connect a new receive endpoint to the bus, use the `ConnectReceiveEndpoint` method.

```cs
var handle = bus.ConnectReceiveEndpoint("secondary-queue", x =>
{
    x.Consumer<SomeConsumer>();
})

// wait for the receive endpoint to be ready, throws an exception if a fault occurs
var ready = await handle.Ready;
```

When connecting a receive endpoint, consumers will configure the broker topology so that messages types are subscribed to topics/exchanges. If the receive endpoint is temporary, the configuration should be skipped, and the `ConnectConsumer` style methods should be used after the receive endpoint is ready.

### Disconnect an Endpoint

Connected endpoints will be stopped when the bus is stopped. If the endpoint needs to be stopped before the bus, the handle can be used.

```csharp
await handle.StopAsync();
```

### Container Integration

When using `AddMassTransit` with your dependency injection container of choice, receive endpoints can be connected using the `IReceiveEndpointConnector` interface. When using this interface, consumers which were added during configuration can be configured on the receive endpoint.

To connect a receive endpoint and configure a consumer:

<<< @/docs/code/containers/MicrosoftConnect.cs
