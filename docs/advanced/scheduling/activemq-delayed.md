# ActiveMQ

MassTransit uses the built-in ActiveMQ scheduler to schedule messages.

::: tip Quartz.NET Docker Image
MassTransit provides a [Docker Image](https://hub.docker.com/r/masstransit/activemq) with ActiveMQ ready to run, including scheduler support.
:::

### Configuration

To configure the ActiveMQ message scheduler, see the example below.

<<< @/docs/code/scheduling/SchedulingActiveMQ.cs

::: warning
Scheduled messages cannot be canceled when using the ActiveMQ message scheduler
:::

[1]: https://activemq.apache.org/delay-and-schedule-message-delivery
