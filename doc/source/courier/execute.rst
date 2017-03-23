Executing the routing slip
==========================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/advanced/courier/execute.html

Once built, the routing slip is executed, which sends it to the first activity’s execute URI. To make it easy and to ensure that source information is included, an extension method on *IBus* is available, the usage of which is shown below.

.. sourcecode:: csharp

    await bus.Execute(routingSlip);


It should be pointed out that if the address for the first activity is invalid or cannot be reached, an exception will be thrown by the *Execute* method.
