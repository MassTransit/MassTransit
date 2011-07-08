Prerequisites
=============

To work with MassTransit you will need to be running on a Windows operating
system. The developers of MassTransit regulary test on Windows 7 and
Windows Server 2008RC2. Though it should still work on Windows Server 2003, as
long as .Net 3.5 sp1 is installed.

.Net Framework
""""""""""""""

Currently MassTransit is tested on .Net 3.5 sp1 and .Net 4.0.

Transport Choices
"""""""""""""""""

MassTransit sits on top of a communication layer like MSMQ, or RabbitMQ. So you
will need to have one of those installed. We currently support:

.. toctree::

    transports/loopback.rst
    transports/msmq.rst
    transports/rabbitmq.rst
