---

title: RequestSettings<TSaga, TRequest, TResponse>

---

# RequestSettings\<TSaga, TRequest, TResponse\>

Namespace: MassTransit

The request settings include the address of the request handler, as well as the timeout to use
 for requests.

```csharp
public interface RequestSettings<TSaga, TRequest, TResponse>
```

#### Type Parameters

`TSaga`<br/>

`TRequest`<br/>

`TResponse`<br/>

## Properties

### **ServiceAddress**

The endpoint address of the service that handles the request

```csharp
public abstract Uri ServiceAddress { get; }
```

#### Property Value

Uri<br/>

### **Timeout**

The timeout period before the request times out

```csharp
public abstract TimeSpan Timeout { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ClearRequestIdOnFaulted**

If true, the requestId is cleared when Faulted is triggered

```csharp
public abstract bool ClearRequestIdOnFaulted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TimeToLive**

If specified, the TimeToLive is set on the outgoing request

```csharp
public abstract Nullable<TimeSpan> TimeToLive { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Completed**

Configures the behavior of the Completed event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TSaga, TResponse>> Completed { get; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TSaga, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Faulted**

Configures the behavior of the Faulted event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TSaga, Fault<TRequest>>> Faulted { get; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TSaga, Fault\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **TimeoutExpired**

Configures the behavior of the Timeout Expired event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TSaga, RequestTimeoutExpired<TRequest>>> TimeoutExpired { get; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TSaga, RequestTimeoutExpired\<TRequest\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
