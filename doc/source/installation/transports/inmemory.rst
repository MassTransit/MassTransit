Loopback
========

.. warn::

    This is an In Memory transport. If the process crashes you WILL lose unprocessed
    messages.

.. sourcecode:: csharp

  using MassTransit;

  //later

  Bus.Factory.CreateUsingInMemory(cfg =>{
    //this is in the base MassTransit.dll
  });
