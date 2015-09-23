Bus Tuning
==========

There are a number of settings which can be configured, not including the available
middleware components. Many of them are specific to the transport, but some are 
common.

In Memory Transport
'''''''''''''''''''

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
    	// sets the number of threads available to the 
    	// in-memory message dispatcher
        cfg.TransportConcurrencyLimit = 2;
    });


RabbitMQ Transport
'''''''''''''''''

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMQ(cfg =>
    {
    	// overrides the system-generated temporary bus name
        cfg.BusQueueName = "unique_queue_name_for_the_bus";
    });
