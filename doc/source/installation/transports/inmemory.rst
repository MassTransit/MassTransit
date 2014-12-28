Loopback
========

.. warn::

    This is an In Memory transport. If the process crashes you WILL lose unprocessed
    messages.

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc =>
  {
      //loopback is configured by default
  });
