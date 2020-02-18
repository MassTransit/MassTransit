# Batching

In some scenarios, high message volume can lead to consumer resource bottlenecks. If a system is publishing thousands of messages per second, and has a consumer that is writing the content of those messages to some type of storage, the storage system might not be optimized for thousands of individual writes per second. It may, however, perform better if writes are performed in batches. For example, receiving one hundred messages and then writing the content of those messages using a single storage operation may be significantly more efficient (and faster).

MassTransit has basic support for receiving messages and delivering those messages to a consumer as a `Batch`.

To create a batch consumer, consume the `Batch<T>` interface, where `T` is the message type.

```cs
class LogBatchConsumer :
    IConsumer<Batch<LogMessage>>
{
    public async Task Consume(ConsumeContext<Batch<LogMessage>> context)
    {
        StringBuilder builder = new StringBuilder();

        for(int i = 0; i < context.Message.Length; i++)
        {
            builder.Append(context.Message[i].Text);
        }

        // do something with the string, like write it to a file
    }
}
```

Once the consumer has been created, configure the consumer on a receive endpoint.

```cs
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");

    cfg.ReceiveEndpoint("log-queue", e =>
    {
        // the transport must be configured to deliver at least the batch message limit
        e.PrefetchCount = 200;

        e.Batch<LogMessage>(b =>
        {
            // allow up to 100 messages in a batch
            b.MessageLimit = 100;

            // end the batch early if at least one message has been received and the 
            // time limit is reached.
            b.TimeLimit = TimeSpan.FromSeconds(1);

            b.Consumer(() => new LogBatchConsumer());
        })
    });
});
```

::: tip Important
Each transport has its own limitations. For instance, Amazon SQS only delivers up to ten messages at once – so batch sizes above ten would always time out rather than reaching the batch limit.
:::

For instance, when using Azure Service Bus, there are two settings which must be configured as shown below.

```cs
var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.ReceiveEndpoint("log-queue", e =>
    {
        e.PrefetchCount = 20;
        e.MaxConcurrentCalls = 20;

        e.Batch<LogMessage>(b =>
        {
            b.MessageLimit = 20;
            b.TimeLimit = TimeSpan.FromSeconds(5);

            b.Consumer(() => new LogBatchConsumer());
        })
    });
});
```

The `Batch` interface also includes the timestamp of the first message, the timestamp of the last message, and the completion mode of the batch (message limit or time limit was reached).

::: tip NOTE
The batch support provided is basic, there isn't any support for advanced concepts like acknowledging individual messages, faulting individual messages, etc. Sharp scissors, use with caution.
:::

To configure the consumer using a container, extension methods have been added for the fully-supported containers.

```cs
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");

    cfg.ReceiveEndpoint("log-queue", e =>
    {
        // the transport must be configured to deliver at least the batch message limit
        e.PrefetchCount = 200;

        e.Batch<LogMessage>(b =>
        {
            b.MessageLimit = 100;
            b.TimeLimit = TimeSpan.FromSeconds(1);

            b.Consumer<LogBatchConsumer, LogMessage>(container); // provider, context, etc.
        })
    });
});
```

