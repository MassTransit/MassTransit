---

title: PropertySagaInstanceFactory<TSaga>

---

# PropertySagaInstanceFactory\<TSaga\>

Namespace: MassTransit.Configuration

Creates a saga instance using the constructor, via a compiled expression. This class
 is built asynchronously and hot-wrapped to replace the basic Activator style.

```csharp
public class PropertySagaInstanceFactory<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertySagaInstanceFactory\<TSaga\>](../masstransit-configuration/propertysagainstancefactory-1)

## Properties

### **FactoryMethod**

```csharp
public SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
```

#### Property Value

[SagaInstanceFactoryMethod\<TSaga\>](../masstransit-saga/sagainstancefactorymethod-1)<br/>

## Constructors

### **PropertySagaInstanceFactory()**

```csharp
public PropertySagaInstanceFactory()
```
