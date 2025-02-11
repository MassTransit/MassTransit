---

title: InstanceJobServiceSettings

---

# InstanceJobServiceSettings

Namespace: MassTransit.Configuration

```csharp
public class InstanceJobServiceSettings : JobServiceSettings, IOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceJobServiceSettings](../masstransit-configuration/instancejobservicesettings)<br/>
Implements [JobServiceSettings](../masstransit-jobservice/jobservicesettings), [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions)

## Properties

### **HeartbeatInterval**

```csharp
public TimeSpan HeartbeatInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **InstanceEndpointConfigurator**

```csharp
public IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; set; }
```

#### Property Value

[IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **JobService**

```csharp
public IJobService JobService { get; }
```

#### Property Value

[IJobService](../masstransit-jobservice/ijobservice)<br/>

## Constructors

### **InstanceJobServiceSettings(IOptions\<JobConsumerOptions\>)**

```csharp
public InstanceJobServiceSettings(IOptions<JobConsumerOptions> options)
```

#### Parameters

`options` IOptions\<JobConsumerOptions\><br/>

### **InstanceJobServiceSettings(JobConsumerOptions)**

```csharp
public InstanceJobServiceSettings(JobConsumerOptions options)
```

#### Parameters

`options` [JobConsumerOptions](../masstransit/jobconsumeroptions)<br/>

## Methods

### **ApplyConfiguration\<T\>(T)**

```csharp
public void ApplyConfiguration<T>(T configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>
