---

title: ExecuteArgumentsConfigurator<TArguments>

---

# ExecuteArgumentsConfigurator\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteArgumentsConfigurator<TArguments> : IExecuteArgumentsConfigurator<TArguments>, IPipeConfigurator<ExecuteContext<TArguments>>, IConsumeConfigurator
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteArgumentsConfigurator\<TArguments\>](../masstransit-configuration/executeargumentsconfigurator-1)<br/>
Implements [IExecuteArgumentsConfigurator\<TArguments\>](../../masstransit-abstractions/masstransit/iexecuteargumentsconfigurator-1), [IPipeConfigurator\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator)

## Constructors

### **ExecuteArgumentsConfigurator(IPipeConfigurator\<ExecuteContext\<TArguments\>\>)**

```csharp
public ExecuteArgumentsConfigurator(IPipeConfigurator<ExecuteContext<TArguments>> configurator)
```

#### Parameters

`configurator` [IPipeConfigurator\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ExecuteContext\<TArguments\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ExecuteContext<TArguments>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>
