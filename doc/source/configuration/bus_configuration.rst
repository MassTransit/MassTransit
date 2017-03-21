Bus configuration
=================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/configuration.html

Trying to get multiple applications to talk to each other is not a simple problem.
Often times the biggest is just getting everything configured. With over eight
years of experience in setting up message based systems the developers of MassTransit
have tried to ensure that the defaults cover the majority of the decisions you will
have to make while minimizing the configuration code you have to deal with. We hope
that the options are clear and make sense why you need to select them. Below are
some of the options you have:


Selecting a message transport
"""""""""""""""""""""""""""""

The first decision is what transport are you going to use? RabbitMQ or Azure Service Bus?
If you don't know which one to choose I suggest reading up on the two and see
which one works better for your environment.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg => {});
    Bus.Factory.CreateUsingRabbitMQ(cfg => {});
    Bus.Factory.CreateUsingAzureServiceBus(cfg => {});


.. warning::

    The InMemory transport is a great tool for testing, as it doesn't require a message broker
    to be installed or running. It's also very fast. But it isn't durable, and messages are gone
    if the bus is stopped or the process terminates. So, it's generally not a smart option for a
    production system. However, there are places where durability it not important so the cautionary
    tale is to proceed with caution.


Specifying a Host
"""""""""""""""""

Once the transport has been selected, the message host(s) must be configured. The host settings are
transport specific, so the available options will vary. For instance, the InMemory transport does not
require any configuration, because it's, well, in memory.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
    });

RabbitMQ Specific
'''''''''''''''''

For RabbitMQ, a URI specifying the host (and virtual host, default is /) should be provided, along
with additional configuration for the username/password, as well as options on the transport.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/a_virtual_host"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });

You can also specify the nodes in a cluster to use the RabbitMQ driver's built-in failover capabilities. 
When a connection is interrupted, a new node will be selected and connected to. 

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://myclustername/a_virtual_host"), h =>
        {
            h.Username("guest");
            h.Password("guest");
            h.UseCluster(c =>
            {
                c.Node("rabbit1");
                c.Node("rabbit2");
                c.Node("rabbit3");
            });
        });
    });
	
.. note:: The ``myclustername`` value specified in the ``Uri`` is strictly cosmetic when using clustering in this way. The actual Uri 
          will be rewritten to use a node hostname from the cluster node list.

Azure Specific
''''''''''''''

For Azure Service Bus, a URI specifying the namespace should be provided, along with the
``TokenProvider`` for a token with **manage** permissions.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
    {
        var host = cfg.Host(new Uri("sb://my-namespace.servicebus.windows.net/"), h =>
        {
            h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("KeyName", "keyvalue");
        });
    });


Specifying a receive endpoint
"""""""""""""""""""""""""""""

Once the hosts are configured, any number of receive endpoints can be configured. No receive endpoints
are required, a send/publish only bus is totally legit. An example of configuring a RabbitMQ host with
a single receive endpoint is shown below.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "service_queue", ep =>
        {
        });
    });


Selecting an outbound message serializer
""""""""""""""""""""""""""""""""""""""""

By default, outbound messages are serialized using JSON and inbound messages that are in JSON, BSON,
or XML can be deserialized. To use a different outbound message format, the default serializer can be
changed. If a custom serializer has been created, use the ``SetDefaultSerializer`` extension to specify
the factory methods for the custom serializer.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseBinarySerializer();
        cfg.UseBsonSerializer();
        cfg.UseJsonSerializer();
        cfg.UseXmlSerializer();
    });
