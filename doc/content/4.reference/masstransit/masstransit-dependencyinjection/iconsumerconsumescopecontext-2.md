---

title: IConsumerConsumeScopeContext<TConsumer, T>

---

# IConsumerConsumeScopeContext\<TConsumer, T\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IConsumerConsumeScopeContext<TConsumer, T> : IAsyncDisposable
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract ConsumerConsumeContext<TConsumer, T> Context { get; }
```

#### Property Value

[ConsumerConsumeContext\<TConsumer, T\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>
