RabbitMQ Configuration Options
""""""""""""""""""""""""""""""

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
    sbc.UseRabbitMq(r =>
                    {
                        r.ConfigureHost(new Uri("rabbitmq://hostname/vhost/queue"), h =>
                        {
                            h.SetUsername("username");
                            h.SetPassword("password");
                        });
                    });

    // other options
  });

``UseRabbitMq`` tells the MassTransit code to use RabbitMQ as the transport.
This also sets the default serializer to JSON.


 Routing Topology
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

 .. sourceode:: csharp

  ServiceBusFactory.New(c =>
      {
          c.ReceiveFrom(inputAddress);
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


.. [#pr] *Polymorphic Routing* is routing where ``bus.Subscribe<A>( ... )`` would receive both ``class A {}`` and ``class B : A {}`` message.

.. [#ir] *Interface Routing* is routing where ``bus.Subscribe<IMessage>( ... )``  would receive both ``class A : IMessage {}`` and ``class B : IMessage {}``

Concurrent Consumers with RabbitMQ
----------------------------------

When using concurrent consumers with rabbitmq you need to set both the
number of concurrent consumers to X and you need to set the prefetch. This is a
quick note in the docs to remind people of this. A bit of detail is in the link
below.

https://groups.google.com/forum/#!topic/masstransit-discuss/-0F7jswXIso
