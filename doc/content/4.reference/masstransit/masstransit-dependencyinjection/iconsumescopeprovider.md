---

title: IConsumeScopeProvider

---

# IConsumeScopeProvider

Namespace: MassTransit.DependencyInjection

Provides container scope for the consumer, either at the general level or the message-specific level.

```csharp
public interface IConsumeScopeProvider : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetScope(ConsumeContext)**

```csharp
ValueTask<IConsumeScopeContext> GetScope(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[ValueTask\<IConsumeScopeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetScope\<T\>(ConsumeContext\<T\>)**

```csharp
ValueTask<IConsumeScopeContext<T>> GetScope<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[ValueTask\<IConsumeScopeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>

### **GetScope\<TConsumer, T\>(ConsumeContext\<T\>)**

```csharp
ValueTask<IConsumerConsumeScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[ValueTask\<IConsumerConsumeScopeContext\<TConsumer, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)<br/>
