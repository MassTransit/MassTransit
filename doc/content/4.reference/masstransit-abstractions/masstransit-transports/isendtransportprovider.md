---

title: ISendTransportProvider

---

# ISendTransportProvider

Namespace: MassTransit.Transports

```csharp
public interface ISendTransportProvider
```

## Methods

### **GetSendTransport(Uri)**

```csharp
Task<ISendTransport> GetSendTransport(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NormalizeAddress(Uri)**

```csharp
Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>
