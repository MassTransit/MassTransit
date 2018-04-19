# RabbitMQ Exchange Binding

To bind an exchange to a receive endpoint:

```csharp
cfg.ReceiveEndpoint(host, "input-queue", e =>
{
    e.Bind("exchange-name");
    e.Bind<MessageType>();
})
```

The above will create two exchange bindings, one between the `exchange-name` exchange adn the `input-queue` exchange and a second between the exchange name matching the `MessageType` and the same `input-queue` exchange.

The properties of the exchange binding may also be configured:

```csharp
cfg.ReceiveEndpoint(host, "input-queue", e =>
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