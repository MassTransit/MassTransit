# Handling exceptions

Let's face it, bad things happen. Networks partition, data servers crash, remote endpoints get busy and fail to respond. And when bad things happen, exceptions get thrown. And when exceptions get thrown, people die. Okay, maybe that's a bit dramatic, but the point is, exceptions are a fact of software development.

Fortunately, MassTransit provides a number of features to help your application recover from and deal with exceptions. But before getting into that, an understanding of what happens when a message is consumed is needed.

Take, for example, a consumer that simply throws an exception.

```csharp
public class UpdateCustomerAddressConsumer :
    IConsumer<UpdateCustomerAddress>
{
    public Task Consume(ConsumeContext<UpdateCustomerAddress> context)
    {
        throw new Exception("Very bad things happened");
    }
}
```

When a message is delivered to the consumer, the consumer throws an exception. With a default bus configuration, the exception is caught by middleware in the transport (the `MoveExceptionToTransportFilter` to be exact), and the message is moved to an *_error* queue (prefixed by the receive endpoint queue name). The exception details are stored as headers with the message, for analysis and to assist in troubleshooting the exception.

In addition to moving the message to an error queue, MassTransit also generates a `Fault<T>` event. If the received message specified a `FaultAddress` header, the fault is sent to that address. If a fault address is not found, and a `ResponseAddress` is present, the fault is sent to the response address. If neither address is present, the fault is published.

## Retrying messages

In some cases, the exception may be a transient condition, such as a database deadlock, a busy web service, or some similar type of situation which usually clears up on a second attempt. With these exception types, it is often desirable to retry the message delivery to the consumer, allowing the consumer to try the operation again.

```csharp
public class UpdateCustomerAddressConsumer :
    IConsumer<UpdateCustomerAddress>
{
    ISessionFactory _sessionFactory;

    public async Task Consume(ConsumeContext<UpdateCustomerAddress> context)
    {
        using(var session = _sessionFactory.OpenSession())
        using(var transaction = session.BeginTransaction())
        {
            var customer = session.Get<Customer>(context.Message.CustomerId);
            // update customer address properties from message

            session.Update(customer);

            transaction.Commit();
        }
    }
}
```

With this consumer, an `ADOException` can be thrown if the update fails. In this case, the operation should be retried before moving the message to the error queue. This can be configured on the receive endpoint. Shown below is a retry policy which attempts to deliver the message to a consumer five times before throwing the exception back up the pipeline.

```csharp
var sessionFactory = CreateSessionFactory();

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
    {
        e.UseRetry(r => r.Immediate(5));
        e.Consumer(() => new UpdateCustomerAddressConsumer(sessionFactory));
    });
});
```

The `UseRetry` method is an extension method that configures a middleware filter, in this case the `RetryFilter`. There are a variety of retry policies available, which are detailed in the [reference section](retries.md).

<div class="alert alert-info">
<b>Note:</b>
    In this example, the <i>UseRetry</i> is at the receive endpoint level. Additional retry filters can be added at the bus and consumer level, providing flexibility in how different consumers, messages, etc. are retried.
</div>

## Consuming Faults

After all of the various retry policies have executed, the bus will generate a fault which you can consume. Below is a simple example of consuming a fault thrown by the consumer above.

```csharp
public class UpdateCustomerAddressFaultConsumer :
    IConsumer<Fault<UpdateCustomerAddress>>
{

    public async Task Consume(ConsumeContext<Fault<UpdateCustomerAddress>> context)
    {
        var originalMessage = context.Message.Message;
        var exceptions = context.Message.Exceptions;

        //Do something interesting.
    }
}
```
