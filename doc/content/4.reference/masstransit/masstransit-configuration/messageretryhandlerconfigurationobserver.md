---

title: MessageRetryHandlerConfigurationObserver

---

# MessageRetryHandlerConfigurationObserver

Namespace: MassTransit.Configuration

Configures a message retry for a handler, on the handler configurator, which is constrained to
 the message types for that handler, and only applies to the handler.

```csharp
public class MessageRetryHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageRetryHandlerConfigurationObserver](../masstransit-configuration/messageretryhandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Constructors

### **MessageRetryHandlerConfigurationObserver(CancellationToken, Action\<IRetryConfigurator\>)**

```csharp
public MessageRetryHandlerConfigurationObserver(CancellationToken cancellationToken, Action<IRetryConfigurator> configure)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
