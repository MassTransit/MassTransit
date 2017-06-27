# Key terminology

When getting started using MassTransit, it is a good idea to have a handle on the terminology
used in messaging. To ensure that you are on the right path when looking at a class or interface,
here are some of the terms used when working with MassTransit.


## Messages and serialization

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

```csharp
sbc.UseJsonSerializer(); // uses JSON by default
sbc.UseXmlSerializer();  // uses XML by default
sbc.UseBsonSerializer(); // uses BSON (binary JSON) by default
```

## Sagas

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

```csharp
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
```

## Transports and endpoints

MassTransit is a framework, and being a framework has certain rules. The first of which is known
as the Hollywood principle -- "Don't call us, we'll call you." Once the bus is configured and
running, the receivers are called by the framework as messages are received. There is no need
for the application to poll a message queue or repeated call a framework method in a loop.

<div class="alert alert-info">
<b>Note:</b>
A way to understand this is to think of a message consumer as being similar to a controller
in a web application. With a web application, the socket and HTTP protocol are under the
hood, and the controller is created and action method called by the web framework. MassTransit
is similar, in that the message reception is handled by MT, which then creates the consumer
and calls the Consume method.
</div>

To initiate the calls into your application code, MassTransit creates an abstraction on top of
the messaging platform (such as RabbitMQ).

### Transports
The transport is at the lowest level and is closest to the actual message broker. The transport
communicates with the broker, responsible for sending and receiving messages. The send and receive
sections of the transport are completely independent, keeping reads and writes separate in line with
the Command Query Responsibility Segregation pattern.

### Receive endpoints
A receive endpoint receives messages from a transport, deserializes the message body, and routes
the message to the consumers. Applications do not interact with receive endpoints, other than to
configure and connect consumers. The rest of the work is done entirely by MassTransit.

### Send endpoints
A send endpoint is used by an application to send a message to a specific address. They can be
obtained from the `ConsumeContext` or the `IBus`, and support a variety of message types.

### Endpoint addressing
MassTransit uses Universal Resource Identifiers (URIs) to identify endpoints. URIs are flexible
and easy to include additional information, such as queue or exchange types. An example RabbitMQ
endpoint address for *my_queue* on the local machine would be: `rabbitmq://localhost/my_queue`
