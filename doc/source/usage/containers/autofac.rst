Configuring Autofac
===================

Autofac is a powerful and fast container, and is well supported by MassTransit. Nested lifetime scopes are used
extensively to encapsulate dependencies and ensure clean object lifetime management. The following examples show the 
various ways that MassTransit can be configured, including the appropriate interfaces necessary.

.. note::

    Consumers should not typically depend upon ``IBus`` or ``IBusControl``. A consumer should use the ``ConsumeContext``
    instead, which has all of the same methods as ``IBus``, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.

.. sourcecode:: csharp

    public static void main(string[] args) 
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

                sbc.ReceiveEndpoint("customer_update_queue", ec =>
                {
                    ec.LoadFrom(context);
                })
            });
        })
            .SingleInstance()
            .As<IBusControl>()
            .As<IBus>();

        var container = builder.Build();

        var busControl = container.Resolve<IBusControl>();        
        busControl.Start();
    }


Using a Module
--------------

Autofac modules are great for encapsulating configuration, and that is equally true when using MassTransit. An example of
using modules with Autofac is shown below.

.. sourcecode:: csharp

    class ConsumerModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateCustomerAddressConsumer>();

            builder.RegisterType<SqlCustomerRegistry>()
                .As<ICustomerRegistry>();
        }
    }

    class BusModule :
        Module
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

                    sbc.ReceiveEndpoint(busSettings.QueueName, ec =>
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

