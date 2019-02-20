# Correlating messages

In a distributed message-based system, message correlation is very important. Since operations are potentially executing across hundreds of nodes, the ability to correlate different messages to build a path through the system is absolutely necessary for engineers to troubleshoot problems.

The headers on the message envelope provided by MassTransit already make it easy to specify correlation values. In fact, most are setup by default if not specified by the developer.

MassTransit provides the interface `CorrelatedBy<T>`, which can be used to setup a default correlationId. This is used by sagas as well, since all sagas have a unique `CorrelationId` for each instance of the saga. If a message implements `CorrelatedBy<Guid>`, it will automatically be directed to the saga instance with the matching identifier. If a new saga instance is created by the event, it will be assigned the `CorrelationId` from the initiating message.

For message types that have a correlation identifier, but are not using the `CorrelatedBy` interface, it is possible to declare the identifier for the message type and MassTransit will use that identifier by default for correlation.

```csharp
MessageCorrelation.UseCorrelationId<YourMessageClass>(x => x.SomeGuidValue);
```

<div class="alert alert-info">
<b>Note:</b>
    This should be called before you start the bus. We currently recommend that you put all of these in a static method for easy grouping and then call it at the beginning of the MassTransit configuration block.
</div>

Most transactions in a system will end up being logged and wide scale correlation is likely. Therefore, the use of consistent correlation identifiers is recommended. In fact, using a `Guid` type is highly recommended. MassTransit uses the [NewId](https://www.nuget.org/packages/NewId) library to generate identifiers that are unique and sequential that are represented as a `Guid`. The identifiers are clustered-index friendly, being ordered in a way that SQL Server can efficiently insert them into a database with the *uniqueidentifier* as the primary key. Just use `NewId.NextGuid()` to generate an identifier -- it's fast, fun, and all your friends are doing it.

<div class="alert alert-info">
<b>Note:</b>
    So, what does correlated actually mean? In short it means that this message is a part of a larger conversation. For instance, you may have a message that says <i>New Order (Item:Hammers; Qty:22; OrderNumber:45)</i> and there may be another message that is a response to that message that says <i>Order Allocated(OrderNumber:45)</i>. In this case, the order number is acting as your correlation identifier, it ties the messages together.
</div>

### Correlation by convention

In addition to the explicit `CorrelateBy<T>` interface, a convention-based correlation is supported. If the message contract has a property named ``CorrelationId``, ``CommandId``, or ``EventId``, the correlationId header is automatically populated on Send or Publish. It can also be manually specified using the ``SendContext``.

Bear in mind that sagas default `CorrelateById()` only support messages where the explicit `CorrelateBy<Guid>` interface is implemented. However, the header is still useful if you do not use sagas, for example for message flow analysis and debugging.

## Tracing conversations

There are several other built-in message headers that can be used to correlate messages. However, it is also completely acceptable to add your own custom properties to the message contract for correlation.

In addition to the correlationId, the default headers include:

#### RequestId
  When using the `RequestClient`, or the request/response message handling of MassTransit, each request is assigned a unique `RequestId`. When the message is received by a consumer, the response message sent by the `Respond` method (on the `ConsumeContext`) is assigned the same `RequestId` so that it can be correlated by the request client. This header should not typically be set by the consumer, as it is handled automatically.

#### ConversationId
  The conversation is created by the first message that is sent or published, in which no existing context is available (such as when a message is sent or published by using `IBus.Send` or `IBus.Publish`). If an existing context is used to send or publish a message, the `ConversationId` is copied to the new message, ensuring that a set of messages within the same *conversation* have the same identifier.

#### InitiatorId
  When a message is created within the context of an existing message, such as in a consumer, a saga, etc., the `CorrelationId` of the message (if available, otherwise the `MessageId` may be used) is copied to the `InitiatorId` header. This makes it possible to combine a chain of messages into a graph of producers and consumers.

#### MessageId
  When a message is sent or published, this header is automatically generated for the message.
