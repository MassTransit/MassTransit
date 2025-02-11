---

title: ResponseHandlerConfigurator<TResponse>

---

# ResponseHandlerConfigurator\<TResponse\>

Namespace: MassTransit.Clients

Connects a handler to the inbound pipe of the receive endpoint

```csharp
public class ResponseHandlerConfigurator<TResponse> : IHandlerConfigurator<TResponse>, IConsumeConfigurator, IHandlerConfigurationObserverConnector, IPipeConfigurator<ConsumeContext<TResponse>>
```

#### Type Parameters

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ResponseHandlerConfigurator\<TResponse\>](../masstransit-clients/responsehandlerconfigurator-1)<br/>
Implements [IHandlerConfigurator\<TResponse\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IPipeConfigurator\<ConsumeContext\<TResponse\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Constructors

### **ResponseHandlerConfigurator(TaskScheduler, MessageHandler\<TResponse\>, Task)**

```csharp
public ResponseHandlerConfigurator(TaskScheduler taskScheduler, MessageHandler<TResponse> handler, Task requestTask)
```

#### Parameters

`taskScheduler` [TaskScheduler](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler)<br/>

`handler` [MessageHandler\<TResponse\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`requestTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TResponse\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TResponse>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TResponse\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver)**

```csharp
public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
```

#### Parameters

`observer` [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Connect(IRequestPipeConnector, Guid)**

```csharp
public HandlerConnectHandle<TResponse> Connect(IRequestPipeConnector connector, Guid requestId)
```

#### Parameters

`connector` [IRequestPipeConnector](../../masstransit-abstractions/masstransit/irequestpipeconnector)<br/>

`requestId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[HandlerConnectHandle\<TResponse\>](../masstransit-clients/handlerconnecthandle-1)<br/>
