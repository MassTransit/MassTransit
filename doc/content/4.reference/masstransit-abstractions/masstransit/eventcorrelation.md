---

title: EventCorrelation

---

# EventCorrelation

Namespace: MassTransit

```csharp
public interface EventCorrelation : ISpecification
```

Implements [ISpecification](../masstransit/ispecification)

## Properties

### **DataType**

The data type for the event

```csharp
public abstract Type DataType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ConfigureConsumeTopology**

```csharp
public abstract bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
