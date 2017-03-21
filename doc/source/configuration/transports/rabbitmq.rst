RabbitMQ configuration options
""""""""""""""""""""""""""""""

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/transports.html

.. warning::

    This page has not been updated yet.

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri("rabbitmq://a-machine-name/a-virtual-host"), host =>
        {
            host.Username("username");
            host.Password("password");
        });
    });


Routing topology
----------------

About the RabbitMQ routing topology in place with MassTransit.

- networks are segregated by vhosts
- we generate an exchange for each queue so that we can do direct sends to the queue. it is bound as a fanout exchange
- for each message published we generate series of exchanges that go from concrete class to each of its subclass / interfaces these are linked together from most specific to least specific. This way if you subscribe to the base interface you get all the messages. or you can be more selective. all exchanges in this situation are bound as fanouts.
- the subscriber declares his own queue and his queue exchange - he then also declares/binds his exchange to each of the message type exchanges desired
- the publisher discovers all of the exchanges needed for a given message, binds them all up and then pushes the message into the most specific queue letting RabbitMQ do the fanout for him. (One publish, multiple receivers!)
- control queues are exclusive and auto-delete - they go away when you go away and are not shared.
- we also lose the routing keys. WIN!


RabbitMQ with SSL
-----------------

.. sourcecode:: csharp

    ServiceBusFactory.New(c =>
    {
        c.ReceiveFrom(inputAddress);
        c.UseRabbitMqRouting();
        c.UseRabbitMq(r =>
        {
            r.ConfigureHost(inputAddress, h =>
            {
                h.UseSsl(s =>
                {
                    s.SetServerName(System.Net.Dns.GetHostName());
                    s.SetCertificatePath("client.p12");
                    s.SetCertificatePassphrase("Passw0rd");
                });
            });
        });
    });

You will need to configure RabbitMQ to support SSL also http://www.rabbitmq.com/ssl.html.


RabbitMQ with CloudAMQP
-----------------------

It is not necessary to set SSL specific configuration parameters in order to connect using SSL. What is required is specifying the appropriate SSL specific port (usually 5671 as opposed to the non-ssl port for RabbitMQ which is typically 5672).

.. sourcecode:: csharp

    ServiceBusFactory.New(c =>
    {
        c.UseRabbitMq(r =>
        {
            r.ConfigureHost(host, port, virtualHost, h =>
            {
                h.UseSsl(s =>{ });
            });
        });
    });

	
.. [#pr] *Polymorphic Routing* is routing where ``bus.Subscribe<B>( ... )`` would receive both ``class A {}`` and ``class B : A {}`` message.

.. [#ir] *Interface Routing* is routing where ``bus.Subscribe<C>( ... )``  would receive
