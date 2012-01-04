RabbitMQ Configuration Options
""""""""""""""""""""""""""""""

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    // this is the recommended routing strategy, and will call 'sbc.UseRabbitMq()' on its own.
    sbc.UseRabbitMqRouting();
    // other options
  });
  
Alternatively you can use *raw* RabbitMQ routing.

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    // this is the recommended routing strategy, and will call 'sbc.UseRabbitMq()' on its own.
    sbc.UseRabbitMq();
    // other options
  });

Have a look at this table for clarification:
  
``UseRabbitMq`` tells the MassTransit code to use RabbitMQ as the transport. 
This also sets the default serializer to JSON.

``UseRabbitMqRouting`` configures the bus instance to use the default MassTransit
convention based routing for RabbitMq

 ==================================  ================================      ==================================
  Description                        Calling ``UseRabbitMqRouting``        Not calling ``UseRabbitMqRouting``
 ==================================  ================================      ==================================
 IServiceBus.Publish<T>(T, A<C<T>>)  Enables *Polymorphic routing* [#pr]   Will only route non-polymorphicly.
                                     and *Interface-based routing* [#ir]   Will not route on interfaces.
 
 ==================================  ================================      ==================================
 
 
 RabbitMQ with SSL
 -----------------
 
 .. sourceode:: csharp
 
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
 
 
.. [#pr] *Polymorphic Routing* is routing where ``bus.Subscribe<B>( ... )`` would receive both ``class A {}`` and ``class B : A {}`` message.

.. [#ir] *Interface Routing* is routing where ``bus.Subscribe<C>( ... )``  would receive 