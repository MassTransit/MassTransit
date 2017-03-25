Using the latest filter
=======================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/advanced/middleware/latest.html

The latest filter is pretty simple, it keeps track of the latest message received by the pipeline and makes that
value available. It seems pretty simple, and it is, but it is actually useful in metrics and monitoring scenarios.

.. note::

    This filter is actually usable to capture any context type on any pipe, so you know.


To add a latest to a receive endpoint:

.. sourcecode:: csharp

    ILatestFilter<ConsumeContext<Temperature>> tempFilter;

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
        {
            e.Handler<Temperature>(context => Task.FromResult(true), x =>
            {
                x.UseLatest(x => x.Created = filter => tempFilter = filter);
            })
        });
