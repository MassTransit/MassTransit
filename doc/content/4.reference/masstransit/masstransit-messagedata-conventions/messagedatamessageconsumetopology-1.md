---

title: MessageDataMessageConsumeTopology<T>

---

# MessageDataMessageConsumeTopology\<T\>

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataMessageConsumeTopology<T> : IMessageConsumeTopology<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataMessageConsumeTopology\<T\>](../masstransit-messagedata-conventions/messagedatamessageconsumetopology-1)<br/>
Implements [IMessageConsumeTopology\<T\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)

## Constructors

### **MessageDataMessageConsumeTopology(IMessageInitializer\<T\>)**

```csharp
public MessageDataMessageConsumeTopology(IMessageInitializer<T> initializer)
```

#### Parameters

`initializer` [IMessageInitializer\<T\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>

## Methods

### **Apply(ITopologyPipeBuilder\<ConsumeContext\<T\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<ConsumeContext<T>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>
