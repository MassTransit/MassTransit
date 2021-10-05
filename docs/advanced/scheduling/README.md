# Scheduling

Time is important, particularly in distributed applications. Sophisticated systems need to schedule things, and MassTransit has extensive scheduling support.

MassTransit supports two different methods of message scheduling:

1. Scheduler-based, using either Quartz.NET or Hangfire, where the scheduler runs in a service and schedules messages using a queue.
1. Transport-based, using the transports built-in message scheduling/delay capabilities. In some cases, such as RabbitMQ, this requires an additional plug-in to be installed and configured.

> Recurring schedules are only supported by scheduler-based solutions (Option 1).

## Configuration

Depending upon the scheduling method used, the bus must be configured to use the appropriate scheduler.

<code-group>
<code-block title="Quartz/Hangfire">
<<< @/docs/code/scheduling/SchedulingEndpoint.cs
</code-block>

<code-block title="RabbitMQ">
<<< @/docs/code/scheduling/SchedulingRabbitMQ.cs
</code-block>

<code-block title="Azure Service Bus">
<<< @/docs/code/scheduling/SchedulingAzure.cs
</code-block>

<code-block title="ActiveMQ">
<<< @/docs/code/scheduling/SchedulingActiveMQ.cs
</code-block>

<code-block title="Amazon SQS">
<<< @/docs/code/scheduling/SchedulingAmazonSQS.cs
</code-block>
</code-group>

### Using the message scheduler

To use the message scheduler (outside of a consumer), use _IMessageScheduler_ from the container.

## Quartz.NET

To use Quartz.NET, an instance of Quartz.NET must be running and configured to use the message broker.

### Internal Quartz.NET instance

MassTransit is able to connect to an existing Quartz.NET instance running in the same executable.

<<< @/docs/code/scheduling/SchedulingInternalInstance.cs

::: warning
The code above asumes Quartz.NET is already configured using dependency injection.
:::

### External Quartz.NET instance

MassTransit provides a [Docker Image](https://hub.docker.com/r/masstransit/quartz) with Quartz.NET ready-to-run using SQL Server. A complementary [SQL Server Image](https://hub.docker.com/r/masstransit/sqlserver-quartz) configured to run with Quartz.NET is also available. Combined, these images make getting started with Quartz easy.

### Testing

Quartz.NET can also be configured in-memory, which is great for unit testing. 

> Uses [MassTransit.Quartz](https://nuget.org/packages/MassTransit.Quartz)

<<< @/docs/code/scheduling/SchedulingInMemory.cs

The _UseInMemoryScheduler_ method initializes Quartz.NET for standalone in-memory operation, and configures a receive endpoint named `scheduler`. The _AddMessageScheduler_ adds _IMessageScheduler_ to the container, which will use the same scheduler endpoint.

::: warning
Using the in-memory scheduler uses non-durable storage. If the process terminates, any scheduled messages will be lost, immediately, never to be found again. For any production system, using a standalone service is recommended with persistent storage.
:::

## Transport-based

To configure transport-based message scheduling, refer to the transport-specific section for details.

* [ActiveMQ](activemq-delayed)
* [Amazon SQS](amazonsqs-scheduler)
* [Azure Service Bus](azure-sb-scheduler)
* [RabbitMQ](rabbitmq-delayed)

