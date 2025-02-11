---

title: IPublishTransportProvider

---

# IPublishTransportProvider

Namespace: MassTransit.Transports

```csharp
public interface IPublishTransportProvider
```

## Methods

### **GetPublishTransport\<T\>(Uri)**

```csharp
Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
