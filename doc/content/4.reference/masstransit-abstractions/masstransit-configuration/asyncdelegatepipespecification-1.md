---

title: AsyncDelegatePipeSpecification<T>

---

# AsyncDelegatePipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class AsyncDelegatePipeSpecification<T> : IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncDelegatePipeSpecification\<T\>](../masstransit-configuration/asyncdelegatepipespecification-1)<br/>
Implements [IPipeSpecification\<T\>](../masstransit-configuration/ipipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **AsyncDelegatePipeSpecification(Func\<T, Task\>)**

```csharp
public AsyncDelegatePipeSpecification(Func<T, Task> callback)
```

#### Parameters

`callback` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
