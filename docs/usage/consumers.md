# Consumers

Consumer is a widely used noun for something that _consumes_ something. In MassTransit, a consumer consumes one or more message types when configured on or connected to a receive endpoint. MassTransit includes many consumer types, including consumers, [sagas](/usage/sagas/), saga state machines, [routing slip activities](/advanced/courier/), handlers, and [job consumers](/advanced/job-consumers).

A consumer, which is the most common consumer type, is a class that consumes one or more messages types. For each message type, the `IConsumer<T>` interface is implemented where `T` is the consumed message type. The interface has one method, _Consume_, as shown below.

```cs
public interface IConsumer<in TMessage> :
    IConsumer
    where TMessage : class
{
    Task Consume(ConsumeContext<TMessage> context);
}
```

> Messages must be reference types, and interfaces are supported (and recommended). The [messages](/usage/messages) section has more details on message types. 

An example class that consumes the _SubmitOrder_ message type is shown below.

<<< @/docs/code/usage/UsageConsumer.cs

MassTransit embraces _The Hollywood Principle_, which states, "Don't call us, we'll call you." It is influenced by the Dependency Inversion Principle in that control flows from the framework into the developer's code in response to an event, which in this case involves the delivery of a message by the transport. When a message is delivered from the transport on a receive endpoint and the message type is consumed by the consumer, MassTransit creates a consumer instance (using a consumer factory), and executes the _Consume_ method passing a `ConsumeContext<T>` containing the message.

The _Consume_ method returns a _Task_ that is awaited by MassTransit. While the consumer method is executing, the message is unavailable to other receive endpoints. If the _Task_ completes successfully, the message is acknowledged and removed from the queue.

If the _Task_ faults in the event of an exception, or is canceled (explicitly, or via an _OperationCanceledException_), the consumer instance is released and the exception is propagated back up the pipeline. If the exception does not trigger a retry, the default pipeline will move the message to an error queue.

## Consumer

To receive messages, a consumer must be configured on a receive endpoint and receive endpoints are configured with the bus. A configuration example with a single receive endpoint containing the _SubmitOrderConsumer_ above is shown below.

<<< @/docs/code/usage/UsageConsumerBus.cs

The consumer is configured on a receive endpoint, which will receive messages from the `order-service` queue, using the `.Consumer<T>()` method. Since the consumer has a default constructor (no constructor implies a default constructor), it can be configured without specifying a consumer factory.

::: tip Under the Hood
When a consumer is configured on a receive endpoint, the consumer message types (one for each `IConsumer<T>`) are used to configure the receive endpoint's _consume topology_. The consume topology is then used to configure the broker so that published messages are delivered to the queue. The broker topology varies by transport. For example, the RabbitMQ example above would result in the creation of an exchange for the _SubmitOrder_ message type and a binding from the exchange to an exchange with the same name as the queue (the latter exchange then being bound directly to the queue).

If the queue is persistent (_AutoDelete_ is false, which is the default), the topology remains in place even after the bus has stopped. When the bus is recreated and started, the broker entities are reconfigured to ensure they are properly configured. Any messages waiting in the queue will continue to be delivered to the receive endpoint once the bus is started.
:::

### Skipped Messages

When a consumer is removed (or disconnected) from a receive endpoint, a message type is removed from a consumer, or if a message is mistakenly sent to a receive endpoint, messages may be delivered to the receive endpoint that do not have a consumer. 

If this occurs, the unconsumed message is moved to a *_skipped* queue (prefixed by the original queue name). The original message content is retained and additional headers are added to identify the host that skipped the message.

::: warning Manual Intervention
It may be necessary to use the broker management tools to remove an exchange binding or topic subscription that is no longer used by the receive endpoint to prevent further skipped messages.
:::

### Consumer Factories

::: tip 
When using a container, these methods should _NOT_ be used.
:::

In the example shown above, the consumer had a default constructor so the default constructor consumer factory was used. There are several other consumer factories included, some of which are shown below. If you are using MassTransit with a container, MassTransit includes support for several containers and integrates with them to provide container scope for consumers. Refer to the [containers](/usage/containers) section for details.

<<< @/docs/code/usage/UsageConsumerOverloads.cs

To reiterate, the consumer factory is called for each message to create a consumer instance. Once the _Consume_ method completes, the consumer is dereferenced.

::: tip IDispose / IAsyncDisposable
If using the default or delegate consumer factory and the consumer supports either `IAsyncDisposable` or `IDisposable`, the appropriate dispose method will be called.

When using a container, it is responsible for consumer disposal when the scope is disposed.
:::

### Temporary Consumers

Some consumers only need to receive messages while connected, and any messages published while disconnected should be discarded. This is achieved by using a TemporaryEndpointDefinition as the endpoint definition.

<<< @/docs/code/usage/UsageConsumerTemporaryEndpoint.cs

### Connect Consumers

Once a bus has been configured, the receive endpoints have been created and cannot be modified. However, the bus creates a temporary, auto-delete endpoint for itself. Consumers can be connected to the bus endpoint using any of the `Connect` methods. The bus endpoint is designed to receive responses (via the request client, see the [requests](/usage/requests) section) and **messages sent directly to the bus endpoint**.

::: warning Consume Topology
The bus endpoint does not use its consume topology to configure the broker, and message type exchanges are not created, bound, or otherwise subscribed to the bus endpoint queue. _Published_ messages will not be delivered to the bus endpoint queue and subsequently will not be delivered to consumers connected to the bus endpoint.

This makes the bus endpoint very fast short-lived consumers, such as the request client.
:::

The following example connects a consumer to the bus endpoint, and then sets the _ResponseAddress_ header on the _SubmitOrder_ message to the bus endpoint address.

<<< @/docs/code/usage/UsageConsumerConnect.cs

There are connect methods for the other consumer types as well, including handlers, instances, sagas, and saga state machines.

## Instance

Creating a new consumer instance for each message is highly suggested. However, it is possible to configure an existing consumer instance to which every received message will be delivered (if the message type is consumed by the consumer).

::: tip Thread Safety
An instance consumer **must** be thread-safe since the _Consume_ method may be called from multiple threads simultaneously.
:::

An example instance configuration is shown below.

<<< @/docs/code/usage/UsageInstance.cs

## Handler

While creating a consumer is the preferred way to consume messages, it is also possible to create a simple message handler. By specifying a method, anonymous method, or lambda method, a message can be consumed on a receive endpoint.

> This is great for unit testing, and other simple scenarios. Beyond that, use a consumer.

A simple handler example is shown below.

<<< @/docs/code/usage/UsageHandler.cs

The asynchronous handler method is called for each message delivered to the receive endpoint. Since there is no consumer to create, no scope is created if using a container, and nothing is disposed.
