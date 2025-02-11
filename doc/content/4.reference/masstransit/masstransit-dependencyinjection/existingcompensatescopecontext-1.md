---

title: ExistingCompensateScopeContext<TLog>

---

# ExistingCompensateScopeContext\<TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingCompensateScopeContext<TLog> : ICompensateScopeContext<TLog>, IAsyncDisposable
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingCompensateScopeContext\<TLog\>](../masstransit-dependencyinjection/existingcompensatescopecontext-1)<br/>
Implements [ICompensateScopeContext\<TLog\>](../masstransit-dependencyinjection/icompensatescopecontext-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public CompensateContext<TLog> Context { get; }
```

#### Property Value

[CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

## Constructors

### **ExistingCompensateScopeContext(CompensateContext\<TLog\>, IServiceScope, IDisposable)**

```csharp
public ExistingCompensateScopeContext(CompensateContext<TLog> context, IServiceScope scope, IDisposable disposable)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

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
