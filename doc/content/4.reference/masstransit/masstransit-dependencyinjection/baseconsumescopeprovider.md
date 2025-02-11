---

title: BaseConsumeScopeProvider

---

# BaseConsumeScopeProvider

Namespace: MassTransit.DependencyInjection

```csharp
public abstract class BaseConsumeScopeProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseConsumeScopeProvider](../masstransit-dependencyinjection/baseconsumescopeprovider)

## Methods

### **GetScopeContext\<TScopeContext, TPipeContext\>(TPipeContext, Func\<TPipeContext, IServiceScope, IDisposable, TScopeContext\>, Func\<TPipeContext, IServiceScope, IDisposable, TScopeContext\>, Func\<TPipeContext, IServiceScope, IServiceProvider, TPipeContext\>)**

```csharp
protected ValueTask<TScopeContext> GetScopeContext<TScopeContext, TPipeContext>(TPipeContext context, Func<TPipeContext, IServiceScope, IDisposable, TScopeContext> existingScopeContextFactory, Func<TPipeContext, IServiceScope, IDisposable, TScopeContext> createdScopeContextFactory, Func<TPipeContext, IServiceScope, IServiceProvider, TPipeContext> pipeContextFactory)
```

#### Type Parameters

`TScopeContext`<br/>

`TPipeContext`<br/>

#### Parameters

`context` TPipeContext<br/>

`existingScopeContextFactory` [Func\<TPipeContext, IServiceScope, IDisposable, TScopeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

`createdScopeContextFactory` [Func\<TPipeContext, IServiceScope, IDisposable, TScopeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

`pipeContextFactory` [Func\<TPipeContext, IServiceScope, IServiceProvider, TPipeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[ValueTask\<TScopeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>
