---

title: StateExpressionVisitor<TInstance>

---

# StateExpressionVisitor\<TInstance\>

Namespace: MassTransit.Saga

```csharp
public class StateExpressionVisitor<TInstance> : ExpressionVisitor
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → ExpressionVisitor → [StateExpressionVisitor\<TInstance\>](../masstransit-saga/stateexpressionvisitor-1)

## Constructors

### **StateExpressionVisitor(Expression\<Func\<TInstance, Boolean\>\>)**

```csharp
public StateExpressionVisitor(Expression<Func<TInstance, bool>> expression)
```

#### Parameters

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

## Methods

### **VisitParameter(ParameterExpression)**

```csharp
protected Expression VisitParameter(ParameterExpression node)
```

#### Parameters

`node` ParameterExpression<br/>

#### Returns

Expression<br/>

### **Combine(Expression\<Func\<TInstance, Boolean\>\>, Expression\<Func\<TInstance, Boolean\>\>)**

```csharp
public static Expression<Func<TInstance, bool>> Combine(Expression<Func<TInstance, bool>> expression, Expression<Func<TInstance, bool>> stateExpression)
```

#### Parameters

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`stateExpression` Expression\<Func\<TInstance, Boolean\>\><br/>

#### Returns

Expression\<Func\<TInstance, Boolean\>\><br/>
