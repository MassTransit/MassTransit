# Configuring Castle Windsor

The following example shows how to configure a simple Castle Windsor container, and include the bus in the
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
    var container = new WindsorContainer();

    // register each consumer manually
    container.Register(Component.For<YourConsumer>().LifestyleTransient());

    //or use Windsor's excellent scanning capabilities
    container.Register(AllTypes.FromThisAssembly().BasedOn<IConsumer>());

    var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
    {
        cfg.Host(new Uri("rabbitmq://localhost/"), host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        config.ReceiveEndpoint("customer_update_queue", endpoint =>
        {
            endpoint.EnableMessageScope();
            endpoint.LoadFrom(container);
        })
    });

    container.Register(Component.For<IBus, IBusControl>().Instance(busControl));

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
            })
        });

        container.Release(busConfig);

        container.Register(Component.For<IBus, IBusControl>().Instance(busControl));

        busControl.Start();
    }
}
```
