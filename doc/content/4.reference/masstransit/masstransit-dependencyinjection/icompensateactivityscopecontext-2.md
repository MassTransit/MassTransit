---

title: ICompensateActivityScopeContext<TActivity, TLog>

---

# ICompensateActivityScopeContext\<TActivity, TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface ICompensateActivityScopeContext<TActivity, TLog> : IAsyncDisposable
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract CompensateActivityContext<TActivity, TLog> Context { get; }
```

#### Property Value

[CompensateActivityContext\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/compensateactivitycontext-2)<br/>

## Methods

### **GetService\<T\>()**

```csharp
T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>
