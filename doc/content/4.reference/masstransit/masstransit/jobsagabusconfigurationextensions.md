---

title: JobSagaBusConfigurationExtensions

---

# JobSagaBusConfigurationExtensions

Namespace: MassTransit

```csharp
public static class JobSagaBusConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSagaBusConfigurationExtensions](../masstransit/jobsagabusconfigurationextensions)

## Methods

### **UseJobSagaPartitionKeyFormatters(IBusFactoryConfigurator)**

Add partition key formatters to support partitioned transports

```csharp
public static void UseJobSagaPartitionKeyFormatters(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **SetPartitionedReceiveMode(IJobSagaRegistrationConfigurator)**

Configure the job saga receive endpoints to use the SQL transport partitioned receive mode

```csharp
public static IJobSagaRegistrationConfigurator SetPartitionedReceiveMode(IJobSagaRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>
