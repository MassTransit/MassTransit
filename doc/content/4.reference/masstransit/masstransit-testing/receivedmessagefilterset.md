---

title: ReceivedMessageFilterSet

---

# ReceivedMessageFilterSet

Namespace: MassTransit.Testing

```csharp
public class ReceivedMessageFilterSet : FilterSet<IReceivedMessage>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterSet\<IReceivedMessage\>](../masstransit-testing/filterset-1) → [ReceivedMessageFilterSet](../masstransit-testing/receivedmessagefilterset)

## Constructors

### **ReceivedMessageFilterSet()**

```csharp
public ReceivedMessageFilterSet()
```

## Methods

### **Add\<T\>()**

```csharp
public ReceivedMessageFilterSet Add<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ReceivedMessageFilterSet](../masstransit-testing/receivedmessagefilterset)<br/>

### **Add\<T\>(FilterDelegate\<IReceivedMessage\<T\>\>)**

```csharp
public ReceivedMessageFilterSet Add<T>(FilterDelegate<IReceivedMessage<T>> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [FilterDelegate\<IReceivedMessage\<T\>\>](../masstransit-testing/filterdelegate-1)<br/>

#### Returns

[ReceivedMessageFilterSet](../masstransit-testing/receivedmessagefilterset)<br/>
