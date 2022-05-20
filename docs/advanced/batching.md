# Batching

In some scenarios, high message volume can lead to consumer resource bottlenecks. If a system is publishing thousands of messages per second, and has a consumer that is writing the content of those messages to some type of storage, the storage system might not be optimized for thousands of individual writes per second. It may, however, perform better if writes are performed in batches. For example, receiving one hundred messages and then writing the content of those messages using a single storage operation may be significantly more efficient (and faster).

MassTransit supports receiving messages and delivering those messages to a consumer as a `Batch`.

To create a batch consumer, consume the `Batch<T>` interface, where `T` is the message type. That consumer can then be configured using the container integration, with the batch options specified in a consumer definition. The example below consumes a batch of _OrderAudit_ events, up to 100 at a time, and up to 10 concurrent batches.

<<< @/docs/code/advanced/BatchingConsumer.cs

Once the consumer has been created, configure the consumer on a receive endpoint (in this case, using the default convention).

<<< @/docs/code/advanced/BatchingConsumerBus.cs

If automatic receive endpoint configuration is not used, the receive endpoint can be configured explicitly.

<<< @/docs/code/advanced/BatchingConsumerExplicit.cs

::: warning PrefetchCount
Every transport has its own limitations that may constrain the batch size. For instance, Amazon SQS fetches ten messages at a time, making it an optimal batch size. It is best to experiment and see what works best in your environment.

If the _PrefetchCount_ is lower than the batch limit, performance will be limited by the time limit as the batch size will never be reached.
:::

For instance, when using Azure Service Bus, there are two settings which must be configured as shown below.

<<< @/docs/code/advanced/BatchingConsumerAzure.cs

### Batch Interface

The `Batch` interface, shown below, also includes the first message receipt time, the last message receipt time, and the completion mode of the batch (message limit or time limit was reached).

<<< @/src/MassTransit.Abstractions/Contracts/Batch.cs
