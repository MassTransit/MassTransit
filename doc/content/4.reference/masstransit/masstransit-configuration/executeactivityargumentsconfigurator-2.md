---

title: ExecuteActivityArgumentsConfigurator<TActivity, TArguments>

---

# ExecuteActivityArgumentsConfigurator\<TActivity, TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteActivityArgumentsConfigurator<TActivity, TArguments> : IExecuteActivityArgumentsConfigurator<TArguments>, IPipeConfigurator<ExecuteActivityContext<TArguments>>, IConsumeConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityArgumentsConfigurator\<TActivity, TArguments\>](../masstransit-configuration/executeactivityargumentsconfigurator-2)<br/>
Implements [IExecuteActivityArgumentsConfigurator\<TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityargumentsconfigurator-1), [IPipeConfigurator\<ExecuteActivityContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator)

## Constructors

### **ExecuteActivityArgumentsConfigurator(IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>)**

```csharp
public ExecuteActivityArgumentsConfigurator(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator)
```

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteActivityContext\<TActivity, TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ExecuteActivityContext\<TArguments\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ExecuteActivityContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
