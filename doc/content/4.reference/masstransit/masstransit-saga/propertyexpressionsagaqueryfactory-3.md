---

title: PropertyExpressionSagaQueryFactory<TInstance, TData, TProperty>

---

# PropertyExpressionSagaQueryFactory\<TInstance, TData, TProperty\>

Namespace: MassTransit.Saga

```csharp
public class PropertyExpressionSagaQueryFactory<TInstance, TData, TProperty> : ISagaQueryFactory<TInstance, TData>, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyExpressionSagaQueryFactory\<TInstance, TData, TProperty\>](../masstransit-saga/propertyexpressionsagaqueryfactory-3)<br/>
Implements [ISagaQueryFactory\<TInstance, TData\>](../../masstransit-abstractions/masstransit/isagaqueryfactory-2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **PropertyExpressionSagaQueryFactory(Expression\<Func\<TInstance, TProperty\>\>, ISagaQueryPropertySelector\<TData, TProperty\>)**

```csharp
public PropertyExpressionSagaQueryFactory(Expression<Func<TInstance, TProperty>> propertyExpression, ISagaQueryPropertySelector<TData, TProperty> selector)
```

#### Parameters

`propertyExpression` Expression\<Func\<TInstance, TProperty\>\><br/>

`selector` [ISagaQueryPropertySelector\<TData, TProperty\>](../masstransit-configuration/isagaquerypropertyselector-2)<br/>
