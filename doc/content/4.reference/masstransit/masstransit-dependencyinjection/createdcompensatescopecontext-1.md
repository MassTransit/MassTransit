---

title: CreatedCompensateScopeContext<TLog>

---

# CreatedCompensateScopeContext\<TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public class CreatedCompensateScopeContext<TLog> : ICompensateScopeContext<TLog>, IAsyncDisposable
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CreatedCompensateScopeContext\<TLog\>](../masstransit-dependencyinjection/createdcompensatescopecontext-1)<br/>
Implements [ICompensateScopeContext\<TLog\>](../masstransit-dependencyinjection/icompensatescopecontext-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public CompensateContext<TLog> Context { get; }
```

#### Property Value

[CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

## Constructors

### **CreatedCompensateScopeContext(IServiceScope, CompensateContext\<TLog\>, IDisposable)**

```csharp
public CreatedCompensateScopeContext(IServiceScope scope, CompensateContext<TLog> context, IDisposable disposable)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

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
