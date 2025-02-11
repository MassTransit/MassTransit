---

title: RescuePipeSpecification<TContext, TRescue>

---

# RescuePipeSpecification\<TContext, TRescue\>

Namespace: MassTransit.Configuration

```csharp
public class RescuePipeSpecification<TContext, TRescue> : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<TContext>, ISpecification, IRescueConfigurator<TContext, TRescue>, IPipeConfigurator<TRescue>
```

#### Type Parameters

`TContext`<br/>

`TRescue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [RescuePipeSpecification\<TContext, TRescue\>](../masstransit-configuration/rescuepipespecification-2)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IRescueConfigurator\<TContext, TRescue\>](../masstransit/irescueconfigurator-2), [IPipeConfigurator\<TRescue\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Constructors

### **RescuePipeSpecification(RescueContextFactory\<TContext, TRescue\>)**

```csharp
public RescuePipeSpecification(RescueContextFactory<TContext, TRescue> rescueContextFactory)
```

#### Parameters

`rescueContextFactory` [RescueContextFactory\<TContext, TRescue\>](../masstransit-middleware/rescuecontextfactory-2)<br/>

## Methods

### **Apply(IPipeBuilder\<TContext\>)**

```csharp
public void Apply(IPipeBuilder<TContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
