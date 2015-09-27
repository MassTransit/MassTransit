Routing of messages in MassTransit
""""""""""""""""""""""""""""""""""

How are messages routed?

RabbitMQ routing conventionns
'''''''''''''''''''''''''''''

As we were building in the RabbitMQ support into MassTransit, we tried to follow the best
practices for RabbitMQ at the time. Also, since C# is a strongly typed language,
we have tried to make the most of that as well. MassTransit follows a routing scheme
that is based on the type of the message. All messages published in MassTransit are
routed by the Message Type. In our case the Message Type is going to be the .NET type of
the message class.

Another goal was leveraging RabbitMQ for as much of the routing logic possible. With MSMQ
we had to manage the routing logic ourselves and that added quite a bit of code to the project.
But with RabbitMQ's advanced routing features we hoped we could excise that piece of the system.

To achieve that we devised a routing system that leaned on RabbitMQ's concepts of bindings
and exchanges. By doing so the routing logic has been completely moved to RabbitMQ, which
has lead to us also working well with RabbitMQ's clustering support giving us more HA scenarios
as well.

Let's see the story of the following message classes

.. sourcecode:: csharp

  public class PingMessage { ... }
  public interface LiteCustomerData { ... }
  public class FullCustomerData : LiteCustomerData { ... }

Next we will see what happens when you subscribe to any of these messages.

A note about RabbitMQ. You send to exchanges in RabbitMQ. You receive messages
from queues. So how the hell do messages get anywhere? That's where
bindings come into play. You bind a queue to an exchange. That way one exchange
can service multiple queues. This abstracts the sending of the message from
the target queue. Pretty cool, eh? Ok, anyways.

MassTransit creates an exchange for each message type. So in the three messages
above you would see three exchanges. We also set up a pattern where we bind
exchanges to exchanges from the most specific to the most general. In this
example that would be:

  PingMessage

  FullCustomerData -> LiteCustomerData



.. NOTE::

  A word about Exchange to Exchange bindings. It is a RabbitMQ only feature.
  Exchange queue. To limit the amount of RabbitMQ churn we have established a
  directly bound exchange to your queue. This lets you come on and off the network with
  little impact to the flow of messages. [#churn]_

If you are leveraging our interface based messaging with a message class like

.. sourcecode:: csharp

  public class MyMessage : IMessageAA {}

You would see two exchanges 'MyMessage' and 'IMessageAA'. You will also see an
exchange to exchange binding from 'MyMessage' to 'IMessageAA' (from concrete
to the interface). If you subscribet to the concrete type you get a binding to
'MyMessage' if you subscribe to 'IMessageAA' you get a binding to 'IMessageAA'.

NOTE: Why are we doing this?

.. rubric:: Footnotes

.. [#churn] http://blog.springsource.com/2011/04/01/routing-topologies-for-performance-and-scalability-with-rabbitmq/
.. [#dump] http://codebetter.com/drusellers/2011/05/08/brain-dump-conventional-routing-in-rabbitmq/
