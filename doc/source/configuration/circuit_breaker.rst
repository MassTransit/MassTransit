Using the Circuit Breaker
"""""""""""""""""""""""""

What is this?

.. sourcecode:: csharp

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseCircuitBreaker(cb =>
        {
            //does ???
            cb.ActiveCount = 1;

            //Does ???
            cb.Duration = TimeSpan.FromSeconds(2),

            //Does
            cb.TripThreshold = 3;
        });
    });
