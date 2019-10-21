# RabbitMQ Routing Key Convention

The routing key on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```csharp
public interface SubmitOrder
{
    string CustomerType { get; }
    Guid TransactionId { get; }
    // ...
}

Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Send<SubmitOrder>(x =>
    {
        // use customerType for the routing key
        x.UseRoutingKeyFormatter(context => context.Message.CustomerType);

        // multiple conventions can be set, in this case also CorrelationId
        x.UseCorrelationId(context => context.Message.TransactionId);
    });
    //Keeping in mind that the default exchange config for your published type will be the full typename of your message
    //we explicitly specify which exchange the message will be published to. So it lines up with the exchange we are binding our
    //consumers too.
    cfg.Message<SubmitOrder>(x => x.SetEntityName("submitorder"));
    //Also if your publishing your message: because publishing a message will, by default, send it to a fanout queue. 
    //We specify that we are sending it to a direct queue instead. In order for the routingkeys to take effect.
    cfg.Publish<SubmitOrder>(x => x.ExchangeType = ExchangeType.Direct);
});
```

The consumer could then be created:

```csharp
public class OrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {

    }
}
```

And then connected to a receive endpoint:

```csharp
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.ReceiveEndpoint(host, "priority-orders", x =>
    {
        x.BindMessageExchanges = false;

        x.Consumer<OrderConsumer>();

        x.Bind("submitorder", s => 
        {
            s.RoutingKey = "PRIORITY";
            s.ExchangeType = ExchangeType.Direct;
        });
    });

    cfg.ReceiveEndpoint(host, "regular-orders", x =>
    {
        x.BindMessageExchanges = false;

        x.Consumer<OrderConsumer>();

        x.Bind("submitorder", s => 
        {
            s.RoutingKey = "REGULAR";
            s.ExchangeType = ExchangeType.Direct;
        });
    });
});
```

This would split the messages sent to the exchange, by routing key, to the proper endpoint, using the CustomerType property.
