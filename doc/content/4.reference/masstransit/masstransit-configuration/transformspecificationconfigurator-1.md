---

title: TransformSpecificationConfigurator<TMessage>

---

# TransformSpecificationConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class TransformSpecificationConfigurator<TMessage> : ITransformSpecificationConfigurator<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformSpecificationConfigurator\<TMessage\>](../masstransit-configuration/transformspecificationconfigurator-1)<br/>
Implements [ITransformSpecificationConfigurator\<TMessage\>](../masstransit/itransformspecificationconfigurator-1)

## Constructors

### **TransformSpecificationConfigurator()**

```csharp
public TransformSpecificationConfigurator()
```

## Methods

### **Get\<T\>()**

```csharp
public IConsumeTransformSpecification<TMessage> Get<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1)<br/>

### **Get\<T\>(Func\<T\>)**

```csharp
public IConsumeTransformSpecification<TMessage> Get<T>(Func<T> transformFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`transformFactory` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1)<br/>
