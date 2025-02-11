---

title: StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3>

---

# StateMachineRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>

Namespace: MassTransit.Configuration

```csharp
public class StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3> : StateMachineRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>, IRequestConfigurator<TInstance, TRequest, TResponse>, IRequestConfigurator, RequestSettings<TInstance, TRequest, TResponse>, IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>, RequestSettings<TInstance, TRequest, TResponse, TResponse2>, IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3>, RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3>
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

`TResponse2`<br/>

`TResponse3`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineRequestConfigurator\<TInstance, TRequest, TResponse\>](../masstransit-configuration/statemachinerequestconfigurator-3) → [StateMachineRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>](../masstransit-configuration/statemachinerequestconfigurator-4) → [StateMachineRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../masstransit-configuration/statemachinerequestconfigurator-5)<br/>
Implements [IRequestConfigurator\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/irequestconfigurator-3), [IRequestConfigurator](../../masstransit-abstractions/masstransit/irequestconfigurator), [RequestSettings\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/requestsettings-3), [IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>](../../masstransit-abstractions/masstransit/irequestconfigurator-4), [RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>](../../masstransit-abstractions/masstransit/requestsettings-4), [IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../../masstransit-abstractions/masstransit/irequestconfigurator-5), [RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../../masstransit-abstractions/masstransit/requestsettings-5)

## Properties

### **Settings**

```csharp
public RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3> Settings { get; }
```

#### Property Value

[RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../../masstransit-abstractions/masstransit/requestsettings-5)<br/>

### **Completed3**

```csharp
public Action<IEventCorrelationConfigurator<TInstance, TResponse3>> Completed3 { get; set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TResponse3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Settings**

```csharp
public RequestSettings<TInstance, TRequest, TResponse, TResponse2> Settings { get; }
```

#### Property Value

[RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>](../../masstransit-abstractions/masstransit/requestsettings-4)<br/>

### **Completed2**

```csharp
public Action<IEventCorrelationConfigurator<TInstance, TResponse2>> Completed2 { get; set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TResponse2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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
