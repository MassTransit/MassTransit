---

title: IExecuteActivityScopeContext<TActivity, TArguments>

---

# IExecuteActivityScopeContext\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IExecuteActivityScopeContext<TActivity, TArguments> : IAsyncDisposable
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract ExecuteActivityContext<TActivity, TArguments> Context { get; }
```

#### Property Value

[ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>

## Methods

### **GetService\<T\>()**

```csharp
T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>
