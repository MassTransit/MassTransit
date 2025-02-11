---

title: CompensateActivityLogConfigurator<TActivity, TLog>

---

# CompensateActivityLogConfigurator\<TActivity, TLog\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateActivityLogConfigurator<TActivity, TLog> : ICompensateActivityLogConfigurator<TLog>, IPipeConfigurator<CompensateActivityContext<TLog>>, IConsumeConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateActivityLogConfigurator\<TActivity, TLog\>](../masstransit-configuration/compensateactivitylogconfigurator-2)<br/>
Implements [ICompensateActivityLogConfigurator\<TLog\>](../../masstransit-abstractions/masstransit/icompensateactivitylogconfigurator-1), [IPipeConfigurator\<CompensateActivityContext\<TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator)

## Constructors

### **CompensateActivityLogConfigurator(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>)**

```csharp
public CompensateActivityLogConfigurator(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator)
```

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<CompensateActivityContext\<TLog\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TLog>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<CompensateActivityContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
