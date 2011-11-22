When messages are published, how do they get there?
===================================================

What actually happens when you call ``bus.Publish(yourMessage)``? The process
itself is pretty simple, but it does depend on your transport. The first thing
to know is that MassTransit has something called an 'Outbound Pipeline'. Messages
enter into the outbound pipeline to be sent to the actual transport. Once they
hit the transport, the messages leave your .Net process and enter the process
of the transport infrastructure.

It should be noted that MassTransit prefers a dynamic routing approach. This 
means that when you call ``Subscribe`` methonds, this information will get
routed to all of the nodes on the network immediately. There is nothing to configure
other than your choice of routing provider, which is usually tied to the transport
choice at the moment.

We do realize that some prefer a more static approach to this process, so there
is an extension to the system to allow this called static routing. You will HAVE
to manually set up all subscriptions on ALL endpoints for this to work. Its a lot
of manual monkey work in my opinion, but a scalpel you shall have.

MSMQ Runtime Services & Multicast
---------------------------------

The message is routed through an internal construct called the `Outbound Pipeline`
the pipeline is a tree-like structure with one input point and many output
points. When a message comes in it goes through the pipeline logic, and then
hits an outbound transport. The outbound transport then sends the 
message directly to the subscriber. It is the subscription service that 
keeps all of the outbound and inbound pipelines in order.

RabbitMQ
--------

Because RabbitMQ has a much, much better routing system, instead of trying
to redo that work for RabbitMQ, we instead configure the RabbitMQ system's 
routing primitives to achieve the same thing that we have done in MSMQ
and the Outbound/Inbound pipelines.

So a message is routed straight to the correct RabbitMQ Exchange. The internal
workings of MassTransit make sure to configure RabbitMQ exchanges and bindings 
to implement the MassTransit pattern of routing. This means MT can make one call
to RMQ, and let RabbitMQ deal with it from there.