---

title: ITransformSpecificationConfigurator<TMessage>

---

# ITransformSpecificationConfigurator\<TMessage\>

Namespace: MassTransit

```csharp
public interface ITransformSpecificationConfigurator<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **Get\<T\>()**

Get a transform specification using the default constructor

```csharp
IConsumeTransformSpecification<TMessage> Get<T>()
```

#### Type Parameters

`T`<br/>
The transform specification type

#### Returns

[IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1)<br/>

### **Get\<T\>(Func\<T\>)**

Get a transform specification using the factory method

```csharp
IConsumeTransformSpecification<TMessage> Get<T>(Func<T> transformFactory)
```

#### Type Parameters

`T`<br/>
The transform specification type

#### Parameters

`transformFactory` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>
The transform specification factory method

#### Returns

[IConsumeTransformSpecification\<TMessage\>](../masstransit-configuration/iconsumetransformspecification-1)<br/>
