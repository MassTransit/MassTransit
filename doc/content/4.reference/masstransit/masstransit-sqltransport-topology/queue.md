---

title: Queue

---

# Queue

Namespace: MassTransit.SqlTransport.Topology

```csharp
public interface Queue
```

## Properties

### **QueueName**

```csharp
public abstract string QueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AutoDeleteOnIdle**

Idle time before queue should be deleted (consumer-idle, not producer)

```csharp
public abstract Nullable<TimeSpan> AutoDeleteOnIdle { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MaxDeliveryCount**

Specify the maximum delivery count for messages in the queue

```csharp
public abstract Nullable<int> MaxDeliveryCount { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
