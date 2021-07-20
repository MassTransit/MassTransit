# Castle Windsor

Add reference to MassTransit CastleWindsor NuGet package. The following example shows how to configure a simple Castle Windsor container, and include the bus in the
container. The two bus interfaces, `IBus` and `IBusControl`, are included.

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

::: tip
Consumers should not typically depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i>
instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that
messages can be tracked between consumers, and are sent from the proper address.
:::

```csharp
public static void Main(string[] args)
{
    var container = new WindsorContainer();
    container.AddMassTransit(x =>
    {
        // add a specific consumer
        x.AddConsumer<UpdateCustomerAddressConsumer>();

        // add all consumers in the specified assembly
        x.AddConsumers(Assembly.GetExecutingAssembly());

        // add consumers by type
        x.AddConsumers(typeof(ConsumerOne), typeof(ConsumerTwo));

        // add the bus to the container
        x.UsingRabbitMq(cfg =>
        {
            cfg.ReceiveEndpoint("customer_update", ec =>
            {
                // Configure a single consumer
                ec.ConfigureConsumer<UpdateCustomerConsumer>(context);

                // configure all consumers
                ec.ConfigureConsumers(context);

                // configure consumer by type
                ec.ConfigureConsumer(typeof(ConsumerOne), context);
            });

            // or, configure the endpoints by convention
            cfg.ConfigureEndpoints(context);
        });
    });

    IBusControl busControl = container.Kernel.Resolve<IBusControl>();
    busControl.Start();
}
```
