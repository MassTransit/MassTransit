---

title: ValueTypeGroupKeyProvider<TMessage, TKey>

---

# ValueTypeGroupKeyProvider\<TMessage, TKey\>

Namespace: MassTransit.Configuration

```csharp
public class ValueTypeGroupKeyProvider<TMessage, TKey> : IGroupKeyProvider<TMessage, TKey>
```

#### Type Parameters

`TMessage`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueTypeGroupKeyProvider\<TMessage, TKey\>](../masstransit-configuration/valuetypegroupkeyprovider-2)<br/>
Implements [IGroupKeyProvider\<TMessage, TKey\>](../masstransit/igroupkeyprovider-2)

## Constructors

### **ValueTypeGroupKeyProvider(Func\<ConsumeContext\<TMessage\>, Nullable\<TKey\>\>)**

```csharp
public ValueTypeGroupKeyProvider(Func<ConsumeContext<TMessage>, Nullable<TKey>> provider)
```

#### Parameters

`provider` [Func\<ConsumeContext\<TMessage\>, Nullable\<TKey\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
