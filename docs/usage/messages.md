# Messages

In MassTransit, a message contract is defined _code first_ by creating a .NET type. A message can be defined using a record, class, or interface. Messages should only consist of properties, methods and other behavior should not be included.

::: warning Important
MassTransit uses the full type name, including the _namespace_, for message contracts. When creating the same message *type* in two separate projects, the namespaces **must** match or the message will not be consumed.
:::

The message examples below show the same command to update a customer address using each of the supported contract types.

### Using a record (recommended for .NET 5+)

```cs
namespace Company.Application.Contracts
{
	using System;

	public record UpdateCustomerAddress
	{
		public Guid CommandId { get; init; }
		public DateTime Timestamp { get; init; }
		public string CustomerId { get; init; }
		public string HouseNumber { get; init; }
		public string Street { get; init; }
		public string City { get; init; }
		public string State { get; init; }
		public string PostalCode { get; init; }
	}
}
```

### Using an interface

```cs
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

When defining a message type using an interface, MassTransit will create a dynamic class implementing the interface for serialization, allowing the interface with get-only properties to be presented to the consumer. To create an interface message, use a [message initializer](/usage/producers.md#message-initializers).

### Using a class 

```cs
namespace Company.Application.Contracts
{
	using System;

	public class UpdateCustomerAddress
	{
		public Guid CommandId { get; set; }
		public DateTime Timestamp { get; set; }
		public string CustomerId { get; set; }
		public string HouseNumber { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
	}
}
```

> Properties with `private set;` are not recommended as they are not serialized by default when using `System.Text.Json`.

::: tip
A common mistake when engineers are new to messaging is to create a base class for messages, and try to dispatch that base class in the consumer – including the behavior of the subclass. Ouch. This always leads to pain and suffering, so just say no to base classes.
:::

## Message Names

There are two main message types, _events_ and _commands_. When choosing a name for a message, the type of message should dictate the tense of the message.

### Commands

A command tells _a_ service to do something, and typically a command should only be consumed by a single consumer. If you have a command, such as `SubmitOrder`, then you should have only one consumer that implements `IConsumer<SubmitOrder>` or one saga state machine with the `Event<SubmitOrder>` configured. By maintaining the one-to-one relationship of a command to a consumer, commands may by _published_ and they will be automatically routed to the consumer. 

When using RabbitMQ, there is _no additional overhead_ using this approach. However, both Azure Service Bus and Amazon SQS have a more complicated routing structure and because of that structure, additional charges may be incurred since messages need to be forwarded from topics to queues. For low- to medium-volume message loads this isn't a major concern, but for larger high-volume loads it may be preferable to _[send](producers.md#send)_ (using `Send`) commands directly to the queue to reduce latency and cost.

Commands should be expressed in a verb-noun sequence, following the _tell_ style.

Example Commands:

* UpdateCustomerAddress
* UpgradeCustomerAccount
* SubmitOrder

### Events

An event signifies that something has happened. Events are [published](producers.md#publish) (using `Publish`) via either `ConsumeContext` (within a message consumer), `IPublishEndpoint` (within a container scope), or `IBus` (standalone).

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
| ConversationId    |Auto   | Assigned when the first message is sent or published and no consumed message is available, ensuring that a set of messages within the same conversation have the same identifier.|
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

## Correlation

Messages are usually part of a conversation and identifiers are used to connect messages to that conversation. In the previous section, the headers supported by MassTransit, including _ConversationId_, _CorrelationId_, and _InitiatorId_, are used to combine separate messages into a conversation. Outbound messages that are published or sent by a consumer will have the same _ConversationId_ as the consumed message. If the consumed message has a _CorrelationId_, that value will be copied to the _InitiatorId_. These headers capture the flow of messages involved in the conversation.

_CorrelationId_ may be set, when appropriate, by the developer publishing or sending a message. _CorrelationId_ can be set explicitly on the _PublishContext_ or _SendContext_ or when using a message initializer via the *\_\_CorrelationId* property. The example below shows how either of these methods can be used.

<<< @/docs/code/usage/UsageMessageCorrelation.cs{20-31}

### Correlation Conventions

_CorrelationId_ can also be set by convention. MassTransit includes several conventions by default, which may be used as the source to initialize the _CorrelationId_ header.

1. If the message implements the `CorrelatedBy<Guid>` interface, which has a `Guid CorrelationId` property, its value will be used.
1. If the message has a property named _CorrelationId_, _CommandId_, or _EventId_ that is a _Guid_ or _Guid?_, its value will be used.
1. If the developer registered a _CorrelationId_ provider for the message type, it will be used get the value.

The final convention requires the developer to register a _CorrelationId_ provider prior to bus creation. The convention can be registered two ways, one of which is the new way, and the other which is the original approach that simply calls the new way. An example of the new approach, as well as the previous method, is shown below.

<<< @/docs/code/usage/UsageMessageSetCorrelation.cs

The convention can also be specified during bus configuration, as shown. In this case, the convention applies to the configured bus instance. The previous approach was a global configuration shared by all bus instances.

<<< @/docs/code/usage/UsageMessageSendCorrelation.cs

Registering _CorrelationId_ providers should be done early in the application, prior to bus configuration. An easy approach is putting the registration methods into a class method and calling it during application startup.

### Saga Correlation

Sagas _must_ have a _CorrelationId_, it is the primary key used by the saga repository and the way messages are correlated to a specific saga instance. MassTransit follows the conventions above to obtain the _CorrelationId_ used to create a new or load an existing saga instance. Newly created saga instances will be assigned the _CorrelationId_ from the initiating message.

::: tip New in Version 7
Previous versions of MassTransit only supported automatic correlation when the message implemented the `CorrelatedBy<Guid>` interface. Starting with Version 7, all of the above conventions are used.
:::

### Identifiers

MassTransit uses and highly encourages the use of _Guid_ identifiers. Distributed systems would crumble using monotonically incrementing identifiers (such as _int_ or _long_) due to the bottleneck of locking and incrementing a shared counter. Historically, certain types (okay, we'll call them out - SQL DBAs) have argued against using _Guid_ (or, their term, _uniqueidentifier_) as a key – a clustered primary key in particular. However, with MassTransit, we solved that problem.

MassTransit uses [NewId](https://www.nuget.org/packages/NewId) to generate identifiers that are unique, sequential, and represented as a _Guid_. The generated identifiers are clustered-index friendly, and are ordered so that SQL Server can efficiently insert them into a database with the *uniqueidentifier* as the primary key.

To create a _Guid_, call `NewId.NextGuid()` where you would otherwise call `Guid.NewGuid()` – and start enjoying the benefits of fast, distributed identifiers.

## Guidance

When defining message contracts, what follows is general guidance based upon years of using MassTransit combined with continued questions raised by developers new to MassTransit.

-	Use interfaces and message initializers. Once you adjust it starts to make more sense. Use the Roslyn Analyzer to identify missing or incompatible property initializers.
	-	Inheritance is okay, but keep it sensible as the type hierarchy will be applied to the broker. A message type containing a dozen interfaces is a bit annoying to untangle if you need to delve deep into message routing to troubleshoot an issue.
-	Class inheritance has the same guidance as interfaces, but with more caution.
	-	Consuming a base class type, and expecting polymorphic method behavior almost always leads to problems.
	-	Message design is not object-oriented design. Messages should contain state, not behavior. Behavior should be in a separate class or service.
	-	A big base class may cause pain down the road as changes are made, particularly when supporting multiple message versions.
