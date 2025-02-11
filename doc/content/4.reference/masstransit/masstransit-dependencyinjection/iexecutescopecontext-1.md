---

title: IExecuteScopeContext<TArguments>

---

# IExecuteScopeContext\<TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IExecuteScopeContext<TArguments> : IAsyncDisposable
```

#### Type Parameters

`TArguments`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract ExecuteContext<TArguments> Context { get; }
```

#### Property Value

[ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

## Methods

### **GetService\<T\>()**

```csharp
T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>
