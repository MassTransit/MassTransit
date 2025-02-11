---

title: ICompensateActivityScopeProvider<TActivity, TLog>

---

# ICompensateActivityScopeProvider\<TActivity, TLog\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface ICompensateActivityScopeProvider<TActivity, TLog> : IProbeSite
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetScope(CompensateContext\<TLog\>)**

```csharp
ValueTask<ICompensateScopeContext<TLog>> GetScope(CompensateContext<TLog> context)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

#### Returns

[ValueTask\<ICompensateScopeContext\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetActivityScope(CompensateContext\<TLog\>)**

```csharp
ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetActivityScope(CompensateContext<TLog> context)
```

#### Parameters

`context` [CompensateContext\<TLog\>](../../masstransit-abstractions/masstransit/compensatecontext-1)<br/>

#### Returns

[ValueTask\<ICompensateActivityScopeContext\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>
