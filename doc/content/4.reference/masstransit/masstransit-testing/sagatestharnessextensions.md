---

title: SagaTestHarnessExtensions

---

# SagaTestHarnessExtensions

Namespace: MassTransit.Testing

```csharp
public static class SagaTestHarnessExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaTestHarnessExtensions](../masstransit-testing/sagatestharnessextensions)

## Methods

### **Saga\<T\>(BusTestHarness, String)**

```csharp
public static SagaTestHarness<T> Saga<T>(BusTestHarness harness, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[SagaTestHarness\<T\>](../masstransit-testing/sagatestharness-1)<br/>

### **Saga\<T\>(BusTestHarness, ISagaRepository\<T\>, String)**

```csharp
public static SagaTestHarness<T> Saga<T>(BusTestHarness harness, ISagaRepository<T> repository, string queueName)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`repository` [ISagaRepository\<T\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[SagaTestHarness\<T\>](../masstransit-testing/sagatestharness-1)<br/>
