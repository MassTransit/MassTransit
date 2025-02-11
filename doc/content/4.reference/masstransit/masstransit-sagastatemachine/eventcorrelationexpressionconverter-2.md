---

title: EventCorrelationExpressionConverter<TInstance, TMessage>

---

# EventCorrelationExpressionConverter\<TInstance, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class EventCorrelationExpressionConverter<TInstance, TMessage> : ExpressionVisitor
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → ExpressionVisitor → [EventCorrelationExpressionConverter\<TInstance, TMessage\>](../masstransit-sagastatemachine/eventcorrelationexpressionconverter-2)

## Constructors

### **EventCorrelationExpressionConverter(ConsumeContext\<TMessage\>)**

```csharp
public EventCorrelationExpressionConverter(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

## Methods

### **Convert(Expression\<Func\<TInstance, ConsumeContext\<TMessage\>, Boolean\>\>)**

```csharp
public Expression<Func<TInstance, bool>> Convert(Expression<Func<TInstance, ConsumeContext<TMessage>, bool>> expression)
```

#### Parameters

`expression` Expression\<Func\<TInstance, ConsumeContext\<TMessage\>, Boolean\>\><br/>

#### Returns

Expression\<Func\<TInstance, Boolean\>\><br/>

### **VisitMember(MemberExpression)**

```csharp
protected Expression VisitMember(MemberExpression m)
```

#### Parameters

`m` MemberExpression<br/>

#### Returns

Expression<br/>
