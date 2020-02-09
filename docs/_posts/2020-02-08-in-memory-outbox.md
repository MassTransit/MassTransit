---
title: "In-Memory Outbox, Take out the Trash"
date: 2020-02-08
author: "Chris"
tag: 
  - Patterns
  - Outbox
  - 2PC
---

This post details the _In-Memory Outbox_, including what it does, how to configure it, and how it ensures eventual consistency in the presence of database or message transport failures.

<!-- more -->

MassTransit implements messaging patterns, many of which are designed to ease the transition from a tightly-coupled, database-centric application to a set of services that are highly available, reliable, and eventually consistent. Some of these patterns are obvious, but some of them require a little more explanatation to truly understand how they are best utilized.

## Commands

Commands are used to do things, like update a database record. Updating a database record usually includes publishing events to notify services that a change in state has occurred.

In a transactional mindset, updating the database and publishing the event are expected to be performed as a single atomic operation. In distributed systems, performing a distributed transaction between the database and the message broker is unrealistic.

## Sagas

Sagas in MassTransit are message handlers that maintain state. An initial message creates a saga _instance_ and subsequent messages may correlate to the same instance. Between messages, the sage instance _state_ is persisted using a database. While consuming a message, a saga may send commands and/or publish events.

The message flow for a saga includes:

1. On message receipt, an existing saga instance is loaded from the database. If a matching instance does not exist, a new instance is created.
1. The message is delivered to the saga instance.
1. Once the message is handled, the saga instance is updated or saved to the database.

Step 2 is where _the magic happens_. State can be updated, messages can be sent and published, anything really.

So, what's the problem? A few things actually.

### Failures

An obvious problem is a database failure saving the saga instance. If messages were already sent or published, and the instance was not saved, other services would receive those messages yet the database has not been updated.

A race condition is another concern, since the events may be consumed before the database update is complete. Yes, message brokers are _fast_, and many times messages are already being consumed long before (in computer time) the database update is started.

#### So, retry?

Retrying operations is a key trait of a resilient system. Transient failures happen, even more so in distributed systems, so it makes sense to retry failures in the presents of failures. Of course, not all failures are transient. For instance, trying to create an invoice when the same invoice number has already been created is _never_ going to be successful.

> In this example, designing idempotent services such that duplicate operations do not result in duplicate invoices would be the best solution. But that's another topic worth studying.

If retrying the database failure isn't enough, it may make sense to retry the entire message processing sequence – starting at step 1. In this case, the saga instance is discarded, and the message is retried from the beginning. The saga instance is loaded (or created), the message is delivered, and the instance is saved. This is repeated until it is successful or until the retry policy expires and the message is moved to the *_error* queue.

> Because, it's clearly bad. Study _poison message handling_.

A new issue related to retry is duplicate messages. Sent or published messages may be sent or published multiple times – once for each attempt. This can create non-deterministic behavior in services that are consumed those messages. Therefore, a method of delaying those messages from being sent until the saga instance is saved is needed.

## The Outbox

The outbox holds messages and delivers them after the _transactional_ portion of the message processing has completed. With a saga, the messages are delivered after the saga instance is saved successfully. This ensures that the database is updated before any consumers can start processing any of the produced messages.

### The In-Memory Outbox

Which brings us to the In-Memory Outbox, which is a feature of MassTransit. The In-Memory Outbox holds published and sent messages in memory until the message is processed successfully (such as the saga being saved to the database). Once the message has been processed, the message are delivered to the broker and the received message is acknowledged as successful.

> MassTransit consumes messages in _acknowledgement_ mode, which means a message remains locked and is invisible to other consumers until it is either acknowledged (ack'd) or negatively-acknowledged (n'ack'd).

The full configuration is in the [documentation](...), an example is shown below.

```cs
cfg.ReceiveEndpoint("invoice-saga", e =>
{
    e.UseMessageRetry(r => r.Immediate(5));
    e.UseInMemoryOutbox();

    e.StateMachineSaga<InvoiceStateMachine, InvoiceState>(machine, repository);
});
```

### But, but, what if the message doesn't send?

This question comes up, and it is a fair question. If the broker goes down, the outbox would be unable to deliver the messages. If the process crashes, the messages in the outbox would be lost. Both of these failures can happen, though it is rare. And if computer science has one rule, it is that the rare will always happen. In production. On a Friday afternoon.

## Time to Take out the Trash

Imagine you're twelve, sitting on the sofa, playing video games with your friends. Suddenly, from the other room you hear your mom call out, "Take out the trash!" Of course, you're in the middle of a battle, and while you've explained many times that you can't pause a multiplayer game, mom just doesn't get it. So you do what any 12-year-old does, you ignore her. The trash remains right where it is, in the kitchen.

After a while, the lack of a door opening and closing, the still present smell of burnt popcorn from the kitchen, and your mom calls out again, "Take out the trash." At this point, you're dead, in spectator mode, and decide to comply – you take out the trash. Then you slide back onto the sofa and get ready for round two.

More time passes, the squad is ready and you're about to get on the bus. Your mom, however, didn't hear from you and shouts once more, "I said take out the trash." "Mom, I already took it out" you reply, realizing after that you forgot to mute your mic. The jests and jokes commence as you thank the bus driver and head out.

### The Story

This real world example includes both failures scenarios that are brought up when considering the in-memory outbox.

First, it didn't happen. The database may have been unavailable or the service crashed deserializing the message. Either way, it failed. And the message? It's still on the broker. It will be redelivered. _Mom will keep telling you to take out the trash, until you take it out._

Second, it happened but the messages were not delivered. _You didn't tell her you took it out._ In this case, the message will also be retried. But in this case, this rare case, this is where the previously mentioned term _idempotence_ comes back onto the field.

When the message is attempted a third time (and face it, the third time is dangerously close to getting a chancla to the head), the database was already updated. The invoice is already approved, _in the database_. The messages weren't sent, however, so other services may not know that the invoice was approved. In this case, for the service to be idempotent, it should assume that:

1. The message delivery failed, because it is being delivered – again.
2. Since the invoice is approved, and this is the approve invoice command, something must have failed after the database was updated. 
3. The only thing after the database update is the outbox delivering messages.

> Study Occam's Razer (okay, yeah, I'm a fan of Razer gaming gear so I'm leaving it spelled that way)

The correct thing to do at this point is to use the state in the database, along with any information that is contained in the message, to produce the same commands and events that were produced in the previous attempt. Those messages will be delivered by the outbox, and the message will be acknowledged.

### !! Victory !!

That's it, an easy-to-use, reliable solution to perform atomic operations which update a database and send/publish messages, and it works for any database updates that are sent as commands (delivered by durable message queues).

#### Other Reading

[Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html)
[NServiceBus Outbox](https://docs.particular.net/nservicebus/outbox/)


