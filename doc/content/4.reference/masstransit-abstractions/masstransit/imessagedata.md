---

title: IMessageData

---

# IMessageData

Namespace: MassTransit

```csharp
public interface IMessageData
```

## Properties

### **Address**

Returns the address of the message data

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **HasValue**

True if the value is present in the message, and not null

```csharp
public abstract bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
