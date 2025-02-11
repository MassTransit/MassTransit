---

title: ISendEndpointCache<TKey>

---

# ISendEndpointCache\<TKey\>

Namespace: MassTransit.Transports

```csharp
public interface ISendEndpointCache<TKey>
```

#### Type Parameters

`TKey`<br/>

## Methods

### **GetSendEndpoint(TKey, SendEndpointFactory\<TKey\>)**

Return a SendEndpoint from the cache, using the factory to create it if it doesn't exist in the cache.

```csharp
Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory)
```

#### Parameters

`key` TKey<br/>
The key for the endpoint

`factory` [SendEndpointFactory\<TKey\>](../masstransit-transports/sendendpointfactory-1)<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
