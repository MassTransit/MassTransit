# Configuring Lamar

MassTransit supports Lamar, the successor to StructureMap, created by Jeremy D. Miller.

> Support requires an additional NuGet package, `MassTransit.Lamar`, which is available using [NuGet](https://www.nuget.org/packages/MassTransit.Lamar). For state machine support, add the `MassTransit.Automatonymous.Lamar` [package](https://www.nuget.org/packages/MassTransit.Automatonymous.Lamar).

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

```csharp
public static void Main(string[] args)
{
    var container = new Container(cfg =>
    {
        cfg.AddMassTransit(x =>
        {
            // add a specific consumer
            x.AddConsumer<UpdateCustomerAddressConsumer>();

            // add all consumers in the specified assembly
            x.AddConsumers(Assembly.GetExecutingAssembly());

            // add consumers by type
            x.AddConsumers(typeof(ConsumerOne), typeof(ConsumerTwo));

            // add the bus to the container, may need to create Local function
            x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost/");

                cfg.ReceiveEndpoint("customer_update", ec =>
               {
                    // Configure a single consumer
                    ec.ConfigureConsumer<UpdateCustomerConsumer>(context);

                    // configure all consumers
                    ec.ConfigureConsumers(context);

                    // configure consumer by type
                    ec.ConsumerConsumer(typeof(ConsumerOne), context);
                });

                // or, configure the endpoints by convention
                cfg.ConfigureEndpoints(context);
            });
        });
    });

    IBusControl busControl = container.GetInstance<IBusControl>();
    busControl.Start();
}
```
