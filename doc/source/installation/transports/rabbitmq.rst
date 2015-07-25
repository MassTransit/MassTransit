RabbitMQ
========

A high volume queueing platform

See https://www.rabbitmq.com/download.html for installing rabbitmq.

NuGet
'''''

  nuget Install-Package MassTransit.RabbitMQ


.. sourcecode:: csharp

  using MassTransit;
  using MassTransit.RabbitMqTransport;

  //later

  Bus.Factory.CreateUsingRabbitMq(cfg =>{
    //this is in the base MassTransit.RabbitMqTransport.dll
  });
