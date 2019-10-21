# Connecting endpoints

Once the bus is started, additional endpoints may be connected. An additional endpoint may be needed for a temporary process, or to allow an application to receive responses on a dedicated queue versus using the existing bus receive endpoint.

## Connecting an endpoint

To connect a new receive endpoint to a RabbitMQ host, the host is used.

```csharp
IRabbitMqHost host = ...; // the host configured on the bus

var handle = await host.ConnectReceiveEndpoint("secondary_queue", x =>
{
    x.Consumer<MyConsumer>(...);
    x.Handler<MyMessage>(...);
});
```

Consumers registered on the receive endpoint are configured the same as receive endpoints configured during bus creation, and will have their messages types subscribed to the receive endpoint's queue. And since receive endpoint is configured using the host, all of the transport features available on the host are available for the receive endpoint (such as setting a prefetch count, or specifying message queue properties).

To connect a new receive endpoint to an Azure Service Bus host, the same syntax is used but with the `IServiceBusHost` reference.

## Disconnecting an endpoint

When an endpoint has been connected to an existing bus, it should be shut down before shutting down the bus. The handle returned by the `ConnectReceiveEndpoint` call should be used to `Stop` the receive endpoint.

```csharp
await handle.StopAsync();
```