# Using Courier

Developing applications using a distributed, message-based architecture significantly increases the complexity of performing operations transactionally, where an end-to-end set of steps must be completed entirely, or not at all. In an application using an ACID database, this is typically done using SQL transactions, where partial operations are rolled back if the transaction cannot be completed. However, this doesn't scale when the steps being to include dependencies outside of a single database. And in the distributed, *microservices* based architectures, the use of a single ACID database is shrinking to completely non-existent.

MassTransit Courier is a mechanism for creating and executing distributed transactions with fault compensation that can be used to meet the requirements previously within the domain of database transactions, but built to scale across a large system of distributed services. Courier also works well with MassTransit sagas, which add transaction monitoring and recoverability.

## Using a Routing Slip

A routing slip specifies a sequence of processing steps called *activities* that are combined into a single transaction. As each activity completes, the routing slip is forwarded to the next activity in the itinerary. When all activities have completed, the routing slip is completed and the transaction is complete.

A key advantage to using a routing slip is it allows the activities to vary for each transaction. Depending upon the requirements for each transaction, which may differ based on things like payment methods, billing or shipping address, or customer preference ratings, the routing slip builder can selectively add activities to the routing slip. This dynamic behavior is in contrast to a more explicit behavior defined by a state machine or sequential workflow that is statically defined (either through the use of code, a DSL, or something like Windows Workflow).

## MassTransit Courier

MassTransit Courier is a framework that implements the routing slip pattern. Leveraging a durable messaging transport and the advanced saga features of MassTransit, Courier provides a powerful set of components to simplify the use of routing slips in distributed applications. Combining the routing slip pattern with a state machine such as [Automatonymous](https://github.com/phatboyg/Automatonymous) results in a reliable, recoverable, and supportable approach for coordinating and monitoring message processing across multiple services.

In addition to the basic routing slip pattern, MassTransit Courier also supports [compensations][1] which allow activities to store execution data so that reversible operations can be undone, using either a traditional rollback mechanism or by applying an offsetting operation. For example, an activity that holds a seat for a patron could release the held seat when compensated.

* [Activities](activities.md)
* [Builder](builder.md)
* [Execution](execute.md)
* [Events](events.md)
* [Subscriptions](subscriptions.md)
    

[1]: http://en.wikipedia.org/wiki/Compensation_%28engineering%29