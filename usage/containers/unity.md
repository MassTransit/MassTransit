# Configuring Unity

The following example shows how to configure a simple Unity container, and include the bus in the
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
    var container = new UnityContainer(); 

    // register each consumer
    container.RegisterType<UpdateCustomerAddressConsumer>(new ContainerControlledLifetimeManager());

    // scan for types
    var consumerTypes = new TypeFinder().FindTypesWhichImplement(typeof(IConsumer));
    foreach (var consumerType in consumerTypes)
    {
        container.RegisterType(consumerType, new ContainerControlledLifetimeManager());
    }

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        sbc.ReceiveEndpoint("customer_update_queue", ec =>
        {
            ec.LoadFrom(container);
        })
    });
    
    container.RegisterInstance<IBusControl>(busControl);
    container.RegisterInstance<IBus>(busControl);

    busControl.Start();
}
```