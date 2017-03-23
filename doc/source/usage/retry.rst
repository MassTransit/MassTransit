Retry policies
==============

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/retries.html

When you register various consumers of messages, one of the configuration elements
you have control over is the retry policy.

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("queue_name", ep =>
        {
            ep.Handler(async cxt => {});
            ep.Handler(async cxt => {}, endpointConfig =>
            {
              endpointConfig.Retry(Retry.None);
            });
        });
    });

Retry options
"""""""""""""

None
''''

Immediate
'''''''''

``Retry.Immediate(5)``

Would retry 5 times with no delay.

Intervals
'''''''''

``Retry.Intervals(5.Seconds(), 5.Seconds())``

Would retry 2 times at 0:05, 0:10.

Exponential
'''''''''''

Takes a number of retries and attempts to do so for each interval.

``Retry.Exponential(5, 5.Seconds(), 5.Seconds(), 5.Seconds())``

Would retry 5 times at 0:05, 0:10, 0:15, 0:20, 0:25. ??

Incremental
'''''''''''

Takes a number of retries and attempts to do so for each interval.

``Retry.Incremental(5, 5.Seconds(), 5.Seconds())``

Would retry 5 times at 0:05, 0:10, 0:15, 0:20, 0:25.

Except
''''''

Selected
''''''''

All
'''

Filter
''''''
