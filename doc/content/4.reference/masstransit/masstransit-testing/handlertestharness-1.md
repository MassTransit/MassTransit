---

title: HandlerTestHarness<TMessage>

---

# HandlerTestHarness\<TMessage\>

Namespace: MassTransit.Testing

```csharp
public class HandlerTestHarness<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerTestHarness\<TMessage\>](../masstransit-testing/handlertestharness-1)

## Properties

### **Consumed**

```csharp
public IReceivedMessageList<TMessage> Consumed { get; }
```

#### Property Value

[IReceivedMessageList\<TMessage\>](../masstransit-testing/ireceivedmessagelist-1)<br/>

## Constructors

### **HandlerTestHarness(BusTestHarness, MessageHandler\<TMessage\>)**

```csharp
public HandlerTestHarness(BusTestHarness testHarness, MessageHandler<TMessage> handler)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`handler` [MessageHandler\<TMessage\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>
