What does MassTransit add on top of MSMQ and RabbitMQ?
======================================================

MassTransit is a service bus implementing the data bus pattern in a distributed setting. It aims to be a .Net-friendly abstraction over the messaging technologies MSMQ and RabbitMQ. As such it brings a lot of the application-specific logic closer to the programmer in an easy-to-configure manner.

Below follows a few of the benefits of having MassTransit as opposed to having raw access to the transport and building everything directly on top of the transport.

Sagas
-----
Sagas is a coordination mechanism in distributed systems that helps with checkpointing. Often Sagas listen for events or messages and reacts on them by sending further messages; what the outgoing messages are may depend on contextual information and questions; such as 'How long ago was this orchestration started?'

Threaded Consumers 
-------------------
Multiple concurrent receives possible.

Exception Management
--------------------
If your connection to the message broker or queue server goes down, MassTransit takes care of trying to reconnect and deal with those failures, so that you don't have to.

Retries & Poison Messages
---------------------------
MassTransit implements some level of generic exception handling for your consumers: upon complete failure from your application to deal with a message, it's moved to an error queue which allows you to inspect the message and requeue it.

If exceptions are thrown from consumers, MassTransit by default performs a number of retries by requeueing the message, before moving it to the error queue.

Transactions
------------
Currently only supported on MSMQ, transactions allow you to join a dequeue operation with a database operation inside of a transaction and have them execute with ACID properties.

Serialization
-------------
How do you format a message over the wire? How to handle DateTime (Unspecified, Local, Utc)? How to handle decimal numbers? MassTransit has already thought about it an implemented sensible defaults. MassTransit provides a number of serializers, including BSON, JSON, XML and Binary.

Headers
-------
Manufacturing a header and using a common envelope format can be a nitty-gritty afair until things stabilize. MassTransit has a documented format that has been tested with billions of messages. Furthermore, the envelope in use allows buses on other nodes to reply using the source address and perform other messaging patterns more easily.

Consumer Lifecycle
------------------
MassTransit handles the creation and disposal of your consumers and doesn't unsubscribe from a queue/exchange until all consumers, consuming specifically the messages that cause the exchange/queue binding, have been unsubscribed.

Also MassTransit handles durable and transient subscriptions and their semantics.

Routing
-------
MassTransit implements a subscription service over dumb transports such as MSMQ and intelligent exchange-queue bindings for smart transports like RabbitMQ. This service communicates in a prioritized band called the Control Bus.

Rx integration
--------------
Interested in using Reactive Extensions to consume? MassTransit gives you this.

Unit Testability
----------------
The loopback transport is an in-memory transport that allow you a certain freedom in your integration tests (end-to-end in a local service).

Furthermore, the TestFactory allows you to easily set up both loopback and real transport-based unit tests for your consumers.

The code-base of MassTransit at https://github.com/MassTransit/MassTransit contains a large number of well written tests that service both as verification of functionality and examples for your own unit tests.

Fluent NHibernate Integration
-----------------------------
Easily map and register your Sagas with Fluent NHibernate and let MassTransit handle the transaction boundary of your Saga, while giving your application easy access to the data in the saga.

In this case you further have the option of unit testing your sagas using Fluent NHibernate using an in-memory SQLite database, which will make your tests run smooth like a mountain river.

Routing & Static Routing
------------------------
The routing engine is state-of-the-art, using the Rete Algorithm:http://en.wikipedia.org/wiki/Rete_algorithm with Stact - the .Net actor framework. 

If you want to route differently than the default per-type routing, MassTransit will allow you to do this easily.

Hackable
--------
If you feel like extending MassTransit with a Transport, Serializer or Service; the interfaces have small surface areas and we're here to help you (both on github and in MassTransit-discuss).

Diagnostics
-----------
Using BusDriver, you can diagnose and inspect any bus on the network by communicating with it over the control bus.

Tracing
-------
Using the tracing functionality you can get very detailed timings of when and where things were consumed, how long the receive took, how long the consume took and what exceptions were thrown if any.

Monitoring
----------
Using the System.Diagnostics namespace and Performance Counters, you can let your operations team know how your applications are doing; message rates and health status.

Distributor
-----------
Using the distributor you can create load-based routing, thereby maximizing the use of your computers.

Timeout Service
---------------
You can schedule persistent callbacks/timeouts in your sagas that allow your application to wake up after e.g. a scheduled SLA limit.

Encryption
----------
Using the PreSharedKeyEncryptedMessageSerializer you get pre-shared key Rijndael encryption.