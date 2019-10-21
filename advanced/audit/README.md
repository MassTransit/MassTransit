# Message audit

Due to asynchronous nature of messaging, it is not always easy to find out the message flow.
Step-into end-to-end debugging is almost impossible to use, especially if message processing
is done in parallel and consumers perform atomic operations.

To enable better diagnostic and troubleshooting, the audit log, which contains all messages
that have been sent and consumed, would provide a great help.

Also, certain systems require keeping the full log of operations, and if all operations are
done using messages, storing these messages will satisfy such requirement.

MassTransit supports message audit by using special observers.

## Principles

Two main parts need to be saved for each message to provide complete audit:
* The message itself
* Metadata

Message metadata includes:
* Message id
* Message type
* Context type (Send, Publish or Consume)
* Conversation id
* Correlation id
* Initiator id
* Request id (for request/response)
* Source address
* Destination address
* Response address (for request/response)
* Fault address

The audit feature is generic and requires an implementation for the `IMessageAuditStore` interface.
This interface is very simple and has one method only:
```csharp
Task StoreMessage<T>(<T> message, MessageAuditMetadata metadata);
```

Some audit store implementations are included out of the box and described below.

There are three observers that connect to the message pipeline and record them.
Two are for messages being sent - send and publish observers; one is for messages that
are being consumed.

Consume observer is invoked before a message is consumed, so the message is stored to the audit
store even if a consumer fails. Send and publish observers are invoked after the message
has actually been sent.

## Installation

There are two extensions methods for `IBusControl` that enable sent and consumed messages audit.
Configuring both looks like this:

```csharp
var busControl = ConfigureBus(); // all usual configuration
busControl.ConnectSendAuditObservers(auditStore);
busControl.ConnectConsumeAuditObserver(auditStore);
```

There, the `auditStore` is the audit persistent store. Currently available stores include:
* [Entity Framework](ef.md)

Please remember that observers need to be configured before the bus starts.

## Filters

Sometimes there are messages in the system that are technical. These could be some monitoring and
healthcheck messages, which are being sent often and could pollute the audit log. Another example
could be the `Fault<T>` events.

In order not to save these messages to the audit store, you can filter them out. You can configure
the observers to use message filters like this:

```csharp
busControl.ConnectSendAuditObservers(auditStore, 
    c => c.Ignore<HealthCheck>());
busControl.ConnectConsumeAuditObserver(auditStore, 
    c => c.Ignore(typeof(ServicePoll), typeof(RubbishEvent)));
```

## Metadata factory

By default, the audit logging feature uses its own, quite complete, metadata collection mechanism.
However, you can implement your own metadata factories to collect more data or different data.

There are two types of metadata factories:
* `DefaultConsumeMetadataFactory` that gets the `ConsumeObserver`
* `DefaultSendMetadataFactory` that gets the `SendObserver` (which is used for both send and publish)

Factories are simple classes that implement `IConsumeMetadataFactory` or `ISendMetadataFactory`
interfaces and return a new `MessageAuditMetadata` object from a given context.
For example, the default consume audit metadata factory implementation looks like this:

```csharp
return new MessageAuditMetadata
{
    ContextType = contextType,
    ConversationId = context.ConversationId,
    CorrelationId = context.CorrelationId,
    InitiatorId = context.InitiatorId,
    MessageId = context.MessageId,
    RequestId = context.RequestId,
    DestinationAddress = context.DestinationAddress?.AbsoluteUri,
    SourceAddress = context.SourceAddress?.AbsoluteUri,
    FaultAddress = context.FaultAddress?.AbsoluteUri,
    ResponseAddress = context.ResponseAddress?.AbsoluteUri,
    Headers = context.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
};
```

To use your own factory, you can use the third parameter when configuring the observers:

```csharp
busControl.ConnectSendAuditObservers(auditStore, filter, new MySendContextMetadataFactory());
busControl.ConnectConsumeAuditObserver(auditStore, filter, new MySendContextMetadataFactory());
```
