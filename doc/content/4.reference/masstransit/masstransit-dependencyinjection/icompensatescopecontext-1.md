---

title: ICompensateScopeContext<TLog>

---

# ICompensateScopeContext\<TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface ICompensateScopeContext<TLog> : IAsyncDisposable
```

#### Type Parameters

`TLog`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract CompensateContext<TLog> Context { get; }
```

#### Property Value

[CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

## Methods

### **GetService\<T\>()**

```csharp
T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>
