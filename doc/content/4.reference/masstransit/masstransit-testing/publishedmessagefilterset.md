---

title: PublishedMessageFilterSet

---

# PublishedMessageFilterSet

Namespace: MassTransit.Testing

```csharp
public class PublishedMessageFilterSet : FilterSet<IPublishedMessage>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterSet\<IPublishedMessage\>](../masstransit-testing/filterset-1) → [PublishedMessageFilterSet](../masstransit-testing/publishedmessagefilterset)

## Constructors

### **PublishedMessageFilterSet()**

```csharp
public PublishedMessageFilterSet()
```

## Methods

### **Add\<T\>()**

```csharp
public PublishedMessageFilterSet Add<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[PublishedMessageFilterSet](../masstransit-testing/publishedmessagefilterset)<br/>

### **Add\<T\>(FilterDelegate\<IPublishedMessage\<T\>\>)**

```csharp
public PublishedMessageFilterSet Add<T>(FilterDelegate<IPublishedMessage<T>> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IPublishedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

#### Returns

[PublishedMessageFilterSet](../masstransit-testing/publishedmessagefilterset)<br/>
