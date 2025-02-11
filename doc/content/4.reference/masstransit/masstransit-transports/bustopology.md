---

title: BusTopology

---

# BusTopology

Namespace: MassTransit.Transports

```csharp
public abstract class BusTopology : IBusTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusTopology](../masstransit-transports/bustopology)<br/>
Implements [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)

## Properties

### **PublishTopology**

```csharp
public IPublishTopology PublishTopology { get; }
```

#### Property Value

[IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **SendTopology**

```csharp
public ISendTopology SendTopology { get; }
```

#### Property Value

[ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

## Methods

### **Publish\<T\>()**

```csharp
public IMessagePublishTopology<T> Publish<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishTopology\<T\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1)<br/>

### **Send\<T\>()**

```csharp
public IMessageSendTopology<T> Send<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageSendTopology\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopology-1)<br/>

### **Message\<T\>()**

```csharp
public IMessageTopology<T> Message<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageTopology\<T\>](../../masstransit-abstractions/masstransit/imessagetopology-1)<br/>

### **TryGetPublishAddress(Type, Uri)**

```csharp
public bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPublishAddress\<T\>(Uri)**

```csharp
public bool TryGetPublishAddress<T>(out Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
