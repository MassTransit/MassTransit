# Configuring Simple Injector

Add reference to MassTransit.SimpleInjector NuGet package. The following example shows how to configure a  SimpleInjector container, and include the bus in the
container. The two bus interfaces, `IBus` and `IBusControl`, are included.

<div class="alert alert-info">
<b>Note:</b>
    Consumers should not typically depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i>
    instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.
</div>

```csharp
public static void Main(string[] args)
{
    container = new Container();
    container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

    container.AddMassTransit(x =>
    {
        // add a specific consumer
        x.AddConsumer<UpdateCustomerAddressConsumer>();

        // add all consumers in the specified assembly
        x.AddConsumers(Assembly.GetExecutingAssembly());

        // add consumers by type
        x.AddConsumers(typeof(ConsumerOne), typeof(ConsumerTwo));

        // add the bus to the container
        x.AddBus(() => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host(new Uri("rabbitmq://localhost"), hostConfigurator =>
            {
            });

            cfg.ReceiveEndpoint("customer_update", ec =>
            {
                // Configure a single consumer
                ec.ConfigureConsumer<UpdateCustomerConsumer>(container);

                // configure all consumers
                ec.ConfigureConsumers(container);

                // configure consumer by type
                ec.ConfigureConsumer(typeof(ConsumerOne), container);
            });

            // or, configure the endpoints by convention
            cfg.ConfigureEndpoints(container);
        }));
    });

    IBusControl busControl = container.GetInstance<IBusControl>();
    busControl.Start();
}
```