Configuring Ninject
===================

The following example shows how to configure a simple Ninject container, and include the bus in the
container. The two bus interfaces, ``IBus`` and ``IBusControl``, are included.

.. note::

    Consumers should not typically depend upon ``IBus`` or ``IBusControl``. A consumer should use the ``ConsumeContext``
    instead, which has all of the same methods as ``IBus``, but is scoped to the receive endpoint. This ensures that
    messages can be tracked between consumers, and are sent from the proper address.

.. sourcecode:: csharp

    public static void main(string[] args) 
    {
        var kernel = new StandardKernel();

        kernel.Bind<UpdateCustomerAddressConsumer>().ToSelf();
            
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

.. note::

    The behavior with Ninject is slightly different, in that the current AppDomain types are checked against the
    container and if any consumer types are registered, they are resolved from the container. The unit tests pass, and
    it works, but just be aware that container metadata is not being used to support this feature. There is some history
    on this, found at the `Ninject issue`_.

.. _Ninject issue: https://github.com/ninject/ninject/issues/35

