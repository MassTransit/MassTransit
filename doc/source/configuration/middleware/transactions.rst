Transaction Middleware
""""""""""""""""""""""""

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseTransaction(trx =>
        {
            trx.IsolationLevel = IsolationLevel.ReadCommitted;
            trx.Timeout = TimeSpan.FromSeconds(5);
        });

    });
