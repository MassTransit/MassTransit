---

title: DelegatePipeSpecification<T>

---

# DelegatePipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class DelegatePipeSpecification<T> : IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegatePipeSpecification\<T\>](../masstransit-configuration/delegatepipespecification-1)<br/>
Implements [IPipeSpecification\<T\>](../masstransit-configuration/ipipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **DelegatePipeSpecification(Action\<T\>)**

```csharp
public DelegatePipeSpecification(Action<T> callback)
```

#### Parameters

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **Apply(IPipeBuilder\<T\>)**

```csharp
public void Apply(IPipeBuilder<T> builder)
```

#### Parameters

`builder` [IPipeBuilder\<T\>](../masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
