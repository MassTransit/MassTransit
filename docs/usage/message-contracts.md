# Message contract

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

There are two main message types,_events_and_commands_. When choosing a name for a message, the type of message should dictate the tense of the message.

### Commands

A command tells a service to do something. Commands are sent (using`Send`) to an endpoint, as it is expected that a single service instance performs the command action. A command should never be published.

Commands should be expressed in a verb-noun sequence, following the_tell_style.

Example Commands:

* UpdateCustomerAddress
* UpgradeCustomerAccount
* SubmitOrder

### Events

An event signifies that something has happened. Events are published (using`Publish`) using 
either`IBus`or the `ConsumeContext`within a message consumer. An event should never be sent directly to an endpoint.

Events should be expressed in a noun-verb (past tense) sequence, indicating that something happened.

Example Events:

* CustomerAddressUpdated
* CustomerAccountUpgraded
* OrderSubmitted, OrderAccepted, OrderRejected, OrderShipped

## Message correlation

Since messages are usually do not live in isolation, publishing one message usually lead to publishing another 
message and then another and so on. It is useful to trace such sequences but to find them, these messages
need to have some information, how do they relate to each other.

The principle of binding messages together by some identifier is called correlation. Usually, message correlation
is done by some unique identifier, which is shared among all messages in one logical sequence.
This identifier is called correlationId.

MassTransit supports different methods to specify the correlationId. Check the [Correlating messages](correlation.md) 
article for more information.