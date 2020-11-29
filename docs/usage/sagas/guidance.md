# Saga Guidance

What follows is a set of guidelines related to sagas that was collected from Discord, Stack Overflow, and other sources to provide an easy way to link answers to commonly asked questions.


### Concurrency

Saga concurrency issues happen, particularly when using optimistic concurrency. The most common reasons include:

- Simultaneous events correlating to the same instance, typically from multiple sources running in parallel
- Commands from the saga to consumers, where the consumer is quick and responds before the saga has finished processing the initiating event

There are certainly others, but anytime multiple events correlate to the same instance, concurrency issues are a concern. For that reason, the following baseline receive endpoint configuration is recommended as a starting point (tuning will depend upon the saga, repository, environment, etc.).

To configure the receive endpoint directly:

```cs
services.AddMassTransit(x =>
{
    x.AddStateMachineSaga<OrderStateMachine, OrderState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://127.0.0.1";
            r.DatabaseName = "orderdb";
        });

    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.ReceiveEndpoint("saga-queue", e =>
        {
            const int ConcurrencyLimit = 20; // this can go up, depending upon the database capacity

            e.PrefetchCount = ConcurrencyLimit;

            e.UseMessageRetry(r => r.Interval(5, 1000));
            e.UseInMemoryOutbox();

            e.ConfigureSaga<OrderState>(context, s =>
            {
                var partition = endpointConfigurator.CreatePartitioner(ConcurrencyLimit);

                s.Message<SubmitOrder>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
                s.Message<OrderAccepted>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
                s.Message<OrderCanceled>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
            });
        });
    }
});
```

This example uses message retry (because concurrency issues throw exceptions), the _InMemoryOutbox_ (to avoid duplicate messages in the event of a concurrency failure), and uses a partitioner to limit the receive endpoint to only one concurrent message for each OrderId (the partitioner uses hashing to meet the partition count). 

> The partitioner in this case is only for this specific receive endpoint, multiple service instances (competing consumer) may still consume events for the same saga instance.