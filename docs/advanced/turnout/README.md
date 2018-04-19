# Long-running tasks using Turnout

Message consumers are similar to web controllers in that they are intended to live for a short time -- the time
it takes to process a single message. However, there are times when the processing *initiated* by a message takes
long time (for instance, longer than a minute). Rather than block the message consumer and preventing it from
consuming additional messages, a way to start an asynchronous task is required.

Turnout enables the execution of long-running tasks initiated by a message consumer. Turnout manages the lifetime
of the task, and appropriately handles failure and handing off to other nodes in the event of a server failure.

> Turnout is an early-access feature, and it's only been tested with some basic scenarios. It works, but there
> are rough edges, so consider it like running with scissors.

## Configuring a Turnout

To configure a turnout, a turnout endpoint is created for the command message type.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.UseInMemoryScheduler();

    cfg.TurnoutEndpoint<AuditCustomerHistory>(host, "audit_consumer_history", e =>
    {
        x.SuperviseInterval = TimeSpan.FromSeconds(30);
        x.SetJobFactory(async context =>
        {
            await Task.Delay(TimeSpan.FromMinutes(7), context.CancellationToken);
        })
    });
});
```

## Turnout - Under the hood

When a turnout endpoint is created, the queue name specified is used to create several queues. First, the
*audit_consumer_history* queue is created, as it is the queue specified by the developer. This is where the
commands are sent, just like a normal consumer.

A second queue, *audit_consumer_history-expired* is also created, and is used as the dead-letter queue for scheduled
messages which are not consumed by the service. More on this in a minute, but this is used to handle node failures gracefully.

> Node failures such as this are still being developed, so, consider some of this forward looking. The simplest case already works though.

A third queue, *turnout-???*, is created to allow the node to send commands to itself to supervise the state of the Task.
Messages are sent to this endpoint by the node (each node gets a unique queue) using the message scheduler (this is a
great place to use the built-in scheduling of Azure Service Bus, or the delayed exchange with RabbitMQ).

### Job supervision

Rather than rely on in-process timers, Turnout uses the queuing and scheduling features to supervise jobs.
Messages are scheduled every interval to check on the status of the job. If the job completed, it is removed
from the job registry. If it is still running, a new supervision command is scheduled. Each command is scheduled
for a specific time, and the time-to-live is set to the supervision interval (plus a delta to ensure it has time to be received).

If the node crashes (does not cleanly shut down), the messages will not be received and will dead letter
to the *-expired* queue. All nodes listen on that queue, and if a node receives a command that was a job
being processed on another node, it will handle that as a faulted command. Since the node is typically running
smoothly, the messages are consumed on time.

> Faulted job handling has yet to be decided, but will be configurable to either follow a retry policy,
> or just report that the job faulted.

### Stopping a node

When a node is stopped, any running jobs are aborted (different than being cancelled). This way, the job is
reported as aborted and another node can pickup the job to continue processing.
