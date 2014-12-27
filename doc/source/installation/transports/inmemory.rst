Loopback
========

.. note::

    The loopback transport is great for testing. Not so much for production.
    
.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
      //loopback is configured by default
  });