---

title: ExistingConsumeScopeContext<TMessage>

---

# ExistingConsumeScopeContext\<TMessage\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingConsumeScopeContext<TMessage> : IConsumeScopeContext<TMessage>, IAsyncDisposable
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingConsumeScopeContext\<TMessage\>](../masstransit-dependencyinjection/existingconsumescopecontext-1)<br/>
Implements [IConsumeScopeContext\<TMessage\>](../masstransit-dependencyinjection/iconsumescopecontext-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ConsumeContext<TMessage> Context { get; }
```

#### Property Value

[ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

## Constructors

### **ExistingConsumeScopeContext(ConsumeContext\<TMessage\>, IServiceScope, IDisposable, ISetScopedConsumeContext)**

```csharp
public ExistingConsumeScopeContext(ConsumeContext<TMessage> context, IServiceScope scope, IDisposable disposable, ISetScopedConsumeContext setter)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`scope` IServiceScope<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **GetService\<T\>()**

```csharp
public T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>

### **CreateInstance\<T\>(Object[])**

```csharp
public T CreateInstance<T>(Object[] arguments)
```

#### Type Parameters

`T`<br/>

#### Parameters

`arguments` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

T<br/>

### **PushConsumeContext(ConsumeContext)**

```csharp
public IDisposable PushConsumeContext(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
