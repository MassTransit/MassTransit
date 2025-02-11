---

title: ReceiveFault

---

# ReceiveFault

Namespace: MassTransit

Published when a message fails to deserialize at the endpoint

```csharp
public interface ReceiveFault : Fault
```

Implements [Fault](../masstransit/fault)

## Properties

### **ContentType**

The specified content type of the message by the transport

```csharp
public abstract string ContentType { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
