# Configuring Autofac

Autofac is a powerful and fast container, and is well supported by MassTransit. Nested lifetime scopes are used
extensively to encapsulate dependencies and ensure clean object lifetime management. The following examples show the
various ways that MassTransit can be configured, including the appropriate interfaces necessary.

> Requires NuGets `MassTransit`, `MassTransit.AutoFac`, and `MassTransit.RabbitMQ`

<div class="alert alert-info">
<b>Note:</b>
    Consumers should not typically depend upon <i>IBus</i> or <i>IBusControl</i>. A consumer should use the <i>ConsumeContext</i>
    instead, which has all of the same methods as <i>IBus</i>, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.
</div>

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MassTransit;

namespace Example
{
    public class UpdateCustomerAddressConsumer : MassTransit.IConsumer<object>
    {
        public async Task Consume(ConsumeContext<object> context)
        {
            //do stuff
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // register a specific consumer
            builder.RegisterType<<UpdateCustomerAddressConsumer>();

            // just register all the consumers
            builder.RegisterConsumers(Assembly.GetExecutingAssembly());

            builder.Register(context =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("customer_update_queue", ec =>
                    {
                        ec.LoadFrom(context);
                    });
                });

                return busControl;
            })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            var container = builder.Build();

            var bc = container.Resolve<IBusControl>();
            bc.Start();
        }
    }
}
```

## Using a Module

Autofac modules are great for encapsulating configuration, and that is equally true when using MassTransit. An example of
using modules with Autofac is shown below.

```csharp
class ConsumerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UpdateCustomerAddressConsumer>();

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
        ec.LoadFrom(context);

        // loading saga state machines
        ec.LoadStateMachineSagas(context);
    })
});
```

## Saga persistence

Below you find samples of how to register different saga persistence implementations with Autofac.

> Saga repositories must be registered as singletons (`SingleInstance()`).

### NHibernate

For NHibernate you can scan an assembly where your saga instance mappings are defined to find
the mapping classes, and then give the list of mapping types as a parameter to the session factory provider.

Then, you instruct Autofac to use the session factory provider to get the `ISession` instance. 
NHibernate saga repository is then registered as generic and since it only uses the `ISession`, 
everything will just work.

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

Entity Framework saga repository needs to have a context factory as a constructor parameter.
This factory just returns a `DbContext` instance, which should have the information about
the saga instance class mapping.

When using the `SagaDbContext<TSaga, TSagaClassMapping>`, you need to register each repository
separately like this:

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

The example above uses the assembly scanning `DbContext` implementation, which
you can find in [this gist](https://gist.github.com/alexeyzimarev/34542645ff8f27550d0679c7cb696111).