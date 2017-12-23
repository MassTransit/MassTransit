Prerequisites
=============

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/introduction/prerequisites.html

MassTransit is written in C# and requires a compatible .NET framework. MassTransit is built and tested with .NET 4.5,
and the shipped assemblies target this version. Visual Studio 2015 is used, and the new C# 6 syntax is used.

*For comparison, my development machines is a MacBook Pro hosting a Windows Server 2012 R2 virtual machine (via VMware
Fusion), with Visual Studio 2015 and Resharper 9.x installed. My secondary rig is a Razer Blade running Windows 10, which
also works just fine.*

.. note:: 
    
    With the release of Mono 4.0, and the upcoming release of .NET Core, MassTransit will be soon target
    both Windows and Mac/Linux applications. At this time, however, this has not been fully tested by the
    primary contributors.


Transports
----------

MassTransit leverages existing message transports, so you will need to have a supported transport installed.


In Memory
~~~~~~~~~

The `in memory`_ transport is included with MassTransit. No additional software is required.

.. _in memory: ../configuration/transports/in_memory.html


RabbitMQ
~~~~~~~~

To use RabbitMQ, download_ and install_ the version appropriate for your operating system. Once the broker is installed,
enable some additional plug-ins for management and message tracking.

.. _download: http://www.rabbitmq.com/download.html
.. _install: http://www.rabbitmq.com/install-windows.html

.. sourcecode::

    rabbitmq-plugins.bat enable rabbitmq_management 

Then, install the ```MassTransit.RabbitMQ``` package in your project and follow the RabbitMQ_ configuration guide.

.. _RabbitMQ: ../configuration/transports/rabbitmq.html


Azure Service Bus
~~~~~~~~~~~~~~~~~

`Azure Service Bus`_ is a generic, cloud-based messaging system for connecting just about anything—applications, services, and devices—wherever they are. Connect apps running on Azure, on-premises systems, or both. You can even use Service Bus to connect household appliances, sensors, and other devices like tablets or phones to a central application or to each other.

.. _Azure Service Bus: http://azure.microsoft.com/en-us/services/service-bus/


To use Azure Service Bus with MassTransit, install the ```MassTransit.AzureServiceBus``` package in your project and follow the
`Service Bus`_ configuration guide.

.. _Service Bus: ../configuration/transports/azure.html

