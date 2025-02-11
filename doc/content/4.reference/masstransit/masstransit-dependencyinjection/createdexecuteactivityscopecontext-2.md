---

title: CreatedExecuteActivityScopeContext<TActivity, TArguments>

---

# CreatedExecuteActivityScopeContext\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public class CreatedExecuteActivityScopeContext<TActivity, TArguments> : IExecuteActivityScopeContext<TActivity, TArguments>, IAsyncDisposable
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CreatedExecuteActivityScopeContext\<TActivity, TArguments\>](../masstransit-dependencyinjection/createdexecuteactivityscopecontext-2)<br/>
Implements [IExecuteActivityScopeContext\<TActivity, TArguments\>](../masstransit-dependencyinjection/iexecuteactivityscopecontext-2), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public ExecuteActivityContext<TActivity, TArguments> Context { get; }
```

#### Property Value

[ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

## Constructors

### **CreatedExecuteActivityScopeContext(ExecuteActivityContext\<TActivity, TArguments\>, IServiceScope, IDisposable)**

```csharp
public CreatedExecuteActivityScopeContext(ExecuteActivityContext<TActivity, TArguments> context, IServiceScope scope, IDisposable disposable)
```

#### Parameters

`context` [ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

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
