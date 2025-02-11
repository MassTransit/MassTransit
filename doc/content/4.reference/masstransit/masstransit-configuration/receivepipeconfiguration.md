---

title: ReceivePipeConfiguration

---

# ReceivePipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public class ReceivePipeConfiguration : IReceivePipeConfiguration, IReceivePipeConfigurator, IPipeConfigurator<ReceiveContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivePipeConfiguration](../masstransit-configuration/receivepipeconfiguration)<br/>
Implements [IReceivePipeConfiguration](../masstransit-configuration/ireceivepipeconfiguration), [IReceivePipeConfigurator](../../masstransit-abstractions/masstransit/ireceivepipeconfigurator), [IPipeConfigurator\<ReceiveContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Specification**

```csharp
public ISpecification Specification { get; }
```

#### Property Value

[ISpecification](../../masstransit-abstractions/masstransit/ispecification)<br/>

### **Configurator**

```csharp
public IReceivePipeConfigurator Configurator { get; }
```

#### Property Value

[IReceivePipeConfigurator](../../masstransit-abstractions/masstransit/ireceivepipeconfigurator)<br/>

### **DeadLetterConfigurator**

```csharp
public IBuildPipeConfigurator<ReceiveContext> DeadLetterConfigurator { get; }
```

#### Property Value

[IBuildPipeConfigurator\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

### **ErrorConfigurator**

```csharp
public IBuildPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator { get; }
```

#### Property Value

[IBuildPipeConfigurator\<ExceptionReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ibuildpipeconfigurator-1)<br/>

## Constructors

### **ReceivePipeConfiguration()**

```csharp
public ReceivePipeConfiguration()
```

## Methods

### **CreatePipe(IConsumePipe, ISerialization)**

```csharp
public IReceivePipe CreatePipe(IConsumePipe consumePipe, ISerialization serializers)
```

#### Parameters

`consumePipe` [IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

`serializers` [ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

#### Returns

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

### **AddPipeSpecification(IPipeSpecification\<ReceiveContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ReceiveContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ReceiveContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
