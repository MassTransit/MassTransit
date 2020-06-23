# Azure Service Bus

Azure Service Bus allows the enqueue time of a message to be specified, making it possible to schedule messages without the use of a separate message scheduler. MassTransit uses this feature to schedule messages.

### Configuration

To configure the Azure Service Bus message scheduler, see the example below.

<<< @/docs/code/scheduling/SchedulingAzure.cs

::: tip
Azure Service Bus supports message cancellation, unlike the other transports.
:::