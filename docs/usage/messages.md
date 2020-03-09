# Messages

In MassTransit, a message contract is defined _code first_ by creating a .NET type. A message can be defined using a class or an interface, resulting in a strongly-typed contract. Messages should be limited to read-only properties and not include methods or behavior.

::: warning Important
MassTransit uses the full type name, including the _namespace_, for message contracts. When creating the same message *type* in two separate projects, the namespaces **must** match or the message will not be consumed.
:::

An example message to update a customer address is shown below.

```csharp
	namespace Company.Application.Contracts
	{
		using System;

		public interface UpdateCustomerAddress
		{
			Guid CommandId { get; }
			DateTime Timestamp { get; }
			string CustomerId { get; }
			string HouseNumber { get; }
			string Street { get; }
			string City { get; }
			string State { get; }
			string PostalCode { get; }
		}
	}
```

::: tip
It is strongly suggested to use interfaces for message contracts, based on experience over several years with varying levels of developer experience. MassTransit will create dynamic interface implementations for the messages, ensuring a clean separation of the message contract from the consumer.
:::

A common mistake when engineers are new to messaging is to create a base class for messages, and try to dispatch that base class in the consumer – including the behavior of the subclass. Ouch. This always leads to pain and suffering, so just say no to base classes.

## Message Names

There are two main message types, _events_ and _commands_. When choosing a name for a message, the type of message should dictate the tense of the message.

### Commands

A command tells a service to do something. Commands are [sent](producers.md#send) (using `Send`) to an endpoint, as it is expected that a single service instance performs the command action. A command should never be published.

Commands should be expressed in a verb-noun sequence, following the _tell_ style.

Example Commands:

* UpdateCustomerAddress
* UpgradeCustomerAccount
* SubmitOrder

### Events

An event signifies that something has happened. Events are [published](producers.md#publish) (using `Publish`) via either `IBus` (standalone) or `ConsumeContext` (within a message consumer). An event should not be sent directly to an endpoint.

Events should be expressed in a noun-verb (past tense) sequence, indicating that something happened.

Example Events:

* CustomerAddressUpdated
* CustomerAccountUpgraded
* OrderSubmitted, OrderAccepted, OrderRejected, OrderShipped

## Message Headers

MassTransit encapsulates every sent or published message in a message envelope (described by the [Envelope Wrapper](https://www.enterpriseintegrationpatterns.com/patterns/messaging/EnvelopeWrapper.html) pattern). The envelope adds a series of message headers, including:

| Property | Type | Description |
| :---------------- |:------:| :--------------------------------------- |
| MessageId         |Auto   | Generated for each message using `NewId.NextGuid`.|
| CorrelationId     |User   | Assigned by the application, or automatically by convention, and should uniquely identify the operation, event, etc.|
| RequestId         |Request| Assigned by the request client, and automatically copied by the _Respond_ methods to correlate responses to the original request.|
| InitiatorId       |Auto   | Assigned when publishing or sending from a consumer, saga, or activity to the value of the _CorrelationId_ on the consumed message.|
| ConversationId    |Auto   | Assigned when the first message sent or published and no consumed message is available, ensuring that a set of messages within the same conversation have the save identifier.|
| SourceAddress     |Auto   | Where the message originated (may be a temporary address for messages published or sent from `IBus`).|
| DestinationAddress|Auto   | Where the message was sent |
| ResponseAddress   |Request| Where responses to the request should be sent. If not present, responses are _published_.|
| FaultAddress      |User   | Where consumer faults should be sent. If not present, faults are _published_.|
| ExpirationTime    |User   | When the message should expire, which may be used by the transport to remove the message if it isn't consumed by the expiration time.|
| SentTime          |Auto   | When the message was sent, in UTC.|
| MessageType       |Auto   | An array of message types, in a `MessageUrn` format, which can be deserialized.|
| Host              |Auto   | The host information of the machine that sent or published the message.|
| Headers           |User   | Additional headers, which can be added by the user, middleware, or diagnostic trace filters.|

Message headers can be read using the `ConsumeContext` interface and specified using the `SendContext` interface.

## Message Correlation

Since messages usually do not live in isolation, publishing one message usually leads to publishing another message, and then another, and so on. It is useful to trace such sequences, however, to find them these messages need to have some information detailing how they relate to each other.

Correlation is the principle of connecting messages together, usually by using a unique identifier that is included in every message that is part of a logical sequence. In MassTransit, the unique identifier is referred to as the `CorrelationId`, which is included in the message envelope and available via the `ConsumeContext` or the `SendContext`. MassTransit also includes a `ConversationId` which is the same across an entire set of related messages.

In a distributed message-based system, message correlation is very important. Since operations are potentially executing across hundreds of nodes, the ability to correlate different messages to build a path through the system is absolutely necessary for engineers to troubleshoot problems.

The headers on the message envelope provided by MassTransit already make it easy to specify correlation values. In fact, most are setup by default if not specified by the developer.

MassTransit provides the interface `CorrelatedBy<T>`, which can be used to setup a default correlationId. This is used by sagas as well, since all sagas have a unique `CorrelationId` for each instance of the saga. If a message implements `CorrelatedBy<Guid>`, it will automatically be directed to the saga instance with the matching identifier. If a new saga instance is created by the event, it will be assigned the `CorrelationId` from the initiating message.

For message types that have a correlation identifier, but are not using the `CorrelatedBy` interface, it is possible to declare the identifier for the message type and MassTransit will use that identifier by default for correlation.

```csharp
MessageCorrelation.UseCorrelationId<YourMessageClass>(x => x.SomeGuidValue);
```

::: tip
This should be called before you start the bus. We currently recommend that you put all of these in a static method for easy grouping and then call it at the beginning of the MassTransit configuration block.
:::

Most transactions in a system will end up being logged and wide scale correlation is likely. Therefore, the use of consistent correlation identifiers is recommended. In fact, using a `Guid` type is highly recommended. MassTransit uses the [NewId](https://www.nuget.org/packages/NewId) library to generate identifiers that are unique and sequential that are represented as a `Guid`. The identifiers are clustered-index friendly, being ordered in a way that SQL Server can efficiently insert them into a database with the *uniqueidentifier* as the primary key. Just use `NewId.NextGuid()` to generate an identifier -- it's fast, fun, and all your friends are doing it.

::: tip
So, what does correlated actually mean? In short it means that this message is a part of a larger conversation. For instance, you may have a message that says <i>New Order (Item:Hammers; Qty:22; OrderNumber:45)</i> and there may be another message that is a response to that message that says <i>Order Allocated(OrderNumber:45)</i>. In this case, the order number is acting as your correlation identifier, it ties the messages together.
:::

### Correlation Conventions

In addition to the explicit `CorrelateBy<T>` interface, a convention-based correlation is supported. If the message contract has a property named ``CorrelationId``, ``CommandId``, or ``EventId``, the correlationId header is automatically populated on Send or Publish. It can also be manually specified using the ``SendContext``.

Bear in mind that sagas default `CorrelateById()` only support messages where the explicit `CorrelateBy<Guid>` interface is implemented. However, the header is still useful if you do not use sagas, for example for message flow analysis and debugging.

## Guidelines

Given everything that was stated above, here are a few guidelines.

1. Interface-based inheritance is OK, don't be afraid, but don't go nuts.
1. Class-based inheritance is to be approached with caution.
1. Composing messages together ends up pushing us into content-based routing which is something we don't recommend.
1. Message Design is not OO Design (a message is just state, no behavior). There is a greater focus on interop and contract design.
1. As messages are more about contracts, we suggest subscribing to interfaces that way you can easily evolve the message definition.
1. A big base class may cause pain down the road as each change will have a larger ripple. This can be especially bad when you need to support multiple versions.
