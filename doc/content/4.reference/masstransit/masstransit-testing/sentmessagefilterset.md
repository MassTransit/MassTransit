---

title: SentMessageFilterSet

---

# SentMessageFilterSet

Namespace: MassTransit.Testing

```csharp
public class SentMessageFilterSet : FilterSet<ISentMessage>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterSet\<ISentMessage\>](../masstransit-testing/filterset-1) → [SentMessageFilterSet](../masstransit-testing/sentmessagefilterset)

## Constructors

### **SentMessageFilterSet()**

```csharp
public SentMessageFilterSet()
```

## Methods

### **Add\<T\>()**

```csharp
public SentMessageFilterSet Add<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[SentMessageFilterSet](../masstransit-testing/sentmessagefilterset)<br/>

### **Add\<T\>(FilterDelegate\<ISentMessage\<T\>\>)**

```csharp
public SentMessageFilterSet Add<T>(FilterDelegate<ISentMessage<T>> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<ISentMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

#### Returns

[SentMessageFilterSet](../masstransit-testing/sentmessagefilterset)<br/>
