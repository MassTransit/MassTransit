Some Key Terms
==============

Things that consume messages
""""""""""""""""""""""""""""

These are things in your application that will at some point be handed a message
by the MasTransit infrastructure. Simplistically these are going to be
``void Consume(SomeMessage msg)`` type methods. Since that is all the infrastructure
needs to put everything together, we have given you several options.

Handlers
''''''''

Handlers are the most atomic unit of message processing. There are nothing more
than an anonymous method that can take a message and do some processing on it.

Instances
'''''''''

This is an actual instance of a class that can process a method. Instances, like
Consumers, need to follow the interfaces ``IConsumer`` interfaces
(:doc:`interfaces`).


Consumers
'''''''''

Consumers are going to be classes that implement one of the ``IConsumer``
interfaces (:doc:`interfaces`).  The bus will create an instance
of the class, and then push the message into the registered consumes method.


Sagas
'''''

Sagas are most complex of the message consumers, as the can consume multiple
types of messages and are typically used to orchestrate several messages
in a protocol. Sagas are designed as state machines, and are typically
correlated on a ``Guid``.

Things that move messages
"""""""""""""""""""""""""

Endpoints

Transports

Addresses

Serializers

The Function of the Bus
"""""""""""""""""""""""

Abstraction for sending
