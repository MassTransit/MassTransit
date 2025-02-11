---

title: ExistingCompensateActivityScopeContext<TActivity, TLog>

---

# ExistingCompensateActivityScopeContext\<TActivity, TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public class ExistingCompensateActivityScopeContext<TActivity, TLog> : ICompensateActivityScopeContext<TActivity, TLog>, IAsyncDisposable
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExistingCompensateActivityScopeContext\<TActivity, TLog\>](../masstransit-dependencyinjection/existingcompensateactivityscopecontext-2)<br/>
Implements [ICompensateActivityScopeContext\<TActivity, TLog\>](../masstransit-dependencyinjection/icompensateactivityscopecontext-2), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public CompensateActivityContext<TActivity, TLog> Context { get; }
```

#### Property Value

[CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

## Constructors

### **ExistingCompensateActivityScopeContext(CompensateActivityContext\<TActivity, TLog\>, IServiceScope, IDisposable)**

```csharp
public ExistingCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, IServiceScope scope, IDisposable disposable)
```

#### Parameters

`context` [CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

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
