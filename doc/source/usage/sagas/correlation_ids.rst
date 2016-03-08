Correlation Ids
===============================

Correlating messages is a powerful feature for messages, and sagas are a great
place to take advantage of them. The correlation identifier allows messages
disparate types to all link together, it effectively marks them as a group.

Ok, that's great, but what does it mean to be correlated? In short it means that
this message is a part of a larger story. For instance, you may have a message that says
``New Order (Item:Hammers; Qty:22; OrderNumber:45)`` and there may be another message that
is a response to that message that says ``Order Allocated(OrderNumber:45)``. In this case
the order number is acting as your correlation id, it ties the messages together.

.. note::

    Since most transactions in a system will ultimately be logged and wide-scale
    correlation is likely, it is recommended to use a Guid type. The Magnum library has
    a CombGuid that is clustered-index friendly for SQL Server, making it as efficient
    to insert as an integer/bigint key. Just use ``CombGuid.Generate()`` to get a new one and
    be happy. Note, this is the same algorithm that `NHibernate <http://nhibernate.info/blog/2009/05/21/using-the-guid-comb-identifier-strategy.html>` uses for ``guid.comb`` primary
    keys.

Also, sagas are always correlated with Guids, so any messages that will interact
with the saga should use a Guid correlation id.

To add a message correlation you do the following:

.. sourcecode:: csharp

    MessageCorrelation.UseCorrelationId<YouMessageClass>(x => x.SomeGuidValue);

.. note::

    This should be called before you start the bus. We currently recommend that
    you put all of these in a static method for easy grouping and then call it
    at the beginning of the MassTransit configuration block.
