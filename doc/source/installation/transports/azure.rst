RabbitMQ
========

A high volume queueing platform

See https://www.rabbitmq.com/download.html for installing rabbitmq.

NuGet
'''''

  nuget Install-Package MassTransit.AzureServiceBus

.. sourcecode:: csharp

  using MassTransit;
  using MassTransit.AzureServiceBusTransport;

  //later

  Bus.Factory.CreateUsingAzureServiceBus(cfg =>{
    //this is in the base MassTransit.AzureServiceBusTransport.dll
  });
