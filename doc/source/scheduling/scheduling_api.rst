Using the Scheduling API
========================

The scheduling API consists of several extension methods that send messages to an endpoint where
the Quartz scheduling consumers are connected.


Scheduling a Message
--------------------

To schedule a message, call the ``ScheduleSend`` method with the message to be delivered.

.. sourcecode:: csharp
    :linenos:

    public interface ScheduleNotification
    {
        DateTime DeliveryTime { get; }
        string EmailAddress { get; }
        string Body { get; }
    }

    public interface SendNotification
    {
        string EmailAddress { get; }
        string Body { get; }
    }

    public class ScheduleNotificationConsumer : 
        IConsumer<ScheduleNotification>
    {
        Uri _schedulerAddress;
        Uri _notificationService;

        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            var schedulerEndpoint = await context.GetSendEndpoint(_schedulerAddress);

            await schedulerEndpoint.ScheduleSend(_notificationService,
                context.Message.DeliveryTime,
                new SendNotification 
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body =  context.Message.Body
                });
        }

        class SendNotificationCommand :
            SendNotification
        {
            public string EmailAddress { get; set; }
            public string Body { get; set; }
        }
    }

The ``ScheduleMessage`` command message will be sent to the Quartz endpoint, which will
schedule a job in Quartz to deliver the message (and save the message body to be delivered).
When the job is triggered, the message will be sent to the destination address.
