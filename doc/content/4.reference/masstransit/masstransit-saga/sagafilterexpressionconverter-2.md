---

title: SagaFilterExpressionConverter<TSaga, TMessage>

---

# SagaFilterExpressionConverter\<TSaga, TMessage\>

Namespace: MassTransit.Saga

```csharp
public class SagaFilterExpressionConverter<TSaga, TMessage> : ExpressionVisitor
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → ExpressionVisitor → [SagaFilterExpressionConverter\<TSaga, TMessage\>](../masstransit-saga/sagafilterexpressionconverter-2)

## Constructors

### **SagaFilterExpressionConverter(TMessage)**

```csharp
public SagaFilterExpressionConverter(TMessage message)
```

#### Parameters

`message` TMessage<br/>

## Methods

### **Convert(Expression\<Func\<TSaga, TMessage, Boolean\>\>)**

```csharp
public Expression<Func<TSaga, bool>> Convert(Expression<Func<TSaga, TMessage, bool>> expression)
```

#### Parameters

`expression` Expression\<Func\<TSaga, TMessage, Boolean\>\><br/>

#### Returns

Expression\<Func\<TSaga, Boolean\>\><br/>

### **VisitMember(MemberExpression)**

```csharp
protected Expression VisitMember(MemberExpression m)
```

#### Parameters

`m` MemberExpression<br/>

#### Returns

Expression<br/>
