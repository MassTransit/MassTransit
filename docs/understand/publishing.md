# Publishing messages

When a message is published (by way of a call to `bus.Publish`), it's important to understand what MassTransit actually
does under the hood. While the explicit implementation details depend upon the message transport being used, the general
pattern is the same.

MassTransit follows the [publish subscribe][1] message pattern, where a copy of the message is delivered to each subscriber.
The message transport determines how the actual routing is performed, but the conventions of each transport are described below.

## Routing on RabbitMQ

RabbitMQ provides powerful routing capabilities out of the box, in the form of exchanges and queues. Exchanges can be bound
to queues, as well as other exchanges, making it easy to create a message routing fabric. MassTransit leverages exchanges
and queues combined with the .NET type system to connect subscribers to publishers.

MassTransit uses the message type to declare exchanges and exchange bindings that match the hierarchy of types implemented
by the message type. Interfaces are declared as separate exchanges (using a fully-qualified type name that is compatible with
the naming structure of exchanges) and bound to the published message type exchange. When a message is first published, the
exchanges are declared once, and then used for the life of the channel.

> Private types, such as classes, are declared as auto-delete so they do not clutter up the exchange namespace.

Once declared, published messages are to the message type exchange, and copies are routed to all the subscribers by RabbitMQ.
This approach was [based on an article][2] on how to maximize routing performance in RabbitMQ.

This dynamic, type-based routing model has proved very powerful in many large applications. The ability to add
new consumers to an existing message publisher is a great way to manage dependencies and keep projects from becoming tightly
coupled.

To see how this plays out, consider the following message types:

```csharp
namespace Company.Messages
{
    public interface CustomerAddressUpdated
    {
    }

    public interface UpdateCustomerAddress
    {
    }

    public class UpdateCustomerAddressCommand :
        UpdateCustomerAddress
    {
    }
}
```

Once the messages have been published, exchanges are created in RabbitMQ for each of the message types:

```text
Exchanges

Company.Messages.CustomerAddressUpdated
Company.Messages.UpdateCustomerAddress
Company.Messages.UpdateCustomerAddressCommand
    - Includes a binding to Company.Messages.UpdateCustomerAddress
```

When a receive endpoint is started, the second half of the exchange/queue binding is performed, where the consumer subscriptions
are bound to the consumer message type exchanges, closing the loop.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
    {
        e.Consumer<UpdateCustomerConsumer>();
    });
});
```

This results in the creation of a queue, as well as a binding to the queue from the `UpdateCustomerAddress` exchange.

```text
Exchanges

customer_update_queue
    - Includes a binding from Company.Messages.UpdateCustomerAddress

Queues

customer_update_queue
    - Includes a binding from the customer_update_queue exchange
```

> Because RabbitMQ only allows messages to be sent to exchanges, an exchange matching the name of the queue is created and bound to the queue.
This makes it easy to send messages directly to the queue using the same name. It's actually a pretty cool abstraction, and RabbitMQ makes
it very flexible by allowing exchange-to-exchange bindings. By keeping the bindings at the exchange level, it eliminates any impact to message
flow. Dru [shared his experience][3] with this as well.

### Balancing the load

Because RabbitMQ is a message broker, it supports multiple readers from the same queue. This makes it super easy to setup a
load balancing scenario where the same service is running on multiple servers, each of which is connected to the same queue. As
messages arrive on the queue, they are delivered to the first available consumer that can receive the message. To get good
load balancing, it's important to set the `PrefetchCount` to a sensible value on the consumer so that messages are well distributed.

### Routing on Azure Service Bus

MassTransit uses a similar approach for Azure Service Bus, but uses Topics, Subscriptions, and Queues.

> More details to come...

[1]: http://www.enterpriseintegrationpatterns.com/patterns/messaging/PublishSubscribeChannel.html
[2]: http://spring.io/blog/2011/04/01/routing-topologies-for-performance-and-scalability-with-rabbitmq/
[3]: http://codebetter.com/drusellers/2011/05/08/brain-dump-conventional-routing-in-rabbitmq/
