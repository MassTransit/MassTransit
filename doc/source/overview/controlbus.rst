What is a Control Bus?
""""""""""""""""""""""

.. warning::

    The existing ``ControlBus`` configuration method is being removed. Please
    read below for the replacement

The idea of the control bus is one that responds to control messages. That is to
say, not the messages and events that are running your business. It is recommended
that the control messages also be sent on a separate ``Endpoint`` or in our case
a different RabbitMQ Queue or MSMQ Queue.

By setting the control bus up on a different ``Endpoint`` control messages
can't get blocked by data messages and vice versa. We use this to do various
things such as reload configuration data, clear caches, or check on the health
of our distributed services.

.. note::

  We recommend doing a purge before startup of the queue to make sure old/stale
  control messages are not consumed.

.. sourcecode:: csharp
    :linenos:

    var controlBus = ServiceBusfactory.New(sbc =>
    {
        sbc.SetPurgeOnStartup(true);

        //other configuration code
    });

Inspiration:

  http://www.eaipatterns.com/ControlBus.html
