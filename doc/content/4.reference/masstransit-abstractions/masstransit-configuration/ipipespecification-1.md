---

title: IPipeSpecification<TContext>

---

# IPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

Configures a pipe builder (typically by adding filters), but allows late binding to the
 pipe builder with pre-validation that the operations will succeed.

```csharp
public interface IPipeSpecification<TContext> : ISpecification
```

#### Type Parameters

`TContext`<br/>

Implements [ISpecification](../masstransit/ispecification)

## Methods

### **Apply(IPipeBuilder\<TContext\>)**

Apply the specification to the builder

```csharp
void Apply(IPipeBuilder<TContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TContext\>](../masstransit-configuration/ipipebuilder-1)<br/>
The pipe builder
