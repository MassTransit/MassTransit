Message scheduling
==================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/scheduling/scheduling-api.html

Why would I want this?

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseMessageScheduler(new Uri("schedulerAddress"));
    });
