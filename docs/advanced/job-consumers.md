# Job Consumers

Message consumers are intended to be used similarly to web controllers. Consumer instances are meant to live for a relatively short time, usually the time it takes to consume a single message. There are, however, scenarios where the processing _initiated_ by a message takes a long time (like, maybe more than a couple of minutes). Instead of waiting for the processing to complete, preventing subsequent message consumption, job consumers are created and run outside of the message transport (but still initiated by a message).

A job consumer is a specialized consumer type designed to execute _jobs_, and is defined by implementing the `IJobConsumer<T>` interface where `T` is the job message type. Job consumers are typically used for long-running tasks, such as converting a video file, but can be used for any task. To use job consumers, _Conductor_, which is a set of managed services included with MassTransit, must be configured.

::: warning New and Improved
Job Consumers replace Turnout, a previous feature of MassTransit, which was poorly supported and very limited. While Turnout is now deprecated, job consumers offer a much better developer experience and are better integrated.
:::

### IJobConsumer

<<< @/src/MassTransit/JobService/IJobConsumer.cs

### Configuration

The example below configures a job consumer on a receive endpoint named using an _IEndpointNameFormatter_ passing the consumer type.

<<< @/docs/code/turnout/JobSystemConsoleService.cs

In this example, the job timeout as well as the number of concurrent jobs allowed is specified using `JobOptions<T>` when configuring the consumer. The job options can also be specified using a consumer definition in the same way.

### Client

To submit jobs to the job consumer, use the service client to create a request client as shown, and send the request. The _JobId_ is assigned the _RequestId_ value.

<<< @/docs/code/turnout/JobSystemClient.cs

### Supporting Endpoints

When the job service endpoints are configured, a set of saga state machines are configured used to track job execution across multiple service instances. This ensure that each job execution is tracked, faults are observed, and retry attempts are scheduled.

Conductor is used to manage the _service instance_, including the service instance endpoint. Each service instance has its own endpoint which is used to communicate with the job consumers executing on that instance.
