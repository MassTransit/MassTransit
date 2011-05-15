When messages are published, how do they get there?
===================================================

Ok, so when you publish a messages ``bus.Publish(yourMessage)`` your message
is then routed through the MassTransit infrastructure and delivered
to your consumers. How that happens depends a little on what transport you
are using. (See Below)

However, both transports do their message routing dynamically. There is, 
by default, no static way to set routes. The routes are all interpreted
at run time and deployed to the system.

MSMQ
----

The message is routed to an internal construct called the `Outbound Pipeline`
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
