In-Memory transport
===================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/transports.html#in-memory-transport

.. warning::

    The in-memory transport is designed for use within a single process only.
    It is not possible to use the in-memory transport to communicate between multiple processes
    (even if they are on the same machine). By the way, it is possible to share the same
    in-memory transport with multiple bus instances *within the same process* by configuring
    the transport provider using InMemoryTransportCache (see below).


.. warning::

    The InMemory transport is a great tool for testing, as it doesn't require a message broker
    to be installed or running. It's also very fast. But it isn't durable, and messages are gone
    if the bus is stopped or the process terminates. So, it's generally not a smart option for a
    production system. However, there are places where durability it not important so the cautionary
    tale is to proceed with caution.


The In-Memory transport uses the ``loopback`` address (a holdover from previous version of MassTransit).
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


Sharing transports
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
