When messages are published, how do they get there?
===================================================

What actually happens when you call ``bus.Publish(yourMessage)``? The process
itself is pretty simple, but it does depend on your transport. The first thing
to know is that MassTransit has something called an 'Outbound Pipeline'. Messages
enter into the outbound pipeline to be sent to the actual transport. Once they
hit the transport, the messages leave your .Net process and enter the process
of the transport infrastructure.

.. note::

	Transports that are currently supported are RabbitMQ. There are
	also community contributed ActiveMQ and Azure Service Bus transports.
	Finally there is a non-production In-Memory Loopback which we use for
	testing.

MassTransit prefers a dynamic routing model, we do not enforce a static routing
model, however it can be achieved with the 'static routing bits'. What this
means is that when you call the ``Subscribe`` methods, MassTransit is going to
setup the necessary routing for you.

The 'necessary' routing will vary by transport that you use.

.. note::

	We do realize that some prefer a more static approach to this process, so there
	is an extension to the system to allow this called static routing. You will HAVE
	to manually set up all subscriptions on ALL endpoints for this to work. Its a lot
	of manual monkey work in my opinion, but a scalpel you shall have.

With that out of the way lets discuss how the transport affects routing.

RabbitMQ
--------

RabbitMQ is a powerful and flexible platform that has robust routing capabilities.
When you subscribe to a message, we dynamically setup bindings and exchanges
in RabbitMQ that match the type names of the messages.

So a message is routed straight to the correct RabbitMQ Exchange. The internal
workings of MassTransit make sure to configure RabbitMQ exchanges and bindings
to implement the MassTransit pattern of routing. This means MT can make one call
to RMQ, and let RabbitMQ deal with it from there.
