# What does MassTransit add to the transport?

MassTransit is a lightweight service bus for building distributed .NET applications. The main goal is to provide
a consistent, .NET friendly abstraction over the message transport (whether it is RabbitMQ, Azure Service Bus, etc.).
To meet this goal, MassTransit brings a lot of the application-specific logic closer to the developer in an easy to 
configure and understand manner.

The benefits of using MassTransit over the message transport, as opposed to using the raw transport APIs and building
everything from scratch, are shown below. These are just a few, and some are more significant than others. The fact
that the hosting of your consumers, handlers, sagas, etc. are all managed consistently with a well documented
production ready framework is the biggest advantage. You can also find numerous blog posts, podcasts, and articles
written about MassTransit online.

### Concurrency

Concurrent, asynchronous message consumers for maximum receive throughput and high server utilization.

### Connection management

The network is unreliable. If the application is disconnected from the message broker, MassTransit takes care of
reconnecting and making sure all of the exchanges, queues, and bindings are restored.

### Exception, retries, and poison messages

Your message consumers don't need to know about broker acknowledgement protocols. If your message consumer runs to 
completion, the message is acknowledged and removed from the queue. If you throw an exception, MassTransit uses a 
retry policy to redeliver the message to the consumer. If the retries are exhausted due to continued failures or
other reasons, MassTransit moves the message to an error queue. If the message did not reach a consumer due to being
misrouted to the queue, the message is moved to a skipped queue.

### Serialization

C# is a statically typed language, and developers work with types. RabbitMQ works with bytes. So how do you format
a message over the wire? How do you handle different date/time formats (local, UTC, unspecified)? How do you deal
with numbers, are they integers, longs, or decimals? MassTransit has already thought about this and implemented 
sensible defaults for you. And there are many serializers provided out of the box, including JSON, BSON, and XML as
well as the .NET binary formatter as a last resort.

You can even protect your messages using AES-256 encryption, to keep prying eyes away and to ensure the safety of
private information (to meet PCI or HIPAA requirements).

### Message header and correlation

Designing a common message envelope can be a nitty-gritty affair until things stabilize. And MassTransit is already
stable having been used in production since 2008. The format is [well documented](../advanced/interoperability.html)
and has been tested with billions of messages. Furthermore, the envelope includes headers for tracking messages,
including conversations, correlations, and requests. The address and host information in the envelope make it easy to
build any messaging pattern.

### Consumer lifecycle management

MassTransit handles consumer creation and disposal, and integrates with most major dependency injection containers
using their built-in lifetime scope management. This ensures that dependencies are created and destroyed as part of 
the message consumption pipeline.

### Routing

MassTransit provides a heavily production tested convention for using RabbitMQ exchanges to route published messages
to the subscribed consumers. The structure is CPU and memory friendly, which keeps RabbitMQ happy.

### Rx integration

Interested in or already using Reactive Extensions? MassTransit makes it easy to connect Rx to RabbitMQ.

### Unit testing made easy

One of the first rules of unit testing is to avoid hitting infrastructure. And RabbitMQ is just that. MassTransit 
includes a high-performance in-memory transport for testing every consumer using the same code that would be used
in production. And the MassTransit.TestFramework NuGet package includes test harnesses
that handle the setup and teardown of the bus so you can easily test your message consumers and sagas.

### Sagas

Sagas are a powerful abstraction that supports message orchestration with durable state. Whether you use the original
somewhat explicit syntax, or the powerful state machine syntax of **Automatonymous**, you can build highly available
distributed workflow and coordination services easily. MassTransit supports both Entity Framework and NHibernate, using
code-based mapping and migrations to simply code deployments and upgrades.

### Scheduling

MassTransit has strong integration with Quartz.NET, to make it easy to schedule messages for future delivery. This brings
distributed applications into the fourth dimension, making time a first-class citizen. Some incredibly powerful routing
systems have been built by the authors using Quartz in combination with other MassTransit features.

There are also other scheduling providers that are supported by MassTransit, such as RabbitMQ deferred messages and
Azure Service Bus scheduled enqueueing.

### Monitoring performance counters

Keeping an eye on your services performance is critical, and having the right tools is a huge plus. MassTransit updates
a range of performance counters as messages are processed so operations can keep an eye on message flow and compare
the throughput to that of RabbitMQ.
