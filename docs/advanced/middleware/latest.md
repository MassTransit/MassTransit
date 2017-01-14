# Using the latest filter

The latest filter is pretty simple, it keeps track of the latest message received by the pipeline and makes that
value available. It seems pretty simple, and it is, but it is actually useful in metrics and monitoring scenarios.

> This filter is actually usable to capture any context type on any pipe, so you know.

To add a latest to a receive endpoint:

```csharp
ILatestFilter<ConsumeContext<Temperature>> tempFilter;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
    {
        e.Handler<Temperature>(context => Task.FromResult(true), x =>
        {
            x.UseLatest(x => x.Created = filter => tempFilter = filter);
        })
    });
```