---

title: ISqlQueueConfigurator

---

# ISqlQueueConfigurator

Namespace: MassTransit

Configure a database transport queue

```csharp
public interface ISqlQueueConfigurator
```

## Properties

### **AutoDeleteOnIdle**

If specified, the queue will be automatically removed after no consumer activity within the specific idle period

```csharp
public abstract Nullable<TimeSpan> AutoDeleteOnIdle { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxDeliveryCount**

The maximum number of message delivery attempts by the transport before moving the message to the DLQ (defaults to 10)

```csharp
public abstract Nullable<int> MaxDeliveryCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
