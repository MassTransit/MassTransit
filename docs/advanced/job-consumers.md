# Job Consumers

When a message is delivered from the message broker to a consumer instance, the message is _locked_ by the broker. Once the consumer completes, MassTransit will acknowledge the message on the broker, removing it from the queue. While the message is locked, it will not be delivered to another consumer – on any bus instance reading from the same queue (competing consumer pattern). However, if the broker connection is lost the message will be unlocked and redelivered to a new consumer instance. The lock timeout is usually long enough for most message consumers, and this rarely is an issue in practice for consumers that complete quickly.

However, there are plenty of use cases where consumers may run for a longer duration, from minutes to even hours. In these situations, a job consumer _may_ be used to decouple the consumer from the broker. A job consumer is a specialized consumer designed to execute _jobs_, defined by implementing the `IJobConsumer<T>` interface where `T` is the job message type. Job consumers may be used for long-running tasks, such as converting a video file, but can really be used for any task. Job consumers have additional requirements, such as a database to store the job messages, manage concurrency and retry, and report job completion or failure. 

::: warning
MassTransit includes a job service that keeps track of each job, assigns jobs to service instances, and schedules job retries when necessary. The job service uses three saga state machines and the default configuration uses an in-memory saga repository, which is **not durable**. When using job consumers for production use cases, configuring durable saga repositories is _highly recommended_ to avoid possible message loss.

Check out the [sample project](https://github.com/MassTransit/Sample-JobConsumers) on GitHub, which includes the Entity Framework configuration for the job service state machines.
:::

To use job consumers, a _service instance_ must be configured (see below).

### IJobConsumer

A job consumer implements the `IJobConsumer<T>` interface, shown below.

<<< @/src/MassTransit.Abstractions/IJobConsumer.cs

### Configuration


The example below configures a job consumer on a receive endpoint named using an _IEndpointNameFormatter_ passing the consumer type.

<<< @/docs/code/turnout/JobSystemConsoleService.cs

In this example, the job timeout as well as the number of concurrent jobs allowed is specified using `JobOptions<T>` when configuring the consumer. The job options can also be specified using a consumer definition in the same way.

### Client

To submit jobs to the job consumer, use the service client to create a request client as shown, and send the request. The _JobId_ is assigned the _RequestId_ value.

<<< @/docs/code/turnout/JobSystemClient.cs

### Job Service Endpoints

The job service saga state machines are configured on their own endpoints, using the configured endpoint name formatter. These endpoints are required on _at least one_ bus instance. Additionally, it is not necessary to configure them on _every_ bus instance. In the example above, the job service endpoint are configured. Another method, _ConfigureJobService_, is used to configure the job service without configuring the saga state machine endpoints. In situations where there are many bus instances with job consumers, it is suggested that only one or two instances host the job service endpoints to avoid concurrency issues with the sagas repositories – particularly when optimistic locking is used.

To configure a service instance without the job service endpoints, replace _ConfigureJobServiceEndpoints_ with _ConfigureJobService_.

```cs
x.UsingRabbitMq((context, cfg) =>
{
    cfg.ServiceInstance(instance =>
    {
        instance.ConfigureJobService();

        instance.ConfigureEndpoints(context);
    });
});
```

For a more detailed example of configuring the job service endpoints, including persistent storage, see the sample mentioned in the warning box above.
