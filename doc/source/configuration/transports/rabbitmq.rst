RabbitMQ Configuration Options
""""""""""""""""""""""""""""""

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    sbc.UseRabbitMq();         //1
    sbc.UseRabbitMqRouting();  //2
  });

``UseRabbitMq`` tells the MassTransit code to use RabbitMQ as the transport. 
This also sets the default serializer to JSON.

``UseRabbitMqRouting`` configures the bus instance to use the default MassTransit
convention based routing for RabbitMq