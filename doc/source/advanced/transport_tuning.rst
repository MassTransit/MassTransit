Tuning the transport
====================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit

There are a number of settings which can be configured, not including the available
middleware components. Many of them are specific to the transport, but some are
common.

In-Memory transport
-------------------

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
    	// sets the number of threads available to the
    	// in-memory message dispatcher
        cfg.TransportConcurrencyLimit = 2;
    });


Sharing transports
~~~~~~~~~~~~~~~~~~

While it seems weird, and it's probably only useful in test scenarios, the transport cache
can be shared across bus instances. To share a transport cache, use the syntax below.

.. sourcecode:: csharp

    var inMemoryTransportCache = new InMemoryTransportCache(Environment.ProcessorCount);

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.SetTransportProvider(inMemoryTransportCache);
    });

As many bus instances as desired can be share the same cache. Again, useful for testing. Not sure I'd
want to use this anywhere else.


RabbitMQ transport
------------------


.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMQ(cfg =>
    {
    	// overrides the system-generated temporary bus name
        cfg.BusQueueName = "unique_queue_name_for_the_bus";
    });
