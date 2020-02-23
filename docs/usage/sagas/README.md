# Sagas

The ability to orchestrate a series of events is a powerful feature, and MassTransit makes this possible.

A saga is a long-lived transaction managed by a coordinator. Sagas are initiated by an event, sagas orchestrate events, and sagas maintain the state of the overall transaction. Sagas are designed to manage the complexity of a distributed transaction without locking and immediate consistency. They manage state and track any compensations required if a partial failure occurs.

We didn't create it, we learned it from the [original Princeton paper][1] and from Arnon Rotem-Gal-Oz's [description][2].

## State Machine Sagas

MassTransit includes [Automatonymous](automatonymous), which provides a powerful state machine syntax to create sagas. This approach is highly recommended when using MassTransit.

## Consumer Sagas

MassTransit supports [consumer sagas](consumer-saga), which implement one or more interfaces to consume correlated saga events. This support is included so that it is easy to move applications from other saga implementations to MassTransit. 


[1]: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf
[2]: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf

