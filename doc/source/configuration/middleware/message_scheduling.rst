Message Scheduling
==================

Why would I want this?

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseMessageScheduler(new Uri("schedulerAddress"));
    });
