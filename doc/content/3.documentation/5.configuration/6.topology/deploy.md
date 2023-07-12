---
navigation.title: Deploy
---

# Deploy Topology

There are some scenarios, such as when using Azure Functions, where it may be necessary to deploy the topology to the broker separately, without actually starting the service (and thereby consuming messages). To support this, MassTransit has a `DeployTopologyOnly` flag that can be specified when configuring the bus. When used with the `DeployAsync` method, a simple console application can be created that creates all the exchanges/topics, queues, and subscriptions/bindings.

To deploy the broker topology using a console application, see the example below.

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.DeployTopologyOnly = true;

        cfg.ConfigureEndpoints(context);
    });
});
```

`IBusControl` is used to deploy the topology:

```csharp
var busControl = provider.GetRequiredService<IBusControl>();

try
{
    using var source = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    
    await busControl.DeployAsync(source.Token);

    Console.WriteLine("Topology Deployed");
}
catch (Exception ex)
{
    Console.WriteLine("Failed to deploy topology: {0}", ex);
}
```
