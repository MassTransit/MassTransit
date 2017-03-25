Transaction middleware
""""""""""""""""""""""

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/advanced/transactions.html

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
