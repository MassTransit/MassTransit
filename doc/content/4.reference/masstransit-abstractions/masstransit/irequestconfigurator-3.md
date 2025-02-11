---

title: IRequestConfigurator<TInstance, TRequest, TResponse>

---

# IRequestConfigurator\<TInstance, TRequest, TResponse\>

Namespace: MassTransit

```csharp
public interface IRequestConfigurator<TInstance, TRequest, TResponse> : IRequestConfigurator
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

Implements [IRequestConfigurator](../masstransit/irequestconfigurator)

## Properties

### **Completed**

Configure the behavior of the Completed event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, TResponse>> Completed { set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Faulted**

Configure the behavior of the Faulted event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, Fault<TRequest>>> Faulted { set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **TimeoutExpired**

Configure the behavior of the Faulted event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, RequestTimeoutExpired<TRequest>>> TimeoutExpired { set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, RequestTimeoutExpired\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
