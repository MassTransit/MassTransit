---

title: IReceivedMessage

---

# IReceivedMessage

Namespace: MassTransit.Testing

```csharp
public interface IReceivedMessage : IAsyncListElement
```

Implements [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **Context**

```csharp
public abstract ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **StartTime**

```csharp
public abstract DateTime StartTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ElapsedTime**

```csharp
public abstract TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Exception**

```csharp
public abstract Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **MessageType**

```csharp
public abstract Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ShortTypeName**

```csharp
public abstract string ShortTypeName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageObject**

```csharp
public abstract object MessageObject { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
