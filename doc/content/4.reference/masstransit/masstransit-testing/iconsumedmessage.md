---

title: IConsumedMessage

---

# IConsumedMessage

Namespace: MassTransit.Testing

```csharp
public interface IConsumedMessage
```

## Properties

### **Context**

```csharp
public abstract ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

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
