# Scheduling

Time is important, particularly in distributed applications. Sophisticated systems need to schedule things, and MassTransit has extensive scheduling support.

MassTransit supports two different methods of message scheduling:

1. Scheduler-based, using either Quartz.NET or Hangfire, where the scheduler runs in a service and schedules messages using a queue.
1. Transport-based, using the transports built-in message scheduling/delay capabilities. In some cases, such as RabbitMQ, this requires an additional plug-in to be installed and configured.

> Recurring schedules are only supported by scheduler-based solutions (Option 1).

## Configuration

Depending upon the scheduling method used, the bus must be configured to use the appropriate scheduler.

### Quartz.NET / Hangfire

To configure the bus to use either Quartz.NET or Hangfire for message scheduling, add the _UseMessageScheduler_ method as shown below.

<<< @/docs/code/scheduling/SchedulingEndpoint.cs

The _UseMessageScheduler_ configures the bus to use the scheduler endpoint. The _AddMessageScheduler_ adds _IMessageScheduler_ to the container, which will use the same scheduler endpoint.

::: tip Quartz.NET Docker Image
MassTransit provides a [Docker Image](https://hub.docker.com/r/masstransit/quartz) with Quartz.NET ready-to-run using SQL Server. A complementary [SQL Server Image](https://hub.docker.com/r/masstransit/sqlserver-quartz) configured to run with Quartz.NET is also available. Combined, these images make getting started with Quartz easy.
:::

Quartz.NET can also be configured in-memory, which is great for unit testing. 

> Uses [MassTransit.Quartz](https://nuget.org/packages/MassTransit.Quartz)

<<< @/docs/code/scheduling/SchedulingInMemory.cs

The _UseInMemoryScheduler_ method initializes Quartz.NET for standalone in-memory operation, and configures a receive endpoint named `scheduler`. The _AddMessageScheduler_ adds _IMessageScheduler_ to the container, which will use the same scheduler endpoint.

::: warning
Using the in-memory scheduler uses non-durable storage. If the process terminates, any scheduled messages will be lost, immediately, never to be found again. For any production system, using a standalone service is recommended with persistent storage.
:::

### Transport-based

To configure transport-based message scheduling, refer to the transport-specific section for details.

* [ActiveMQ](activemq-delayed)
* [Amazon SQS](amazonsqs-scheduler)
* [Azure Service Bus](azure-sb-scheduler)
* [RabbitMQ](rabbitmq-delayed)

