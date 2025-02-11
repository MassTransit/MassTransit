---

title: ISqlBusTopology

---

# ISqlBusTopology

Namespace: MassTransit

```csharp
public interface ISqlBusTopology : IBusTopology
```

Implements [IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)

## Properties

### **PublishTopology**

```csharp
public abstract ISqlPublishTopology PublishTopology { get; }
```

#### Property Value

[ISqlPublishTopology](../masstransit/isqlpublishtopology)<br/>

### **SendTopology**

```csharp
public abstract ISqlSendTopology SendTopology { get; }
```

#### Property Value

[ISqlSendTopology](../masstransit/isqlsendtopology)<br/>

## Methods

### **Publish\<T\>()**

```csharp
ISqlMessagePublishTopology<T> Publish<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessagePublishTopology\<T\>](../masstransit/isqlmessagepublishtopology-1)<br/>

### **Send\<T\>()**

```csharp
ISqlMessageSendTopology<T> Send<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessageSendTopology\<T\>](../masstransit/isqlmessagesendtopology-1)<br/>
