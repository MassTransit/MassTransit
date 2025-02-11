---

title: MessageDataConsumeTopologyConvention

---

# MessageDataConsumeTopologyConvention

Namespace: MassTransit.MessageData.Conventions

```csharp
public class MessageDataConsumeTopologyConvention : IConsumeTopologyConvention, IMessageConsumeTopologyConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataConsumeTopologyConvention](../masstransit-messagedata-conventions/messagedataconsumetopologyconvention)<br/>
Implements [IConsumeTopologyConvention](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconvention), [IMessageConsumeTopologyConvention](../../masstransit-abstractions/masstransit-configuration/imessageconsumetopologyconvention)

## Constructors

### **MessageDataConsumeTopologyConvention(IMessageDataRepository)**

```csharp
public MessageDataConsumeTopologyConvention(IMessageDataRepository repository)
```

#### Parameters

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

## Methods

### **TryGetMessageConsumeTopologyConvention\<T\>(IMessageConsumeTopologyConvention\<T\>)**

```csharp
public bool TryGetMessageConsumeTopologyConvention<T>(out IMessageConsumeTopologyConvention<T> convention)
```

#### Type Parameters

`T`<br/>

#### Parameters

`convention` [IMessageConsumeTopologyConvention\<T\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumetopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
