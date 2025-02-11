---

title: IOutboxOptionsConfigurator

---

# IOutboxOptionsConfigurator

Namespace: MassTransit

```csharp
public interface IOutboxOptionsConfigurator
```

## Properties

### **MessageDeliveryLimit**

The number of messages to deliver at a time from the outbox to the broker

```csharp
public abstract int MessageDeliveryLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MessageDeliveryTimeout**

Transport Send timeout when delivering messages to the transport

```csharp
public abstract TimeSpan MessageDeliveryTimeout { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
