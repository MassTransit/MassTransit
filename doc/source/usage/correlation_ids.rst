Correlation identifiers
=======================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/correlation.html

In a distributed message-based system, message correlation is very important. Since operations are potentially executing across hundreds of nodes, the ability to correlate different messages to build a path through the system is absolutely necessary for engineers to troubleshoot problems.

The headers on the message envelope provided by MassTransit already make it easy to specify correlation values. In fact, most are setup by default if not specified by the developer.

MassTransit provides the interface ``CorrelatedBy<T>``, which can be used to setup a default correlationId. This is used by sagas as well, since all sagas have a unique ``CorrelationId`` for each instance of the saga. If a message implements ``CorrelatedBy<Guid>``, it will automatically be directed to the saga instance with the matching identifier. If a new saga instance is created by the event, it will be assigned the ``CorrelationId`` from the initiating message.

For message types that have a correlation identifier, but are not using the ``CorrelatedBy`` interface, it is possible to declare the identifier for the message type and MassTransit will use that identifier by default for correlation.


.. sourcecode:: csharp

    MessageCorrelation.UseCorrelationId<YourMessageClass>(x => x.SomeGuidValue);

.. note::

    This should be called before you start the bus. We currently recommend that
    you put all of these in a static method for easy grouping and then call it
    at the beginning of the MassTransit configuration block.

Most transactions in a system will end up being logged and wide scale correlation is likely. Therefore, the use of consistent correlation identifiers is recommended. In fact, using a ``Guid`` type is highly recommended. MassTransit uses the `NewId`_ library to generate identifiers that are unique and sequential that are represented as a ``Guid``. The identifiers are clustered-index friendly, being ordered in a way that SQL Server can efficiently insert them into a database with the *uniqueidentifier* as the primary key. Just use ``NewId.NextGuid()`` to generate an identifier -- it's fast, fun, and all your friends are doing it.

.. _NewId: https://www.nuget.org/packages/NewId

.. note::

    So, what does correlated actually mean? In short it means that this message is a part of a larger conversation. For instance, you may have a message that says ``New Order (Item:Hammers; Qty:22; OrderNumber:45)`` and there may be another message that is a response to that message that says ``Order Allocated(OrderNumber:45)``. In this case, the order number is acting as your correlation identifier, it ties the messages together.
