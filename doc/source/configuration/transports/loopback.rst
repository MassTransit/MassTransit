Loopback Configuration Options
""""""""""""""""""""""""""""""

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    //loopback is the default
  });

.. note::

    Great for testing, not great for production.