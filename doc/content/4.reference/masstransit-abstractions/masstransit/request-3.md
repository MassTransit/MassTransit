---

title: Request<TSaga, TRequest, TResponse>

---

# Request\<TSaga, TRequest, TResponse\>

Namespace: MassTransit

A request is a state-machine based request configuration that includes
 the events and states related to the execution of a request.

```csharp
public interface Request<TSaga, TRequest, TResponse>
```

#### Type Parameters

`TSaga`<br/>

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

## Properties

### **Name**

The name of the request

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Settings**

The settings that are used for the request, including the timeout

```csharp
public abstract RequestSettings<TSaga, TRequest, TResponse> Settings { get; }
```

#### Property Value

[RequestSettings\<TSaga, TRequest, TResponse\>](../masstransit/requestsettings-3)<br/>

### **Completed**

The event that is raised when the request completes and the response is received

```csharp
public abstract Event<TResponse> Completed { get; set; }
```

#### Property Value

[Event\<TResponse\>](../masstransit/event-1)<br/>

### **Faulted**

The event raised when the request faults

```csharp
public abstract Event<Fault<TRequest>> Faulted { get; set; }
```

#### Property Value

[Event\<Fault\<TRequest\>\>](../masstransit/event-1)<br/>

### **TimeoutExpired**

The event raised when the request times out with no response received

```csharp
public abstract Event<RequestTimeoutExpired<TRequest>> TimeoutExpired { get; set; }
```

#### Property Value

[Event\<RequestTimeoutExpired\<TRequest\>\>](../masstransit/event-1)<br/>

### **Pending**

The state that is transitioned to once the request is pending

```csharp
public abstract State Pending { get; set; }
```

#### Property Value

[State](../masstransit/state)<br/>

## Methods

### **SetRequestId(TSaga, Nullable\<Guid\>)**

Sets the requestId on the instance using the configured property

```csharp
void SetRequestId(TSaga instance, Nullable<Guid> requestId)
```

#### Parameters

`instance` TSaga<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetRequestId(TSaga)**

Gets the requestId on the instance using the configured property

```csharp
Nullable<Guid> GetRequestId(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GenerateRequestId(TSaga)**

Generate a requestId, using either the CorrelationId of the saga, or a NewId

```csharp
Guid GenerateRequestId(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **SetSendContextHeaders(SendContext\<TRequest\>)**

Set the headers on the outgoing request [SendContext\<T\>](../masstransit/sendcontext-1)

```csharp
void SetSendContextHeaders(SendContext<TRequest> context)
```

#### Parameters

`context` [SendContext\<TRequest\>](../masstransit/sendcontext-1)<br/>
