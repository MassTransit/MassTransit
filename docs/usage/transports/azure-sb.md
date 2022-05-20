# Azure Service Bus

> [MassTransit.Azure.ServiceBus.Core](https://nuget.org/packages/MassTransit.Azure.ServiceBus.Core/)

MassTransit fully supports Azure Service Bus, including many of the advanced features and capabilities.

::: warning WARNING
The Azure Service Bus transport only supports **Standard** and **Premium** tiers of the Microsoft Azure Service Bus service. Premium tier is recommended for production environments. See [Performance](#performance) section below.
:::

## Minimal Example

To configure Azure Service Bus, use the connection string (from the Azure portal) to configure the host as shown below.

<<< @/docs/code/transports/ServiceBusConsoleListener.cs

## Broker Topology

With Azure Service Bus (ASB), which supports topics and queues, messages are _sent_ or _published_ to topics and ASB routes those messages through topics to the appropriate queues.

## Configuration 


Azure Service Bus queues includes an extensive set a properties that can be configured. All of these are optional, MassTransit uses sensible defaults, but the control is there when needed.

<<< @/docs/code/transports/ServiceBusReceiveEndpoint.cs

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

## Additional Examples

### Example with Azure Managed Identity

The following example shows how to configure Azure Service Bus using an Azure Managed Identity:

<<< @/docs/code/transports/ServiceBusManagedIdentityConsoleListener.cs

During local development, in the case of Visual Studio, you can configure the account to use under Options -> Azure Service Authentication. Note that your Azure Active Directory user needs explicit access to the resource and have the 'Azure Service Bus Data Owner' role assigned.

::: warning WARNING
To ensure that Mass Transit has sufficient permissions to perform queue management as well as messaging operations. Your identity & managed identity will need to have the correct role assignments within Azure.

Assigning the role **Azure Service Bus Data Owner** will provide sufficient permissions for Mass Transit to function on the namespace.
:::

## Performance

We **really** recommend that you use the Premium subscription levels for production workloads. We have performed our own testing using [MassTransit Benchmark](https://github.com/MassTransit/MassTransit-Benchmark) on a P4 instance. It is also critical that your application is in the same DC as the ASB instance. From a home test using a 1Gb fiber connection we could not get over 600/second. When running in the same DC as the ASB we were able to acheive 6k/second. This test was done with one instance writing to ASB and another instance reading from ASB, as adding consumption over the same AMQP connection killed throughput.