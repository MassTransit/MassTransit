# RabbitMQ Routing Key Convention

The routing key on published/sent messages can be configured by convention, allowing the same method to be used for messages which implement a common interface type. If no common type is shared, each message type may be configured individually using various conventional selectors. Alternatively, developers may create their own convention to fit their needs.

When configuring a bus, the send topology can be used to specify a routing key formatter for a particular message type.

```
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
});
```

The consumer could then be created:

```
public class OrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {

    }
}
```

And then connected to a receive endpoint:

```
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.ReceiveEndpoint(host, "priority-orders", x)
    {
        x.BindMessageExchanges = false;
        
        x.Consumer<OrderConsumer>();

        x.Bind("submitorder", s => 
        {
            s.RoutingKey = "PRIORITY";
            s.ExchangeType = ExchangeType.Direct;
        });
    });

    cfg.ReceiveEndpoint(host, "regular-orders", x)
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