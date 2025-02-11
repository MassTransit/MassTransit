---

title: HandlerTestHarnessExtensions

---

# HandlerTestHarnessExtensions

Namespace: MassTransit.Testing

```csharp
public static class HandlerTestHarnessExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerTestHarnessExtensions](../masstransit-testing/handlertestharnessextensions)

## Methods

### **Handler\<T\>(BusTestHarness, MessageHandler\<T\>)**

```csharp
public static HandlerTestHarness<T> Handler<T>(BusTestHarness harness, MessageHandler<T> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`handler` [MessageHandler\<T\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

#### Returns

[HandlerTestHarness\<T\>](../masstransit-testing/handlertestharness-1)<br/>

### **Handler\<T\>(BusTestHarness)**

```csharp
public static HandlerTestHarness<T> Handler<T>(BusTestHarness harness)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

#### Returns

[HandlerTestHarness\<T\>](../masstransit-testing/handlertestharness-1)<br/>
