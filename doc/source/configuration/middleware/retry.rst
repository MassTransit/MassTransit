Retry
=====

At the bus level

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        //the default
        cfg.UseRetry(Retry.None);
    });

and at the endpoint level

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("inbound", ep =>
        {
            ep.UseRetry(Retry.Immediate(5));
        });
    });

Retry Policies
--------------

* None
* Immediate
* Intervals
* Exponential
* Incremental
* Except
* Selected
* All
* Filter
