---

title: JobServiceSettings

---

# JobServiceSettings

Namespace: MassTransit.JobService

Settings relevant to the job consumer endpoints and the service instance

```csharp
public interface JobServiceSettings : IOptions
```

Implements [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions)

## Properties

### **JobService**

```csharp
public abstract IJobService JobService { get; }
```

#### Property Value

[IJobService](../masstransit-jobservice/ijobservice)<br/>

### **HeartbeatInterval**

```csharp
public abstract TimeSpan HeartbeatInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InstanceAddress**

```csharp
public abstract Uri InstanceAddress { get; }
```

#### Property Value

Uri<br/>

### **InstanceEndpointConfigurator**

```csharp
public abstract IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; }
```

#### Property Value

[IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
