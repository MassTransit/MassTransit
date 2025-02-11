---

title: SetScopedConsumeContext<TBus>

---

# SetScopedConsumeContext\<TBus\>

Namespace: MassTransit.DependencyInjection

```csharp
public class SetScopedConsumeContext<TBus> : ISetScopedConsumeContext
```

#### Type Parameters

`TBus`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetScopedConsumeContext\<TBus\>](../masstransit-dependencyinjection/setscopedconsumecontext-1)<br/>
Implements [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)

## Constructors

### **SetScopedConsumeContext(Func\<IServiceProvider, IScopedConsumeContextProvider\>)**

```csharp
public SetScopedConsumeContext(Func<IServiceProvider, IScopedConsumeContextProvider> setterProvider)
```

#### Parameters

`setterProvider` [Func\<IServiceProvider, IScopedConsumeContextProvider\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **PushContext(IServiceScope, ConsumeContext)**

```csharp
public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
