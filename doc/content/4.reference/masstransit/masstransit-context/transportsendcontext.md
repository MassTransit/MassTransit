---

title: TransportSendContext

---

# TransportSendContext

Namespace: MassTransit.Context

```csharp
public interface TransportSendContext : PublishContext, SendContext, PipeContext
```

Implements [PublishContext](../../masstransit-abstractions/masstransit/publishcontext), [SendContext](../../masstransit-abstractions/masstransit/sendcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Methods

### **WritePropertiesTo(IDictionary\<String, Object\>)**

```csharp
void WritePropertiesTo(IDictionary<string, object> properties)
```

#### Parameters

`properties` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **ReadPropertiesFrom(IReadOnlyDictionary\<String, Object\>)**

```csharp
void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
```

#### Parameters

`properties` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>
