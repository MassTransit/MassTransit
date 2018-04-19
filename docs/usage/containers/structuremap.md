# Configuring StructureMap

The following example shows how to configure a simple StructureMap container, and include the bus in the
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
    var container = new Container(cfg =>
    {
        // register each consumer
        cfg.ForConcreteType<UpdateCustomerAddressConsumer>();

        //or use StructureMap's excellent scanning capabilities
    });

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        sbc.ReceiveEndpoint("customer_update_queue", ec =>
        {
            // if only one consumer in the consumer for this queue
            ec.LoadFrom(container);

            // otherwise, be smart, register explicitly
            ec.Consumer<UpdateCustomerConsumer>(container);
        })
    });

    container.Configure(cfg =>
    {
        For<IBusControl>()
            .Use(busControl);
        Forward<IBusControl, IBus>();
    });

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