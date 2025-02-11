---

title: ConsumeScopeProvider

---

# ConsumeScopeProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class ConsumeScopeProvider : BaseConsumeScopeProvider, IConsumeScopeProvider, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseConsumeScopeProvider](../masstransit-dependencyinjection/baseconsumescopeprovider) → [ConsumeScopeProvider](../masstransit-dependencyinjection/consumescopeprovider)<br/>
Implements [IConsumeScopeProvider](../masstransit-dependencyinjection/iconsumescopeprovider), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumeScopeProvider(IRegistrationContext)**

```csharp
public ConsumeScopeProvider(IRegistrationContext context)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

### **ConsumeScopeProvider(IServiceProvider, ISetScopedConsumeContext)**

```csharp
public ConsumeScopeProvider(IServiceProvider serviceProvider, ISetScopedConsumeContext setScopedConsumeContext)
```

#### Parameters

`serviceProvider` IServiceProvider<br/>

`setScopedConsumeContext` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **GetScope(ConsumeContext)**

```csharp
public ValueTask<IConsumeScopeContext> GetScope(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[ValueTask\<IConsumeScopeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetScope\<T\>(ConsumeContext\<T\>)**

```csharp
public ValueTask<IConsumeScopeContext<T>> GetScope<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[ValueTask\<IConsumeScopeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetScope\<TConsumer, T\>(ConsumeContext\<T\>)**

```csharp
public ValueTask<IConsumerConsumeScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[ValueTask\<IConsumerConsumeScopeContext\<TConsumer, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>
