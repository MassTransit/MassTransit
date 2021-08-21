# RabbitMQ

MassTransit uses the [RabbitMQ delayed exchange][1] plug-in to schedule messages.

::: tip RabbitMQ Docker Image
MassTransit provides a [Docker Image](https://hub.docker.com/r/masstransit/rabbitmq) with RabbitMQ ready to run, including the delayed exchange plug-in. 
:::

### Configuration

To configure the delayed exchange message scheduler, see the example below.

<<< @/docs/code/scheduling/SchedulingRabbitMQ.cs

::: warning
Scheduled messages cannot be canceled when using the delayed exchange message scheduler.
:::

[1]: https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/
