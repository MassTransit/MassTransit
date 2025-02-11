---

title: ExistingConsumerConsumeScopeContext<TConsumer, T>

---

# ExistingConsumerConsumeScopeContext\<TConsumer, T\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingConsumerConsumeScopeContext<TConsumer, T> : IConsumerConsumeScopeContext<TConsumer, T>, IAsyncDisposable
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingConsumerConsumeScopeContext\<TConsumer, T\>](../masstransit-dependencyinjection/existingconsumerconsumescopecontext-2)<br/>
Implements [IConsumerConsumeScopeContext\<TConsumer, T\>](../masstransit-dependencyinjection/iconsumerconsumescopecontext-2), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ConsumerConsumeContext<TConsumer, T> Context { get; }
```

#### Property Value

[ConsumerConsumeContext\<TConsumer, T\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

## Constructors

### **ExistingConsumerConsumeScopeContext(ConsumerConsumeContext\<TConsumer, T\>, IDisposable)**

```csharp
public ExistingConsumerConsumeScopeContext(ConsumerConsumeContext<TConsumer, T> context, IDisposable disposable)
```

#### Parameters

`context` [ConsumerConsumeContext\<TConsumer, T\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
