# Autofac

Autofac is a powerful and fast container, and is well supported by MassTransit. Nested lifetime scopes are used extensively to encapsulate dependencies and ensure clean object lifetime management. The following examples show the various ways that MassTransit can be configured, including the appropriate interfaces necessary.

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

> Requires NuGets `MassTransit`, `MassTransit.AutoFac`, and `MassTransit.RabbitMQ`

::: tip
Consumers should not depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i> instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that messages can be tracked between consumers and are sent from the proper address.
:::

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MassTransit;

namespace Example
{
    public class UpdateCustomerAddressConsumer : 
        MassTransit.IConsumer<UpdateCustomerAddress>
    {
        public Task Consume(ConsumeContext<UpdateCustomerAddress> context)
        {
            //do stuff
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                // add a specific consumer
                x.AddConsumer<UpdateCustomerAddressConsumer>();

                // add all consumers in the specified assembly
                x.AddConsumers(Assembly.GetExecutingAssembly());

                // add consumers by type
                x.AddConsumers(typeof(ConsumerOne), typeof(ConsumerTwo));

                // add the bus to the container
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost");

                    cfg.ReceiveEndpoint("customer_update", ec =>
                    {
                        // Configure a single consumer
                        ec.ConfigureConsumer<UpdateCustomerConsumer>(context);

                        // configure all consumers
                        ec.ConfigureConsumers(context);

                        // configure consumer by type
                        ec.ConfigureConsumer(typeof(ConsumerOne));
                    });

                    // or, configure the endpoints by convention
                    cfg.ConfigureEndpoints(context);
                });
            });
            var container = builder.Build();

            var bc = container.Resolve<IBusControl>();
            bc.Start();
        }
    }
}
```
