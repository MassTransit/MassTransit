---

title: Observes<TMessage, TSaga>

---

# Observes\<TMessage, TSaga\>

Namespace: MassTransit

```csharp
public interface Observes<TMessage, TSaga> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

`TSaga`<br/>

Implements [IConsumer\<TMessage\>](../masstransit/iconsumer-1), [IConsumer](../masstransit/iconsumer)

## Properties

### **CorrelationExpression**

```csharp
public abstract Expression<Func<TSaga, TMessage, bool>> CorrelationExpression { get; }
```

#### Property Value

Expression\<Func\<TSaga, TMessage, Boolean\>\><br/>
