Key Terminology
===============

When getting started using MassTransit, it is a good idea to have a handle on the terminology
used in messaging. To ensure that you are on the right path when looking at a class or interface,
here are some of the terms used when working with MassTransit.

Messages and Serialization
--------------------------

MassTransit is a service bus, and a service bus is designed to move *messages*. At the lowest
level, a message is a chunk of JSON, XML, or even binary data. When using a statically typed
language (such as C#), a message is represented by an instance of a class (or interface) that
has relevant properties, each of which can be a value, list, dictionary, or even another nested
class.

When using MassTransit, messages are sent and received, published and subscribed, as types. The
translation (called serialization) between the textual representation of the message (which is
JSON, XML, etc.) and a type is handled using a *message serializer*. The default serialization
varies (for MSMQ, the framework uses XML by default, for RabbitMQ JSON is used instead). The
default serialization can be changed when a service bus is being configured.

.. sourcecode:: csharp

    sbc.UseJsonSerializer(); // uses JSON by default
    sbc.UseXmlSerializer();  // uses XML by default
    sbc.UseBsonSerializer(); // uses BSON (binary JSON) by default

Sagas
"""""

All of the receiver types above are stateless by design, the framework makes no effort to
correlate multiple messages to a single receiver. Often it is necessary to orchestrate
multiple messages, usually of different types, into a saga (sometimes called a workflow). A
saga is a long-running transaction that is managed at the application layer (instead of, for
example, inside of a database or a distributed transaction coordinator). MassTransit allows
sagas to be declared as a regular class or as a state machine using a fluent interface.

The key difference for sagas is that the framework manages the saga instance and correlates
messages to the proper saga instance. This correlation is typically done using a *CorrelationId*,
which is an interface (called *CorrelatedBy*). Messages correlated an individual saga must be
done using a **Guid**. Sagas may also *observe* messages that are not correlated directly to
the saga instance, but this should be done carefully to avoid potentially matching a message
to hundreds of saga instances which may cause database performance issues.

.. sourcecode:: csharp

    public class MySaga :
        ISaga,
        InitiatedBy<MyInitialMessage>,
        Orchestrates<MyFollowUpMessage>
    {
        public Guid CorrelationId { get; set; }
        public Task Consume(ConsumeContext<MyInitialMessage> message)
        {}
        public Task Consume(ConsumeContext<MyFollowUpMessage> message)
        {}
    }

Transports and Endpoints
------------------------

MassTransit is a framework, and being a framework has certain rules. The first of which is known
as the Hollywood principle -- "Don't call us, we'll call you." Once the bus is configured and
running, the receivers are called by the framework as messages are received. There is no need
for the application to poll a message queue or repeated call a framework method in a loop.

To initiate the calls into your application code, MassTransit creates an abstraction on top of
the messaging platform (such as RabbitMQ).

Transports
""""""""""

At the lowest level, closest to the actual messaging platform used, is the transport. Transports
communicate with the actual platform API to send and receive messages. The transport implementation
is split into two parts, inbound and outbound, providing the ability to support asymmetric APIs
where sending and receiving have different behaviors and/or addresses.

Endpoints
"""""""""

The endpoint is the abstraction used to send messages directly and to receive messages by the
service bus. It is very uncommon (and not recommended) for an application to call *Receive*
on an endpoint. Endpoints are referenced by *address* and no distinction is made between inbound
and outbound at the endpoint level.

Address
"""""""

In MassTransit, a URI is used as an address to an endpoint. The elements of the URI are used to
determine the proper transport, server, port, and queue name of the actual endpoint. For example,
a RabbitMQ endpoint on the local machine named "my_queue" would have the address shown below.

    ``rabbitmq://localhost/my_queue``
