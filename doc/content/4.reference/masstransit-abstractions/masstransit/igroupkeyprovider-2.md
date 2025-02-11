---

title: IGroupKeyProvider<TMessage, TKey>

---

# IGroupKeyProvider\<TMessage, TKey\>

Namespace: MassTransit

```csharp
public interface IGroupKeyProvider<TMessage, TKey>
```

#### Type Parameters

`TMessage`<br/>

`TKey`<br/>

## Methods

### **TryGetKey(ConsumeContext\<TMessage\>, TKey)**

```csharp
bool TryGetKey(ConsumeContext<TMessage> context, out TKey key)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`key` TKey<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
