---

title: IExecuteActivityScopeProvider<TActivity, TArguments>

---

# IExecuteActivityScopeProvider\<TActivity, TArguments\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IExecuteActivityScopeProvider<TActivity, TArguments> : IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetScope(ExecuteContext\<TArguments\>)**

```csharp
ValueTask<IExecuteScopeContext<TArguments>> GetScope(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

#### Returns

[ValueTask\<IExecuteScopeContext\<TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetActivityScope(ExecuteContext\<TArguments\>)**

```csharp
ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetActivityScope(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

#### Returns

[ValueTask\<IExecuteActivityScopeContext\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>
