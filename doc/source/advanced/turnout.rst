Running Long Tasks with Turnout
===============================

Message consumers are similar to web controllers in that they are intended to live for a short time -- the time it takes to process a single message. However, there are times when the processing *initiated* by a message takes long time (for instance, longer than a minute). Rather than block the message consumer and preventing it from consuming additional messages, a way to start an asynchronous task is required.

Turnout enables the execution of long-running tasks initiated by a message consumer. Turnout manages the lifetime of the task, and appropriately handles failure and handing off to other nodes in the event of a server failure.


Configurating a Turnout
-----------------------

To configure a turnout, a receive endpoint is created and the turnout is configured for the initiating message type.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseInMemoryScheduler();
        
        cfg.ReceiveEndpoint(host, "audit_consumer_history", e =>
        {
            e.Turnout<AuditCustomerHistory>(cfg, x =>
            {
                x.SuperviseInterval = TimeSpan.FromSeconds(30);
                x.SetJobFactory(async context =>
                {
                    await Task.Delay(TimeSpan.FromHours(2));
                })
            });
        });
    });



The Supervise/Monitor Pattern
-----------------------------

Turnout uses a new pattern, where there are two queues at play. The first, a supervision queue, is unique to each service instance. The second, a monitoring queue, is shared by all service instances (competing consumer).

Supervising Tasks
~~~~~~~~~~~~~~~~~

Each service instance keeps track of running tasks by sending messages to itself on a regular interval. As long as the service is up and running, the supervision messages are read from the queue and processing continues normally. If a service instance fails, and is no longer reading from the queue, other nodes can intervene and restart the job.

Monitoring Tasks
~~~~~~~~~~~~~~~~

Each service instance receives from the monitoring queue, and handles the messages by checking to see if the monitored job is being kept alive by the job's instance. If the monitor message is delivered and consumed and the supervise message is still waiting in the queue, the instance assumes that the job's instance is no longer available and takes ownership of the job.