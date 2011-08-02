Routing of Messages in MassTransit
""""""""""""""""""""""""""""""""""

How are messages routed?

RabbitMQ Routing Conventionns
'''''''''''''''''''''''''''''

As we were building in the RabbitMQ support we have tried to follow the best
practices for RabbitMQ at the time. Also since we (.net) are a strongly typed language
we have tried to make the most of that as well. MassTransit follows a routing scheme
that is based on message type. Everything in MassTransit is routed by the Message Type.
In our case the Message Type is going to be the .net type of the message class.

Another value we wanted to bring was leveraging RabbitMQ as much as possible
for the routing logic. To acheive that we devised a system that would allow 
the routing logic to be completely embedded in RabbitMQ which helps with both message
flow and helps us acheive a high availability scenario.

Lets see the story of the following message classes

.. sourcecode:: csharp

  public class PingMessage { ... }
  public interface LiteCustomerData { ... }
  public class FullCustomerData : LiteCustomerData { ... }

Next we will see what happens when you subscribe to any of these messages.

A note about rabbitmq. You send to exchanges in rabbitmq. you receive messages
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

  A word about Exchange to Exchange bindings. Its a rabbitmq only feature.
  Exchange queue. To limit the amount of rabbitmq churn we have established a 
  directly bound exchange to your queue. This lets you come on and off the network with 
  little impact to the flow of messages. 

If you are leveraging our interface based messaging with a message class like

.. sourcecode:: csharp

  public class MyMessage : IMessageAA {}

You would see two exchanges 'MyMessage' and 'IMessageAA'. You will also see an
exchange to exchange binding from 'MyMessage' to 'IMessageAA' (from concrete
to the interface). If you subscribet to the concrete type you get a binding to
'MyMessage' if you subscribe to 'IMessageAA' you get a binding to 'IMessageAA'.

NOTE: Why are we doing this?