# Configuring StructureMap

The following example shows how to configure a simple StructureMap container, and include the bus in the
container. The two bus interfaces, `IBus` and `IBusControl`, are included.

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

<div class="alert alert-info">
<b>Note:</b>
    Consumers should not typically depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i>
    instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.
</div>

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

## Using a Registry

In larger applications, the use of registries is common. In fact, this is one of the better ways to use MassTransit with
StructureMap, as it ensures consistent usage across services.

```csharp
class BusRegistry : Registry
{
    public BusRegistry()
    {
        For<IBusControl>(new SingletonLifecycle())
            .Use(context => Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("customer_update_queue", e => e.LoadFrom(context));
            }));
        Forward<IBusControl, IBus>();
    }
}

class ConsumerRegistry : Registry
{
    public ConsumerRegistry()
    {
        ForConcreteType<UpdateCustomerAddressConsumer>();

        For<ICustomerRegistry>()
            .Use<SqlCustomerRegistry>();
    }
}

public void CreateContainer()
{
    _container = new Container(x =>
    {
        x.AddRegistry(new BusRegistry());
        x.AddRegistry(new ConsumerRegistry());
    });
}
```