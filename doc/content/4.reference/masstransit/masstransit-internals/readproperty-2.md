---

title: ReadProperty<T, TProperty>

---

# ReadProperty\<T, TProperty\>

Namespace: MassTransit.Internals

```csharp
public class ReadProperty<T, TProperty> : IReadProperty<T, TProperty>, IReadProperty<T>
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadProperty\<T, TProperty\>](../masstransit-internals/readproperty-2)<br/>
Implements [IReadProperty\<T, TProperty\>](../masstransit-internals/ireadproperty-2), [IReadProperty\<T\>](../masstransit-internals/ireadproperty-1)

## Constructors

### **ReadProperty(PropertyInfo)**

```csharp
public ReadProperty(PropertyInfo propertyInfo)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

## Methods

### **Get(T)**

```csharp
public TProperty Get(T content)
```

#### Parameters

`content` T<br/>

#### Returns

TProperty<br/>
