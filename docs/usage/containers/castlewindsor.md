# Configuring Castle Windsor

Add reference to MassTransit CastleWindsor NuGet package. The following example shows how to configure a simple Castle Windsor container, and include the bus in the
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

    IBusControl busControl = container.Kernel.Resolve<IBusControl>();
    busControl.Start();
}
```

## Using a Windsor Installer

It is sometimes useful to put the initialization into an installer, so that it can use existing components from
the container for configuration.

```csharp
public class MassTransitInstaller :
    IWindsorInstaller
{
    public void Install(IWindsorContainer container,
        Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
        var busConfig = container.Resolve<BusConfiguration>();

        var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
        {
            config.Host(busConfig.HostAddress, host =>
            {
                host.Username(busConfig.Username);
                host.Password(busConfig.Password);
            });

            config.ReceiveEndpoint(busConfig.QueueName, endpoint =>
            {
                endpoint.EnableMessageScope();
                
                endpoint.LoadFrom(container);
                
                // Above method works but it is deprecated, instead below method should be used to get Consumer from container.
                endPoint.Consumer<YourConsumer>(container.Kernel);
            })
        });

        container.Release(busConfig);

        container.Register(Component.For<IBus, IBusControl>().Instance(busControl));

        busControl.Start();
    }
}
```
