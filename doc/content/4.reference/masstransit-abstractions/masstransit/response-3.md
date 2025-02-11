---

title: Response<T1, T2, T3>

---

# Response\<T1, T2, T3\>

Namespace: MassTransit

The response for a request that accepts two response types, which can be matched easily or converted back into a tuple of
 tasks.

```csharp
public struct Response<T1, T2, T3>
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Response\<T1, T2, T3\>](../masstransit/response-3)<br/>
Implements [Response](../masstransit/response), [MessageContext](../masstransit/messagecontext)

## Properties

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **Message**

```csharp
public object Message { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **Response(Task\<Response\<T1\>\>, Task\<Response\<T2\>\>, Task\<Response\<T3\>\>)**

```csharp
public Response(Task<Response<T1>> response1, Task<Response<T2>> response2, Task<Response<T3>> response3)
```

#### Parameters

`response1` [Task\<Response\<T1\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`response2` [Task\<Response\<T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`response3` [Task\<Response\<T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **Is(Response\<T1\>)**

```csharp
public bool Is(out Response<T1> result)
```

#### Parameters

`result` [Response\<T1\>](../masstransit/response-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Is(Response\<T2\>)**

```csharp
public bool Is(out Response<T2> result)
```

#### Parameters

`result` [Response\<T2\>](../masstransit/response-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Is(Response\<T3\>)**

```csharp
public bool Is(out Response<T3> result)
```

#### Parameters

`result` [Response\<T3\>](../masstransit/response-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Is\<T\>(Response\<T\>)**

```csharp
public bool Is<T>(out Response<T> result)
```

#### Type Parameters

`T`<br/>

#### Parameters

`result` [Response\<T\>](../masstransit/response-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Deconstruct(Task\<Response\<T1\>\>, Task\<Response\<T2\>\>, Task\<Response\<T3\>\>)**

```csharp
public void Deconstruct(out Task<Response<T1>> r1, out Task<Response<T2>> r2, out Task<Response<T3>> r3)
```

#### Parameters

`r1` [Task\<Response\<T1\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`r2` [Task\<Response\<T2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

`r3` [Task\<Response\<T3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
