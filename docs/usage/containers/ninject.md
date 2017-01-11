# Configuring Ninject

The following example shows how to configure a simple Ninject container, and include the bus in the
container. The two bus interfaces, `IBus` and `IBusControl`, are included.

<div class="alert alert-info">
<b>Note:</b>
    Consumers should not typically depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i>
    instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.
</div>

```csharp
public static void main(string[] args) 
{
    var kernel = new StandardKernel();

    // register a specific consumer
    kernel.Bind<UpdateCustomerAddressConsumer>().ToSelf();
    
    // just register all the consumers using Ninject.Extensions.Conventions
    kernel.Bind(x =>
    {
        x.FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IConsumer>()
            .BindToSelf();
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
            ec.LoadFrom(kernel);
        })
    });
    
    kernel.Bind<IBus>()
        .ToProvider(new CallbackProvider<IBus>(x => x.Kernel.Get<IBusControl>()));
    
    busControl.Start();
}
```

<div class="alert alert-info">
<b>Note:</b>
    The behavior with Ninject is slightly different, in that the current AppDomain types are checked against the
    container and if any consumer types are registered, they are resolved from the container. The unit tests pass, and
    it works, but just be aware that container metadata is not being used to support this feature. There is some history
    on this, found at the <a href="https://github.com/ninject/ninject/issues/35">Ninject issue</a>.
</div>