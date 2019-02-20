# Scheduling with Azure Service Bus

Azure Service Bus allows the enqueue time of a message to be specified, making it possible to schedule messages without the use of a separate message scheduler. MassTransit makes it easy to take advantage of this feature by configuring the bus scheduler to specify the enqueue time for scheduled messages.

## Configuring the enqueue time scheduler

To configure the bus (or a receive endpoint) to use the enqueue time for message scheduling, add the code below to the configuration.

```csharp
var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
{
    var host = cfg.Host(serviceAddress, h =>
    {
        // ...
    });

    cfg.UseServiceBusMessageScheduler();
});
```

This configures the bus scheduler, which is available via the MessageSchedulerContext interface. Once configured, the message scheduling extensions can be used (which are available on the ConsumeContext). For example, to schedule a message for future delivery from within a message consumer.

```csharp
public class ScheduleNotificationConsumer :
    IConsumer<AssignSeat>
{
    Uri _schedulerAddress;
    Uri _notificationService;

    public async Task Consume(ConsumeContext<AssignSeat> context)
    {
        if(context.Message.ReservationTime - DateTime.Now < TimeSpan.FromHours(8))
        {
            // assign the seat for the reservation
        }
        else
        {
            // seats can only be assigned eight hours before the reservation

            context.ScheduleMessage(context.Message.ReservationTime - TimeSpan.FromHours(8), context.Message);
        }
    }
}
```

This will schedule the message to be delivered to the consumer endpoint at the specified time.

> If the message should be sent to a different endpoint, the destination address can be specified as an additional parameter.
