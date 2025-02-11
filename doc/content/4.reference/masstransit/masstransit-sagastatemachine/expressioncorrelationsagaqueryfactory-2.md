---

title: ExpressionCorrelationSagaQueryFactory<TInstance, TData>

---

# ExpressionCorrelationSagaQueryFactory\<TInstance, TData\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ExpressionCorrelationSagaQueryFactory<TInstance, TData> : ISagaQueryFactory<TInstance, TData>, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExpressionCorrelationSagaQueryFactory\<TInstance, TData\>](../masstransit-sagastatemachine/expressioncorrelationsagaqueryfactory-2)<br/>
Implements [ISagaQueryFactory\<TInstance, TData\>](../../masstransit-abstractions/masstransit/isagaqueryfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExpressionCorrelationSagaQueryFactory(Expression\<Func\<TInstance, ConsumeContext\<TData\>, Boolean\>\>)**

```csharp
public ExpressionCorrelationSagaQueryFactory(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
```

#### Parameters

`correlationExpression` Expression\<Func\<TInstance, ConsumeContext\<TData\>, Boolean\>\><br/>

## Methods

### **TryCreateQuery(ConsumeContext\<TData\>, ISagaQuery\<TInstance\>)**

```csharp
public bool TryCreateQuery(ConsumeContext<TData> context, out ISagaQuery<TInstance> query)
```

#### Parameters

`context` [ConsumeContext\<TData\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TInstance\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
