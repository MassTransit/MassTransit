---

title: ITransformConfigurator<TInput>

---

# ITransformConfigurator\<TInput\>

Namespace: MassTransit

```csharp
public interface ITransformConfigurator<TInput>
```

#### Type Parameters

`TInput`<br/>

## Properties

### **Replace**

Specifies if the message should be replaced, meaning modified in-place, instead of creating a new message

```csharp
public abstract bool Replace { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Default\<TProperty\>(Expression\<Func\<TInput, TProperty\>\>)**

Set the specified message property to the default value (ignoring the input value)

```csharp
void Default<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TInput, TProperty\>\><br/>

### **Set\<TProperty\>(Expression\<Func\<TInput, TProperty\>\>, TProperty)**

Set the specified property to a constant value

```csharp
void Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, TProperty value)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TInput, TProperty\>\><br/>

`value` TProperty<br/>

### **Set\<TProperty\>(Expression\<Func\<TInput, TProperty\>\>, Func\<TransformPropertyContext\<TProperty, TInput\>, TProperty\>)**

Set the property to the value, using the source context to create/select the value

```csharp
void Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, Func<TransformPropertyContext<TProperty, TInput>, TProperty> valueProvider)
```

#### Type Parameters

`TProperty`<br/>
The property type

#### Parameters

`propertyExpression` Expression\<Func\<TInput, TProperty\>\><br/>
The property select expression

`valueProvider` [Func\<TransformPropertyContext\<TProperty, TInput\>, TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The method to return the property

### **Set\<TProperty\>(PropertyInfo, IPropertyProvider\<TInput, TProperty\>)**

Set the property to the value, using the property provider specified

```csharp
void Set<TProperty>(PropertyInfo property, IPropertyProvider<TInput, TProperty> propertyProvider)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`propertyProvider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

### **Transform\<TProperty\>(PropertyInfo, IPropertyProvider\<TInput, TProperty\>)**

Transform the property, but leave it unchanged on the input

```csharp
void Transform<TProperty>(PropertyInfo property, IPropertyProvider<TInput, TProperty> propertyProvider)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`property` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

`propertyProvider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>
