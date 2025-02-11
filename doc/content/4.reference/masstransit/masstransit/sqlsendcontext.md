---

title: SqlSendContext

---

# SqlSendContext

Namespace: MassTransit

```csharp
public interface SqlSendContext : SendContext, PipeContext, RoutingKeySendContext, PartitionKeySendContext
```

Implements [SendContext](../../masstransit-abstractions/masstransit/sendcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [RoutingKeySendContext](../../masstransit-abstractions/masstransit/routingkeysendcontext), [PartitionKeySendContext](../../masstransit-abstractions/masstransit/partitionkeysendcontext)

## Properties

### **TransportMessageId**

```csharp
public abstract Guid TransportMessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Priority**

```csharp
public abstract Nullable<short> Priority { set; }
```

#### Property Value

[Nullable\<Int16\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
