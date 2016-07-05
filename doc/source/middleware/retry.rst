Retry
=====

The retry policy can be specified at the bus level, as well as the endpoint and consumer level.
The policy closest to the exception is the policy that is used.

To configure the default retry policy for the entire bus.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseRetry(Retry.None);
    });

To configure the retry policy for a receive endpoint.

.. sourcecode:: csharp

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

Retry Filters
------------

A retry policy can also be configured for a specific set of exceptions, using a filter.

* Except
* Selected
* All
* Filter
