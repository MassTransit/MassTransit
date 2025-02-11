---

title: IBusTopology

---

# IBusTopology

Namespace: MassTransit

```csharp
public interface IBusTopology
```

## Properties

### **PublishTopology**

```csharp
public abstract IPublishTopology PublishTopology { get; }
```

#### Property Value

[IPublishTopology](../masstransit/ipublishtopology)<br/>

### **SendTopology**

```csharp
public abstract ISendTopology SendTopology { get; }
```

#### Property Value

[ISendTopology](../masstransit/isendtopology)<br/>

## Methods

### **Publish\<T\>()**

Returns the publish topology for the specified message type

```csharp
IMessagePublishTopology<T> Publish<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessagePublishTopology\<T\>](../masstransit/imessagepublishtopology-1)<br/>

### **Send\<T\>()**

Returns the send topology for the specified message type

```csharp
IMessageSendTopology<T> Send<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageSendTopology\<T\>](../masstransit/imessagesendtopology-1)<br/>

### **Message\<T\>()**

Returns the message topology for the specified message type

```csharp
IMessageTopology<T> Message<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageTopology\<T\>](../masstransit/imessagetopology-1)<br/>

### **TryGetPublishAddress(Type, Uri)**

Returns the destination address for the specified message type, as a short address.

```csharp
bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The message type

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPublishAddress\<T\>(Uri)**

Returns the destination address for the specified message type, as a short address.

```csharp
bool TryGetPublishAddress<T>(out Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
