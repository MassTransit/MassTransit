---

title: TransformSpecification<TMessage>

---

# TransformSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public abstract class TransformSpecification<TMessage> : ITransformConfigurator<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecification\<TMessage\>](../masstransit-configuration/transformspecification-1)<br/>
Implements [ITransformConfigurator\<TMessage\>](../masstransit/itransformconfigurator-1)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Replace**

```csharp
public bool Replace { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Default\<TProperty\>(Expression\<Func\<TMessage, TProperty\>\>)**

```csharp
public void Default<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TMessage, TProperty\>\><br/>

### **Set\<TProperty\>(Expression\<Func\<TMessage, TProperty\>\>, TProperty)**

```csharp
public void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression, TProperty value)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TMessage, TProperty\>\><br/>

`value` TProperty<br/>

### **Set\<TProperty\>(Expression\<Func\<TMessage, TProperty\>\>, Func\<TransformPropertyContext\<TProperty, TMessage\>, TProperty\>)**

```csharp
public void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression, Func<TransformPropertyContext<TProperty, TMessage>, TProperty> valueProvider)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TMessage, TProperty\>\><br/>

`valueProvider` [Func\<TransformPropertyContext\<TProperty, TMessage\>, TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Set\<TProperty\>(PropertyInfo, IPropertyProvider\<TMessage, TProperty\>)**

```csharp
public void Set<TProperty>(PropertyInfo propertyInfo, IPropertyProvider<TMessage, TProperty> propertyProvider)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`propertyProvider` [IPropertyProvider\<TMessage, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

### **Transform\<TProperty\>(PropertyInfo, IPropertyProvider\<TMessage, TProperty\>)**

```csharp
public void Transform<TProperty>(PropertyInfo propertyInfo, IPropertyProvider<TMessage, TProperty> propertyProvider)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`propertyProvider` [IPropertyProvider\<TMessage, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Build()**

```csharp
protected IMessageInitializer<TMessage> Build()
```

#### Returns

[IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>
