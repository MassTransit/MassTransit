## Azure Service Bus configuration options

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/transports.html

```csharp
Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    cfg.Host(new Uri("sb://localhost"), host =>
    {
        host.OperationTimeout = TimeSpan.FromSeconds(5);
        host.TokenProvider = new ????();
    });
});
```

About Azure Service Bus
