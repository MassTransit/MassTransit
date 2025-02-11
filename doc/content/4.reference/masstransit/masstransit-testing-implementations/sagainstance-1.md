---

title: SagaInstance<T>

---

# SagaInstance\<T\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class SagaInstance<T> : ISagaInstance<T>, IAsyncListElement
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaInstance\<T\>](../masstransit-testing-implementations/sagainstance-1)<br/>
Implements [ISagaInstance\<T\>](../masstransit-testing/isagainstance-1), [IAsyncListElement](../masstransit-testing/iasynclistelement)

## Properties

### **Saga**

```csharp
public T Saga { get; }
```

#### Property Value

T<br/>

### **ElementId**

```csharp
public Nullable<Guid> ElementId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SagaInstance(T)**

```csharp
public SagaInstance(T saga)
```

#### Parameters

`saga` T<br/>
