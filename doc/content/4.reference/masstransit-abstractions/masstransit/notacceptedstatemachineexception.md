---

title: NotAcceptedStateMachineException

---

# NotAcceptedStateMachineException

Namespace: MassTransit

```csharp
public class NotAcceptedStateMachineException : SagaException, ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [MassTransitException](../masstransit/masstransitexception) → [SagaException](../masstransit/sagaexception) → [NotAcceptedStateMachineException](../masstransit/notacceptedstatemachineexception)<br/>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)

## Properties

### **SagaType**

```csharp
public Type SagaType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase)<br/>

### **Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br/>

### **InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **NotAcceptedStateMachineException(Type, Type, Guid, String, Exception)**

```csharp
public NotAcceptedStateMachineException(Type sagaType, Type messageType, Guid correlationId, string currentState, Exception exception)
```

#### Parameters

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`currentState` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
