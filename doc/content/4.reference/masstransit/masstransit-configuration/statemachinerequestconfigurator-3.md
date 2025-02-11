---

title: StateMachineRequestConfigurator<TInstance, TRequest, TResponse>

---

# StateMachineRequestConfigurator\<TInstance, TRequest, TResponse\>

Namespace: MassTransit.Configuration

```csharp
public class StateMachineRequestConfigurator<TInstance, TRequest, TResponse> : IRequestConfigurator<TInstance, TRequest, TResponse>, IRequestConfigurator, RequestSettings<TInstance, TRequest, TResponse>
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineRequestConfigurator\<TInstance, TRequest, TResponse\>](../masstransit-configuration/statemachinerequestconfigurator-3)<br/>
Implements [IRequestConfigurator\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/irequestconfigurator-3), [IRequestConfigurator](../../masstransit-abstractions/masstransit/irequestconfigurator), [RequestSettings\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/requestsettings-3)

## Properties

### **Settings**

```csharp
public RequestSettings<TInstance, TRequest, TResponse> Settings { get; }
```

#### Property Value

[RequestSettings\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/requestsettings-3)<br/>

### **ServiceAddress**

```csharp
public Uri ServiceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **Timeout**

```csharp
public TimeSpan Timeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ClearRequestIdOnFaulted**

```csharp
public bool ClearRequestIdOnFaulted { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TimeToLive**

```csharp
public Nullable<TimeSpan> TimeToLive { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Completed**

```csharp
public Action<IEventCorrelationConfigurator<TInstance, TResponse>> Completed { get; set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Faulted**

```csharp
public Action<IEventCorrelationConfigurator<TInstance, Fault<TRequest>>> Faulted { get; set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **TimeoutExpired**

```csharp
public Action<IEventCorrelationConfigurator<TInstance, RequestTimeoutExpired<TRequest>>> TimeoutExpired { get; set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, RequestTimeoutExpired\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Constructors

### **StateMachineRequestConfigurator()**

```csharp
public StateMachineRequestConfigurator()
```
