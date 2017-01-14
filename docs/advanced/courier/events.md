# Monitoring routing slip execution

During routing slip execution, events are published when the routing slip completes or faults. Every 
event message includes the *TrackingNumber* as well as a *Timestamp* (in UTC, of course) indicating when the event occurred:

  * RoutingSlipCompleted
  * RoutingSlipFaulted
  * RoutingSlipCompensationFailed

Additional events are published for each activity, including:

  * RoutingSlipActivityCompleted
  * RoutingSlipActivityFaulted
  * RoutingSlipActivityCompensated
  * RoutingSlipActivityCompensationFailed

By observing these events, an application can monitor and track the state of a routing slip. To maintain the 
current state, an Automatonymous state machine could be created. To maintain history, events could be stored 
in a database and then queried using the *TrackingNumber* of the routing slip.

