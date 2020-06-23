# Schedule Messages

### From a Consumer

To schedule messages from a consumer, use any of the _ConsumeContext_ extension methods, such as _ScheduleSend_, to schedule messages.

<<< @/docs/code/scheduling/SchedulingConsumeContext.cs

The message scheduler, specified during bus configuration, will be used to schedule the message.

### From a Bus

To schedule messages from a bus, use _IMessageScheduler_ from the container (or create a new one using the bus and appropriate scheduler).

<<< @/docs/code/scheduling/SchedulingScheduler.cs

### Recurring Messages

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
