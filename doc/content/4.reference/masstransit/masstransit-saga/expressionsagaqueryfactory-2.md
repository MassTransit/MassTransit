---

title: ExpressionSagaQueryFactory<TSaga, TMessage>

---

# ExpressionSagaQueryFactory\<TSaga, TMessage\>

Namespace: MassTransit.Saga

Creates a saga query using the specified filter expression

```csharp
public class ExpressionSagaQueryFactory<TSaga, TMessage> : ISagaQueryFactory<TSaga, TMessage>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExpressionSagaQueryFactory\<TSaga, TMessage\>](../masstransit-saga/expressionsagaqueryfactory-2)<br/>
Implements [ISagaQueryFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagaqueryfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExpressionSagaQueryFactory(Expression\<Func\<TSaga, TMessage, Boolean\>\>)**

```csharp
public ExpressionSagaQueryFactory(Expression<Func<TSaga, TMessage, bool>> filterExpression)
```

#### Parameters

`filterExpression` Expression\<Func\<TSaga, TMessage, Boolean\>\><br/>
