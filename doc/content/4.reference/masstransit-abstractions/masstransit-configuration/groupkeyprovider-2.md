---

title: GroupKeyProvider<TMessage, TKey>

---

# GroupKeyProvider\<TMessage, TKey\>

Namespace: MassTransit.Configuration

```csharp
public class GroupKeyProvider<TMessage, TKey> : IGroupKeyProvider<TMessage, TKey>
```

#### Type Parameters

`TMessage`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GroupKeyProvider\<TMessage, TKey\>](../masstransit-configuration/groupkeyprovider-2)<br/>
Implements [IGroupKeyProvider\<TMessage, TKey\>](../masstransit/igroupkeyprovider-2)

## Constructors

### **GroupKeyProvider(Func\<ConsumeContext\<TMessage\>, TKey\>)**

```csharp
public GroupKeyProvider(Func<ConsumeContext<TMessage>, TKey> provider)
```

#### Parameters

`provider` [Func\<ConsumeContext\<TMessage\>, TKey\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **TryGetKey(ConsumeContext\<TMessage\>, TKey)**

```csharp
public bool TryGetKey(ConsumeContext<TMessage> context, out TKey key)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`key` TKey<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
