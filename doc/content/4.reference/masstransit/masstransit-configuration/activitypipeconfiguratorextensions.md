---

title: ActivityPipeConfiguratorExtensions

---

# ActivityPipeConfiguratorExtensions

Namespace: MassTransit.Configuration

```csharp
public static class ActivityPipeConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityPipeConfiguratorExtensions](../masstransit-configuration/activitypipeconfiguratorextensions)

## Methods

### **AddPipeSpecification\<TActivity, TArguments\>(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>, IPipeSpecification\<ExecuteActivityContext\<TArguments\>\>)**

```csharp
public static void AddPipeSpecification<TActivity, TArguments>(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator, IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`specification` [IPipeSpecification\<ExecuteActivityContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **AddPipeSpecification\<TActivity, TLog\>(IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>, IPipeSpecification\<CompensateActivityContext\<TLog\>\>)**

```csharp
public static void AddPipeSpecification<TActivity, TLog>(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator, IPipeSpecification<CompensateActivityContext<TLog>> specification)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`specification` [IPipeSpecification\<CompensateActivityContext\<TLog\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
