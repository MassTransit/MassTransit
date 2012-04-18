Loopback Configuration Options
""""""""""""""""""""""""""""""

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    //loopback is the default
    sbc.ReceiveFrom("loopback://localhost/queue");
  });

.. note::

    Great for testing, not great for production.

So the loopback uses the protocol 'loopback' - localhost is the machine name
and then you can use whatever you want for the 'queue' bit as long as 
each bus instance has a unique one, you should be good to go.
