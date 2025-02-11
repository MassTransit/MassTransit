---

title: CreatedConsumerConsumeScopeContext<TConsumer, T>

---

# CreatedConsumerConsumeScopeContext\<TConsumer, T\>

Namespace: MassTransit.DependencyInjection

```csharp
public class CreatedConsumerConsumeScopeContext<TConsumer, T> : IConsumerConsumeScopeContext<TConsumer, T>, IAsyncDisposable
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CreatedConsumerConsumeScopeContext\<TConsumer, T\>](../masstransit-dependencyinjection/createdconsumerconsumescopecontext-2)<br/>
Implements [IConsumerConsumeScopeContext\<TConsumer, T\>](../masstransit-dependencyinjection/iconsumerconsumescopecontext-2), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ConsumerConsumeContext<TConsumer, T> Context { get; }
```

#### Property Value

[ConsumerConsumeContext\<TConsumer, T\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

## Constructors

### **CreatedConsumerConsumeScopeContext(IServiceScope, ConsumerConsumeContext\<TConsumer, T\>, IDisposable)**

```csharp
public CreatedConsumerConsumeScopeContext(IServiceScope scope, ConsumerConsumeContext<TConsumer, T> context, IDisposable disposable)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [ConsumerConsumeContext\<TConsumer, T\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
