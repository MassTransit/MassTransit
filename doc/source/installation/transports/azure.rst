Azure Service Bus
=================



NuGet
'''''

.. sourcecode:: powershell

  nuget Install-Package MassTransit.AzureServiceBus


.. sourcecode:: csharp

  using MassTransit;
  using MassTransit.AzureServiceBusTransport;

  //later

  Bus.Factory.CreateUsingAzureServiceBus(cfg =>{
    //this is in the base MassTransit.AzureServiceBusTransport.dll
  });
