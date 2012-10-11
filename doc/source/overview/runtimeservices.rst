Runtime Services
===================

The MassTransit framework also comes with several services which can be run to provide additional functionality,
as well as examples of how to build services that either extend bus behavior or provide a general service. These
services are:

Subscription Service
""""""""""""""""""""

Because MSMQ has anemic routing capabilities MassTransit originally shipped with a central subscription registry that
all of the busses could listen to, to learn when a new ‘subscription’ would come online. As of 2.0 the
MsmqMulticastSubscription manager provides similar functionality while removing the need for a single point
of failure. For users of RabbitMQ this service is not needed as the RabbitMQ approach leverages its excellent
message routing capabilities.

.. warning::

  This is only needed for MSMQ or other transports that have no routing capabilities

Timeout Service
"""""""""""""""

This service provide an easy event based way to register timeouts for your sagas. These timeouts are persisted to
disk and will survive restart.

Health Service
""""""""""""""

Attempts to track all known endpoints and monitor their status for up/down. Currently is very remedial.
