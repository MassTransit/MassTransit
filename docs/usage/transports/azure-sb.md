# Azure Service Bus

> [MassTransit.Azure.ServiceBus.Core](https://nuget.org/packages/MassTransit.Azure.ServiceBus.Core/)

::: warning WARNING
The Azure Service Bus transport only supports **Standard** and **Premium** tiers of the Microsoft Azure Service Bus service. Premium tier is recommended for production environments.
:::

## Examples

### Standard

To configure Azure Service Bus, use the connection string (from the Azure portal) to configure the host as shown below.

<<< @/docs/code/transports/ServiceBusConsoleListener.cs

### Example with Azure Managed Identity

The following example shows how to configure Azure Service Bus using an Azure Managed Identity:

<<< @/docs/code/transports/ServiceBusManagedIdentityConsoleListener.cs

During local development, in the case of Visual Studio, you can configure the account to use under Options -> Azure Service Authentication. Note that your Azure Active Directory user needs explicit access to the resource and have the 'Azure Service Bus Data Owner' role assigned.

## Options

Azure Service Bus queues includes an extensive set a properties that can be configured. All of these are optional, MassTransit uses sensible defaults, but the control is there when needed.

<<< @/docs/code/transports/ServiceBusReceiveEndpoint.cs

### Host Properties

| Property                | Type   | Description 
|-------------------------|--------|------------------
| TokenCredential       | Use a specific token-based credential, such as a managed identity token, to access the namespace.  You can use the [DefaultAzureCredential](https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) to automatically apply any one of several credential types.
| TransportType         | Change the transport type from the default (AMQP) to use WebSockets
| PrefetchCount         | int | The number of unacknowledged messages that can be processed concurrently (default based on CPU count)
| MaxConcurrentCalls         | int | How many concurrent messages to dispatch (transport-throttled)
| LockDuration        | TimeSpan   | How long to hold message locks (max is 5 minutes)
| MaxAutoRenewDuration        | TimeSpan   | How long to renew message locks (maximum consumer duration)
| RequiresSession        | bool   | If true, a message SessionId must be specified when sending messages to the queue
| MaxDeliveryCount        | int   | How many times the transport will redeliver the message on negative acknowledgment. This is different from retry, this is the transport redelivering the message to a receive endpoint before moving it to the dead letter queue.


