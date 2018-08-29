# Message contract
<!-- TOC -->

- [Specifying message names](#specifying-message-names)
    - [Commands](#commands)
    - [Events](#events)
- [Message correlation](#message-correlation)

<!-- /TOC -->
In MassTransit, a message contract is defined using the .NET type system. Messages can be defined using both 
classes and interfaces, however, it is suggested that types use read-only properties and no behavior.

<div class="alert alert-info">
<b>Note:</b>
It is strongly suggested to use interfaces for message contracts, based on experience over several years with 
varying levels of developer experience. MassTransit will create dynamic interface implementations for the messages, 
ensuring a clean separation of the message contract from the consumer.
</div>

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

A common mistake when engineers are new to messaging is to create a base class for messages, and try to dispatch that base class in the consumer – including the behavior of the subclass. Ouch. This always leads to pain and suffering, so just say no to base classes.

## Specifying message names

There are two main message types, _events_ and _commands_. When choosing a name for a message, the type of message should dictate the tense of the message.

### Commands

A command tells a service to do something. Commands are sent (using `Send`) to an endpoint, as it is expected that a single service instance performs the command action. A command should never be published.

Commands should be expressed in a verb-noun sequence, following the_tell_style.

Example Commands:

* UpdateCustomerAddress
* UpgradeCustomerAccount
* SubmitOrder

### Events

An event signifies that something has happened. Events are published (using `Publish`) using 
either `IBus` or the `ConsumeContext` within a message consumer. An event should never be sent directly to an endpoint.

Events should be expressed in a noun-verb (past tense) sequence, indicating that something happened.

Example Events:

* CustomerAddressUpdated
* CustomerAccountUpgraded
* OrderSubmitted, OrderAccepted, OrderRejected, OrderShipped

## Message correlation

Since messages usually do not live in isolation, publishing one message usually leads to publishing another 
message, and then another, and so on. It is useful to trace such sequences, however, to find them these messages
need to have some information detailing how they relate to each other.

Correlation is the principle of connecting messages together, usually by using a unique identifier that is included in every message that is part of a logical sequence. In MassTransit, the unique identifier is referred to as the `CorrelationId`, which is included in the message envelope and available via the `ConsumeContext` or the `SendContext`. MassTransit also includes a `ConversationId` which is the same across an entire set of related messages.

MassTransit supports different methods to specify the correlationId. Check the [Correlating messages](correlation.md) 
article for more information.