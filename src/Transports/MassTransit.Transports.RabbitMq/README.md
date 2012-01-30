# RabbitMQ Transport

This readme details the transport implementation.

## Transport aims

Provide a first-citizen class transport to MassTransit that supports all the message patterns we'd want:

 * Publish to any listener (that may or may not have subscribed already)
 * Polymorphic Message Routing
 * Send to endpoint
 * etc...

Further, the transport is able to handle both durable and non-durable (transient?) messages, depending
on desired performance characteristics.

## Work to be done

 * Currently the transport has a prefetch of 1 message. It would be nice to be able to tweak this while still ensuring
   that we don't lose messages if the server or the bus goes down. This would probably have to be facilitated
   through *publisher confirms*
 * The transport uses *basic_get* which is a polling implementation. Potentially it could be faster and nicer to use
   *basic_consume*. Have a look at `InboundRabbitMqTransport` for more details on this.
 * RabbitMQ supports AX-transactions, something that LTC and DTC supposedly is compliant with also - but both
   `TransactionalBehaviour` implementations are empty. Specifically, adding support for two-phase-commit over the
   network.
 * RabbitMQ supports AMQP-transactions, i.e. message batching on both consume and publish/send. Could this be exposed
   as receive context information?
 * It's currently hairy to get SSL/TLS started with RabbitMQ on Windows machines - can we provide guidance or
   something to run that sets up some pieces of the certificate authority machinery required for actual TLS deployments?