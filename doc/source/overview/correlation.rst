Correlation
===========

In MassTransit we can mark a message as correlated by implementing the interface

.. sourcecode:: csharp

	public interface CorrelatedBy<TKey>
	{
        TKey CorrelationId { get; }
	}

Ok, that’s great, but what does it mean to be correlated? In short it means that
this message is a part of a larger story. For instance, you may have a message that says
‘New Order (Item:Hammers; Qty:22; OrderNumber:45)’ and there may be another message that
is a response to that message that says ‘Order Allocated(OrderNumber:45)’. In this case
the order number is acting as your correlation id, it ties the messages together.

NOTE: Since most transactions in a system will ultimately be logged and wide-scale
correlation is likely, it is recommended to use a Guid type. The Magnum library has
a CombGuid that is clustered-index friendly for SQL Server, making it as efficient
to insert as an integer/bigint key. Just use CombGuid.Generate() to get a new one and
be happy. Note, this is the same algorithm that NHibernate uses for guid.comb primary
keys.

Also, sagas are always correlated with Guids, so any messages that will interact
with the saga should use a Guid correlationId.