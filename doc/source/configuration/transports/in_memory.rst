In Memory Transport
===================

.. warning::

    The InMemory transport is a great tool for testing, as it doesn't require a message broker
    to be installed or running. It's also very fast. But it isn't durable, and messages are gone
    if the bus is stopped or the process terminates. So, it's generally not a smart option for a
    production system. However, there are places where durability it not important so the cautionary 
    tale is to proceed with caution.


The InMemory transport uses the ``loopback`` address (a holdover from previous version of MassTransit).
The host doesn't matter, and the queue_name is the name of the queue.

    loopback://localhost/queue_name

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
           //configure the endpoint
        })
    });


Sharing Transports
------------------

While it seems weird, and again, it's probably only useful in test scenarios, the transport cache
can be shared across bus instances. To share a transport cache, use the syntax below.

.. sourcecode:: csharp

    var inMemoryTransportCache = new InMemoryTransportCache(Environment.ProcessorCount);

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.SetTransportProvider(inMemoryTransportCache);
    });

As many bus instances as desired can be share the same cache. Again, useful for testing. Not sure I'd
want to use this anywhere else.
