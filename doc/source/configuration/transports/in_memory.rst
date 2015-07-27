Loopback Configuration Options
""""""""""""""""""""""""""""""

.. warning::

    Great for testing, not great for production.

.. sourcecode:: csharp

  Bus.Factory.CreateUsingInMemory(cfg =>
  {
      cfg.ReceiveEndpoint("queue_name", ep =>{
          //configure the endpoint
      })
  });


So the loopback uses the protocol 'loopback' - localhost is the machine name
and then you can use whatever you want for the 'queue' bit as long as
each bus instance has a unique one, you should be good to go.
