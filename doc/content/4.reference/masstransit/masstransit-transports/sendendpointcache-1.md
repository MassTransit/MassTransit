---

title: SendEndpointCache<TKey>

---

# SendEndpointCache\<TKey\>

Namespace: MassTransit.Transports

Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)

```csharp
public class SendEndpointCache<TKey> : ISendEndpointCache<TKey>
```

#### Type Parameters

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointCache\<TKey\>](../masstransit-transports/sendendpointcache-1)<br/>
Implements [ISendEndpointCache\<TKey\>](../masstransit-transports/isendendpointcache-1)

## Constructors

### **SendEndpointCache()**

```csharp
public SendEndpointCache()
```

## Methods

### **GetSendEndpoint(TKey, SendEndpointFactory\<TKey\>)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory)
```

#### Parameters

`key` TKey<br/>

`factory` [SendEndpointFactory\<TKey\>](../masstransit-transports/sendendpointfactory-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
