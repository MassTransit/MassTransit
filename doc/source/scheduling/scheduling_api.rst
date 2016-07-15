Using the scheduling API
========================

The scheduling API consists of several extension methods that send messages to an endpoint where
the Quartz scheduling consumers are connected.

Configuring the quartz address
------------------------------

The bus has an internal context that is used to make it so that consumers that need to schedule messages do not have to be aware of the specific scheduler type being used, or the message scheduler address. To configure the address, use the extension method shown below.

.. sourcecode:: csharp

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
    });

Once configured, messages may be scheduled from any message consumer as shown below.


Scheduling a message from a consumer
------------------------------------

To schedule a message, call the ``ScheduleMessage`` method with the message to be delivered.

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
        Uri _notificationService;

        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            context.ScheduleMessage(_notificationService,
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

Scheduling a message from the bus
---------------------------------

If a message needs to be scheduled from the bus itself (not in the context of consuming a message), the SendEndpoint for the quartz scheduler should be retrieved and used to schedule the send.

.. sourcecode:: csharp

    var schedulerEndpoint = await bus.GetSendEndpoint(_schedulerAddress);    
                                                                      
    await schedulerEndpoint.ScheduleSend(_notificationService,                   
        context.Message.DeliveryTime,                                            
        new SendNotification                                                     
        {                                                                        
            EmailAddress = context.Message.EmailAddress,                         
            Body =  context.Message.Body                                         
        });

This should only be used outside of a consume context, however, as the lineage of the message will be lost (things like ConversationId, InitiatorId, etc.).
