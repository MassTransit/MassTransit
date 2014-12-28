Retry Policies
==============

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

Retry Options
"""""""""""""

None
''''

Immediate
'''''''''

Intervals
'''''''''

Exponential
'''''''''''

Incremental
'''''''''''

Except
''''''

Selected
''''''''

All
'''

Filter
''''''
