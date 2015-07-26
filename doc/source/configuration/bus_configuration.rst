Bus Configuration
""""""""""""""""""""""""""""

Trying to get multiple applications to talk to each other is not a simple problem.
Often times the biggest is just getting everything configured. With over eight
years of experience in setting up message based systems the developers of MassTransit
have tried to ensure that the defaults cover the majority of the decisions you will
have to make while minimizing the configuration code you have to deal with. We hope
that the options are clear and make sense why you need to select them. Below are
some of the options you have:


Transport Factory Options
'''''''''''''''''''''''''

The first decision is what transport are you going to use? RabbitMQ or Azure Service Bus?
If you don't know which one to choose I suggest reading up on the two and see
which one works better for your environment.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg => {});
    Bus.Factory.CreateUsingRabbitMQ(cfg => {});
    //coming soon
    //Bus.Factory.CreateUsingAzureServiceBus(cfg => {});


.. warning::

    In-Memory is for testing only.

Basic Options
'''''''''''''

The next decision we have to make is what address are we going to listen on? Here
we are using the In-Memory transport and specifing an queue
to receive messages on.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("test_queue", ep =>
        {

        });
    });


The same endpoint but for rabbit mq.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/a_virtual_host"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "test_queue", ep =>
        {

        });
    });

Serializer Options
''''''''''''''''''

This is mostly optional, because the transports will set their preferred defaults, but if you
need to override the default you can using these methods. With the ``SetDefaultSerializer`` you can
provide a custom serializer that you created.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        //receive code options

        cfg.UseBinarySerializer();
        cfg.UseBsonSerializer();
        cfg.UseJsonSerializer();
        cfg.UseXmlSerializer();
    });


