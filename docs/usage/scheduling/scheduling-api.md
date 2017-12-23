# Using the scheduling API

The scheduling API consists of several extension methods that send messages to an endpoint where
the Quartz scheduling consumers are connected.

## Configuring the Quartz address

The bus has an internal context that is used to make it so that consumers that need to schedule 
messages do not have to be aware of the specific scheduler type being used, or the message scheduler 
address. To configure the address, use the extension method shown below.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
});
```

Once configured, messages may be scheduled from any message consumer as shown below.

## Scheduling a message from a consumer

To schedule a message, call the `ScheduleSend` method with the message to be delivered.

```csharp
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
        await context.ScheduleSend(_notificationService,
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
```

The `ScheduleMessage` command message will be sent to the Quartz endpoint, which will
schedule a job in Quartz to deliver the message (and save the message body to be delivered).
When the job is triggered, the message will be sent to the destination address.

## Scheduling a message from the bus

If a message needs to be scheduled from the bus itself (not in the context of consuming a message), the 
`SendEndpoint` for the Quartz scheduler should be retrieved and used to schedule the send.

```csharp
var schedulerEndpoint = await bus.GetSendEndpoint(_schedulerAddress);    
                                                                    
await schedulerEndpoint.ScheduleSend(_notificationService,                   
    context.Message.DeliveryTime,                                            
    new SendNotification                                                     
    {                                                                        
        EmailAddress = context.Message.EmailAddress,                         
        Body =  context.Message.Body                                         
    });
```

This should only be used outside of a consume context, however, as the lineage of the message will be lost 
(things like ConversationId, InitiatorId, etc.).

## Scheduling a recurring message

You can also schedule a message to be send to you periodically. This functionality uses the Quartz.Net periodic 
schedule feature and requires some knowledge of cron expressions.

To request a recurring message, you need to use `ScheduleRecurringSend` extension method, which is available 
for both `Context` and `SendEndpoint`. This message requires a schedule object as a parameter, which must 
implement `RecurringSchedule` interface. Since this interface is rather broad, you can use the default 
abstract implementation `DefaultRecurringSchedule` as the base class for your own schedule.

```csharp
public class PollExternalSystemSchedule : DefaultRecurringSchedule
{
    public PollExternalSystemSchedule()
    {
        CronExpression = "0 0/1 * 1/1 * ? *"; // this means every minute
    }
}

public class PollExternalSystem {}
```

```csharp
var schedulerEndpoint = await bus.GetSendEndpoint(_schedulerAddress);
    
var scheduledRecurringMessage = await schedulerEndpoint.ScheduleRecurringSend(
    InputQueueAddress, new PollExternalSystemSchedule(), new PollExternalSystem());
```

When you stop your service or just have any other need to tell Quartz service to stop sending you 
these recurring messages, you can use the return value of `ScheduleRecurringSend` to cancel the recurring schedule.

```csharp
await bus.CancelScheduledRecurringMessage(scheduledRecurringMessage);
```

You can also cancel using schedule id and schedule group values, which are part of the recurring schedule object.
