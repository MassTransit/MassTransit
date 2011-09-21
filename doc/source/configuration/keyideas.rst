Key Terminology
===============

When getting started using MassTransit, it is a good idea to have a handle on the terminology
used in messaging. To ensure that you are on the right path when looking at a class or interface,
here are some of the terms used when working with MassTransit.

Receiving Messages
------------------

At the application layer, most users of MassTransit are interested in receiving messages.
There are several different receiver types that are supported, providing flexibility it
how you interact with the framework.

Handlers
""""""""

The easiest (and, by definition, least flexible) type of receiver is the **Handler**. A *handler*
is any method (including anonymous and lambda methods) that has a single argument of a message
type and a void return type.

.. sourcecode:: csharp

    void MyMessageHandler(MyMessage message)
    {}

When a message is received, MassTransit will call the method passing the message as the argument.
With a handler, no special controls are available to manage the lifecycle of the receiver. Therefore,
it is up to the application to deal with the fact that the handler may be called simultaneously 
from multiple threads if more than one message is being received. If your application is not
thread-safe, it is recommended that the concurrent consumer limit be set to one in the bus
configuration to avoid multithreading issues.

Instances
"""""""""

An **Instance** receiver is a class instance where the class implements one or more ``Consumes``
interfaces. Each of the ``Consumes`` interfaces accepts a generic argument (which must be a
reference type) that declares the type of message the instance wants to consume. Once an
instance is subscribed, as messages of the subscribed types are received, MassTransit will
call the ``Consume`` method on the class instance passing the message as the argument.

.. sourcecode:: csharp

    public class MyClass :
        Consumes<MyMessage>.All,
        Consumes<MyOtherMessage>.All
    {
        public void Consume(MyMessage message)
        {}
        public void Consume(MyOtherMessage message)
        {}
    }


Consumers
"""""""""

A **Consumer** is the most useful type of receiver and support a number of features that allow
proper lifecycle management of dependencies, as well as multiple message type handling. Consumers
are declared using the same interfaces as an *instance*, however, instead of subscribing an 
already created instance of the class to the bus, the consumer type is subscribed along with a
*consumer factory*. As messages are received, MassTransit calls the consumer factory to get an
instance of the consumer and calls the *Consume* method on the instance passing the message as
the argument.

By using the consumer factory, MassTransit allows the implementation to handle the lifecycle of
consumer instances. Actual implementations vary, and can range from a simple constructor call
to create an instance to the retrieval of a consumer and any of the consumers dependencies (such as a
database session, cache reference, etc.) from an inversion of control (IoC) container. Since
the consumer factory returns a handler to MassTransit, the factory can wrap the consumer call
with any lifecycle management/synchronization code before and after the message is consumed.

Interfaces for Consumers
''''''''''''''''''''''''

.. sourcecode:: csharp
    
    public class Consumes<TMessage>
    {
        public interface All : IConsumer
        {
            void Consume(TMessage message);
        }
        
        public interface Selected : All
        {
            bool Accept(TMessage message);
        }
        
        public interface For<TCorrelationId> :
            All,
            CorrelatedBy<TCorrelationId>
        {
            
        }
    }

All
'''

``Consumes<TMessage>.All``

This interface defines the ``void Consume(TMessage message)`` method

Selected
''''''''

``Consumes<TMessage>.Selected``

This interface defines an additional method allowing to process only selected
messages, by implementing the ``bool Accept(TMessage message)`` method.

For<TCorrelationId>
'''''''''''''''''''

``Consumes<TMessage>.For<TCorrelationId>``

This interface defines how to do a correlated consumer.


.. note::

    Consumers are usually sourced from an IoC container. When they are, MassTransit respects
    your container's lifecycle.

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
        InitiatedBy<MyInitialMessage>.All,
        Orchestrates<MyFollowUpMessage>.All
    {
        public Guid CorrelationId { get; set; }
        public void Consume(MyInitialMessage message)
        {}
        public void Consume(MyFollowUpMessage message)
        {}
    }


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


Transports and Endpoints
------------------------

MassTransit is a framework, and being a framework has certain rules. The first of which is known
as the Hollywood principle -- "Don't call us, we'll call you." Once the bus is configured and
running, the receivers are called by the framework as messages are received. There is no need
for the application to poll a message queue or repeated call a framework method in a loop.

To initiate the calls into your application code, MassTransit creates an abstraction on top of
the messaging platform (such as MSMQ or RabbitMQ).

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
an MSMQ endpoint on the local machine named "my_queue" would have the address shown below.

    ``msmq://localhost/my_queue``

A RabbitMQ queue on a remote server may be listed as below.

    ``rabbitmq://user@password:remote_server/my_queue``