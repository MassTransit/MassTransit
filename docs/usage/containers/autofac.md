# Configuring Autofac

Autofac is a powerful and fast container, and is well supported by MassTransit. Nested lifetime scopes are used extensively to encapsulate dependencies and ensure clean object lifetime management. The following examples show the various ways that MassTransit can be configured, including the appropriate interfaces necessary.

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

> Requires NuGets `MassTransit`, `MassTransit.AutoFac`, and `MassTransit.RabbitMQ`

<div class="alert alert-info">
<b>Note:</b>
    Consumers should not depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i> instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that messages can be tracked between consumers and are sent from the proper address.
</div>

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
                        ec.ConsumerConsumer(typeof(ConsumerOne));
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

## Using Nested Container
MassTransit and Autofac give you an ability to reconfigure your container based on Consumer. It could be very powerful when you have different way to resolve your services depends on Consumer's type. It could be very helpful to build Multitenant applications.

```csharp
builder.RegisterType<HttpTenantProvider>().As<ITenantProvider>();

builder.Register(context =>
{
    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        //configuration
        cfg.ReceiveEndpoint("name", ec => 
        {
            ec.Consumer<YourConsumer>(context, "scope_name", (c, context) =>
            {
                c.RegisterInstance(new ConsumerTenantProvider(context)).As<ITenantProvider>();
                //other configuration
            });
        })
    }
}
```

## Using a Module

Autofac modules are great for encapsulating configuration, and that is equally true when using MassTransit. An example of using modules with Autofac is shown below.

```csharp
class ConsumerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // requires that AddMassTransit has been called prior to this 
        // module being loaded, maybe, I don't know for sure that this
        // is even possible
        builder.AddConsumer<UpdateCustomerAddressConsumer>();

        builder.RegisterType<SqlCustomerRegistry>()
            .As<ICustomerRegistry>();
    }
}

class BusModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(context =>
        {
            var busSettings = context.Resolve<BusSettings>();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(busSettings.HostAddress, h =>
                {
                    h.Username(busSettings.Username);
                    h.Password(busSettings.Password);
                });

                cfg.ReceiveEndpoint(busSettings.QueueName, ec =>
                {
                    ec.LoadFrom(context);
                })
            });
        })
            .SingleInstance()
            .As<IBusControl>()
            .As<IBus>();
    }
}

public IContainer CreateContainer()
{
        var builder = new ContainerBuilder();

    builder.RegisterModule<BusModule>();
    builder.RegisterModule<ConsumerModule>();

    return builder.Build();
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

## Registering State Machine Sagas

By using an additional package `MassTransit.Automatonymous.Autofac` you can also register state machine sagas:

```csharp
var builder = new ContainerBuilder();
// register everything else

// register saga state machines, assuming Saga1 and Saga2 are in different assemblies
builder.RegisterStateMachineSagas(typeof(Saga1).Assembly, typeof(Saga2).Assembly);

// registering saga state machines from current assembly
builder.RegisterStateMachineSagas(Assembly.GetExecutingAssembly());

// do not forget registering saga repositories
// see examples below
```

and load them from a contained when configuring the bus.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(busSettings.HostAddress, h =>
    {
        h.Username(busSettings.Username);
        h.Password(busSettings.Password);
    });

    cfg.ReceiveEndpoint(busSettings.QueueName, ec =>
    {
        // loading consumers
        ec.ConfigureConsumers(context);

        // loading saga state machines
        ec.ConfigureSagas(context);
    })
});
```

## Saga persistence

Below you find samples of how to register different saga persistence implementations with Autofac.

> Saga repositories must be registered as singletons (`SingleInstance()`).

### NHibernate

For NHibernate you can scan an assembly where your saga instance mappings are defined to find the mapping classes, and then give the list of mapping types as a parameter to the session factory provider.

Then, you instruct Autofac to use the session factory provider to get the `ISession` instance. NHibernate saga repository is then registered as generic and since it only uses the `ISession`, everything will just work.

```csharp
var mappings = mappingsAssembly
    .GetTypes()
    .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
        (t.BaseType.GetGenericTypeDefinition() == typeof(SagaClassMapping<>) ||
        t.BaseType.GetGenericTypeDefinition() == typeof(ClassMapping<>)))
    .ToArray();
builder.Register(c => new SqlServerSessionFactoryProvider(connString, mappings).GetSessionFactory())
    .As<ISessionFactory>()
    .SingleInstance();
builder.RegisterGeneric(typeof(NHibernateSagaRepository<>))
    .As(typeof(ISagaRepository<>));
```

### Entity Framework

Entity Framework saga repository needs to have a context factory as a constructor parameter. This factory just returns a `DbContext` instance, which should have the information about the saga instance class mapping.

When using the `SagaDbContext<TSaga, TSagaClassMapping>`, you need to register each repository separately like this:

```csharp
builder.Register(c => new EntityFrameworkSagaRepository<MySaga>(
    () => new SagaDbContext<MySaga, MySagaMapping>(connectionString)))
        .As<ISagaRepository<MySaga>>().SingleInstance();
```

You can use your own context implementation and register the repository as generic like this:

```csharp
builder.Register(c => new AssemblyScanningSagaDbContext(typeof(MySagaMapping).Assembly,
    connectionString).As<DbContext>();
builder.RegisterGeneric(typeof(EntityFrameworkSagaRepository<>))
    .As(typeof(ISagaRepository<>))
    .SingleInstance();
builder.RegisterStateMachineSagas(typeof(MySaga).Assembly);
```

The example above uses the assembly scanning `DbContext` implementation, which you can find in [this gist](https://gist.github.com/alexeyzimarev/34542645ff8f27550d0679c7cb696111).