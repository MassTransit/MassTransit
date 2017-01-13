---
layout: post
title: Message Contracts.
subtitle: Leverage contracts to enable decoupled systems.
---

Good fences make good neighbors.

* ToC
{:toc}

In MassTransit, a message contract is defined using the .NET type system. Messages can be defined using both classes and interfaces, however, it is suggested that types use read-only properties and no behavior.

> It is strongly suggested to use interfaces for message contracts, based on experience over several years with varying levels of developer experience. MassTransit will create dynamic interface implementations for the messages, ensuring a clean separation of the message contract from the consumer.

An example message to update a customer address is shown below.


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

A common mistake when engineers are new to messaging is to create a base class for messages, and try to dispatch that base class in the consumer – including the behavior of the subclass. Ouch. This always leads to pain and suffering, so just say no to base classes.

## Specifying message names

There are two main message types, *events* and *commands*. When choosing a name for a
message, the type of message should dictate the tense of the message.

### Commands


A command tells a service to do something. Commands are sent (using ``Send``) to an endpoint,
as it is expected that a single service instance performs the command action. A command should
never be published.

Commands should be expressed in a verb-noun sequence, following the *tell* style.

Example Commands:

* UpdateCustomerAddress
* UpgradeCustomerAccount
* SubmitOrder

### Events

An event signifies that something has happened. Events are published (using ``Publish``) using
either ``IBus`` or the ``ConsumeContext`` within a message consumer. An event should never be
sent directly to an endpoint.

Events should be expressed in a noun-verb (past tense) sequence, indicating that something happened.

Example Events:

* CustomerAddressUpdated
* CustomerAccountUpgraded
* OrderSubmitted, OrderAccepted, OrderRejected, OrderShipped


### Correlating messages

There are several built-in message headers that can be used to correlate messages. However, it is also
completely acceptable to add properties to the message contract for correlation. The default headers
available include:

#### CorrelationId
  An explicit correlation identifier for the message. If the message contract has a property named
  ``CorrelationId``, ``CommandId``, or ``EventId`` this header is automatically populated on Send
  or Publish. Otherwise, it can be manually specified using the ``SendContext``.

#### RequestId
  When using the ``RequestClient``, or the request/response message handling of MassTransit, each
  request is assigned a unique ``RequestId``. When the message is received by a consumer, the response
  message sent by the ``Respond`` method (on the ``ConsumeContext``) is assigned the same ``RequestId``
  so that it can be correlated by the request client. This header should not typically be set by the
  consumer, as it is handled automatically.

#### ConversationId
  The conversation is created by the first message that is sent or published, in which no existing
  context is available (such as when a message is sent or published from a message consumer). If an
  existing context is used to send or publish a message, the ``ConversationId`` is copied to the
  new message, ensuring that a set of messages within the same *conversation* have the same identifier.

#### InitiatorId
  When a message is created within the context of an existing message, such as in a consumer, a saga, etc.,
  the ``CorrelationId`` of the message (if available, otherwise the ``MessageId`` may be used) is copied
  to the ``InitiatorId`` header. This makes it possible to combine a chain of messages into a graph of
  producers and consumers.

#### MessageId
  When a message is sent or published, this header is automatically generated for the message.
