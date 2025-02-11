---

title: ExistingExecuteScopeContext<TArguments>

---

# ExistingExecuteScopeContext\<TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingExecuteScopeContext<TArguments> : IExecuteScopeContext<TArguments>, IAsyncDisposable
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingExecuteScopeContext\<TArguments\>](../masstransit-dependencyinjection/existingexecutescopecontext-1)<br/>
Implements [IExecuteScopeContext\<TArguments\>](../masstransit-dependencyinjection/iexecutescopecontext-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ExecuteContext<TArguments> Context { get; }
```

#### Property Value

[ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

## Constructors

### **ExistingExecuteScopeContext(ExecuteContext\<TArguments\>, IServiceScope, IDisposable)**

```csharp
public ExistingExecuteScopeContext(ExecuteContext<TArguments> context, IServiceScope scope, IDisposable disposable)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

`scope` IServiceScope<br/>

`disposable` [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

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
