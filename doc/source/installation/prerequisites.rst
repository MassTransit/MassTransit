Prerequisites
=============

MassTransit is a .NET framework for C# and will need a .NET runtime to run on.

To work with MassTransit you will need to be running on a Windows operating
system. The developers of MassTransit regulary test on Windows 7 and
Windows Server 2008RC2.

.. note::

    People are starting to run MassTransit with RabbitMQ on Mono with success.

.NET Framework
""""""""""""""

Currently MassTransit is tested on .NET 4.5

Transport choices
"""""""""""""""""

MassTransit sits on top of a communication layer like `RabbitMQ`_ or `Azure Service Bus`_. So you
will need to have one of those installed. We currently support:

.. toctree::

    transports/inmemory.rst
    transports/rabbitmq.rst
    transports/azure.rst

.. _RabbitMQ: http://www.rabbitmq.com/
.. _Azure Service Bus: http://azure.microsoft.com/en-us/services/service-bus/
