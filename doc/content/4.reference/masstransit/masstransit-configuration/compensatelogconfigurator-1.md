---

title: CompensateLogConfigurator<TLog>

---

# CompensateLogConfigurator\<TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateLogConfigurator<TLog> : ICompensateLogConfigurator<TLog>, IPipeConfigurator<CompensateContext<TLog>>, IConsumeConfigurator
```

#### Type Parameters

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateLogConfigurator\<TLog\>](../masstransit-configuration/compensatelogconfigurator-1)<br/>
Implements [ICompensateLogConfigurator\<TLog\>](../../masstransit-abstractions/masstransit/icompensatelogconfigurator-1), [IPipeConfigurator\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator)

## Constructors

### **CompensateLogConfigurator(IPipeConfigurator\<CompensateContext\<TLog\>\>)**

```csharp
public CompensateLogConfigurator(IPipeConfigurator<CompensateContext<TLog>> configurator)
```

#### Parameters

`configurator` [IPipeConfigurator\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<CompensateContext\<TLog\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<CompensateContext<TLog>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<CompensateContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
