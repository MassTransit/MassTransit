# Handling faults

MassTransit delivers messages to consumers by calling the _Consume_ method. When a message consumer throws an exception instead of returning normally, a `Fault<T>` is produced, which may be published or sent depending upon the context.

A `Fault<T>` is a generic message contract including the original message that caused the consumer to fail, as well as the `ExceptionInfo`, `HostInfo`, and the time of the exception.

```csharp
public interface Fault<T>
    where T : class
{
    Guid FaultId { get; }
    Guid? FaultedMessageId { get; }
    DateTime Timestamp { get; }
    ExceptionInfo[] Exceptions { get; }
    HostInfo Host { get; }
    T Message { get; }
}
```

If the message headers specify a `FaultAddress`, the fault is sent directly to that address. If the _FaultAddress_ is not present, but a `ResponseAddress` is specified, the fault is sent to the response address. Otherwise, the fault is published, allowing any subscribed consumers to receive it.

# Consuming faults

Developers may want to do something with faults, such as updating an operational dashboard. To observe faults separate of the consumer that caused the fault to be produced, a consumer can consume fault messages the same as any other message.

```csharp
public class DashboardFaultConsumer :
    IConsumer<Fault<SubmitOrder>>
{
    public async Task Consume(ConsumeContext<Fault<SubmitOrder>> context)
    {
        // update the dashboard
    }
}
```

Faults can also be observed by state machines when specified as an event:

```csharp
Event(() => SubmitOrderFaulted, 
    x => x.CorrelateById(m => m.Message.Message.OrderId)
    .SelectId(m => m.Message.Message.OrderId));

public Event<Fault<SubmitOrder>> SubmitOrderFaulted { get; private set; }
```
