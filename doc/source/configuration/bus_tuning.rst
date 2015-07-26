Bus Tuning
""""""""""

How to tune the bus

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ConcurrencyLimit = 2;
        cfg.SetDefaultTransactionTimeout(5.Minutes());

    });
